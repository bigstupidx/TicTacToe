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
