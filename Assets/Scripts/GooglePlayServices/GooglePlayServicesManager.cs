using UnityEngine;
using UnityEngine.SocialPlatforms;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.Multiplayer;

public class GooglePlayServicesManager : Singleton<GooglePlayServicesManager> {
    
    public event InvitationReceivedDelegate InvitationOffScreenEvent;
    public event MatchDelegate MatchOffScreenDelegate;

    void Start() {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
            // enables saving game progress.
            .EnableSavedGames()
            // registers a callback to handle game invitations received while the game is not running
            .WithInvitationDelegate(InvitationOffScreenEvent)
            // registers a callback for turn based match notifications received while the game is not running
            .WithMatchDelegate(MatchOffScreenDelegate)
            .Build();

        // Activate play games platform and enable debugging
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();


        // authenticate user
        Social.localUser.Authenticate((bool success) => {
            Debug.Log("Authentication state " + success);
        });
    }
	
}
