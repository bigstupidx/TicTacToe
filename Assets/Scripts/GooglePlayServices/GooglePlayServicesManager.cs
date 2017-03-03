using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.Multiplayer;
using System.Net;
using System.IO;
using System.Threading;
using System.Collections;

public class GooglePlayServicesManager : Singleton<GooglePlayServicesManager> {

    private Thread checkThread;
    private string htmlText = string.Empty;

    void Start() {
        if (FindObjectsOfType<GooglePlayServicesManager>().Length >= 2) {
            Destroy(gameObject);

            return;
        }

        DontDestroyOnLoad(gameObject);

        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
            // enables saving game progress.
            .EnableSavedGames()
            // registers a callback to handle game invitations received while the game is not running
            .WithInvitationDelegate(OnInvitationReceived)
            .Build();

        // Activate play games platform and enable debugging
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();

        if (PreferencesScript.Instance.GPCanAutoLogin()) {
            // authenticate user
            Social.localUser.Authenticate((bool success) => {

            });
        }

        checkThread = new Thread(new ThreadStart(() => {
            // We check for internet connection
            htmlText = GetHtmlFromUri("http://google.com");
        }));
        checkThread.Start();

        StartCoroutine(CheckForGooglePlaySignedIn());
    }

    private IEnumerator CheckForGooglePlaySignedIn() {
        yield return new WaitUntil(() => htmlText == string.Empty);

        // this phrase is in the beginning of the google page
        if (htmlText.Contains("schema.org/WebPage") && !Social.localUser.authenticated) {
            // the player is not signed in so prompt them to do so
            PopupManager.Instance.PopUp(new PopUpTwoButton(
                "You are not signed in to Google Play.\nDo you want to sign in?", "No", "Sign in").Builder()
                .SetButtonColors(new Color(0.95686f, 0.26275f, 0.21176f), new Color(0.29804f, 0.68627f, 0.31373f))
                .SetButtonTextColors(Color.white, Color.white)
                .SetButtonPressActions(() => { }, () => {
                    PreferencesScript.Instance.GPFromNowCanAutoLogin();
                    Social.localUser.Authenticate((bool success) => { });
                })
            );
        }
    }

    private void OnInvitationReceived(Invitation invitation, bool shouldAutoAccept) {
        Debug.Log("Invitation recieved");
        if (shouldAutoAccept && !shouldAutoAccept) {
            PopupManager.Instance.PopUp(
                new PopUpTwoButton(invitation.Inviter.DisplayName + "\nwould like to play with you!", "Decline", "Accept")
                    .SetButtonColors(new Color(0.95686f, 0.26275f, 0.21176f), new Color(0.29804f, 0.68627f, 0.31373f))
                    .SetButtonTextColors(Color.white, Color.white)
                    .SetButtonPressActions(() => { }, () => {
                        ScaneManager.Instance.GoToSceneThenDo("GooglePlayConnectScreen", () => {
                            PlayGamesPlatform.Instance.RealTime.AcceptInvitation(invitation.InvitationId, FindObjectOfType<GooglePlayGameManager>());
                        });
                    })
                );
        }
    }

    public static string GetHtmlFromUri(string resource) {
        string html = string.Empty;
        HttpWebRequest req = (HttpWebRequest) WebRequest.Create(resource);
        try {
            using (HttpWebResponse resp = (HttpWebResponse) req.GetResponse()) {
                bool isSuccess = (int) resp.StatusCode < 299 && (int) resp.StatusCode >= 200;
                if (isSuccess) {
                    using (StreamReader reader = new StreamReader(resp.GetResponseStream())) {
                        //We are limiting the array to 80 so we don't have
                        //to parse the entire html document feel free to 
                        //adjust (probably stay under 300)
                        char[] cs = new char[80];
                        reader.Read(cs, 0, cs.Length);
                        foreach (char ch in cs) {
                            html += ch;
                        }
                    }
                }
            }
        } catch {
            return "";
        }
        return html;
    }

}
