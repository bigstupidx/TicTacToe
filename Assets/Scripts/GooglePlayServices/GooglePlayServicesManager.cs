using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.Multiplayer;

public class GooglePlayServicesManager : Singleton<GooglePlayServicesManager> {

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
            // registers a callback for turn based match notifications received while the game is not running
            .WithMatchDelegate(OnGotMatch)
            .Build();

        // Activate play games platform and enable debugging
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();

        if (!Social.localUser.authenticated) {
            Debug.Log("Prompt player");
            // the player is not signed in so prompt them to do so
            PopupManager.Instance.PopUp(new PopUpTwoButton(
                "You are not signed in to Google Play.\nDo you want to sign in?", "No", "Sign in").Builder()
                .SetButtonColors(new Color(0.95686f, 0.26275f, 0.21176f), new Color(0.29804f, 0.68627f, 0.31373f))
                .SetButtonTextColors(Color.white, Color.white)
                .SetButtonPressActions(() => { }, () => {
                    PreferencesScript.Instance.GPFromNowCanAutoLogin();
                })
            );
        }

        if (PreferencesScript.Instance.GPCanAutoLogin()) { 
            // authenticate user
            Social.localUser.Authenticate((bool success) => {
            
            });
        }
    }

    private void OnInvitationReceived(Invitation invitation, bool shouldAutoAccept) {

    }

    private void OnGotMatch(TurnBasedMatch match, bool shouldAutoLaunch) {

    }

}
