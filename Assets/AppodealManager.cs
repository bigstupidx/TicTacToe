using AppodealAds.Unity.Api;
using AppodealAds.Unity.Common;
using UnityEngine;

public class AppodealManager : Singleton<AppodealManager> {

    /// <summary>
    /// After how many games we should show an ad
    /// </summary>
    [HideInInspector]
    private int afterGameCountShowAd = 5;
    public string[] hideOnScreens;

	void Start() {
        if (FindObjectsOfType<AppodealManager>().Length >= 2) {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        // Init it
        string appKey = "c65365488495257d6e73717447e1a3a1063672d6c0921f9d";
        Appodeal.disableLocationPermissionCheck();
        Appodeal.initialize(appKey, Appodeal.INTERSTITIAL | Appodeal.BANNER);

        ScaneManager.OnScreenChange += OnScreenChange;
    }

    private void OnScreenChange(string from, string to) {
        Debug.Log("Screen change detected in appodeal " + from + " " + to);
        if (to == "Game" || to == "GameAI" || to == "ServerBluetoothGame" || to == "GooglePlayGameServer") {
            gameCounter = 0;
            FindObjectOfType<TTTGameLogic>().SomeoneWonGameEvent += GameEndAd;
        } else if (to == "GooglePlayGameClient" || to == "ClientBluetoothGame") {
            gameCounter = 0;
            ClientCellStorage.SomeoneWonEvent += GameEndAd;
        } else {
            ClientCellStorage.SomeoneWonEvent -= GameEndAd;

            TTTGameLogic gl = FindObjectOfType<TTTGameLogic>();
            if (gl != null) gl.SomeoneWonGameEvent -= GameEndAd;
        }

        /* for (int i = 0; i < hideOnScreens.Length; i++) {
            if (hideOnScreens[i] == to) {
                Appodeal.hide(Appodeal.BANNER_BOTTOM);
                return;
            }
        }

        Appodeal.show(Appodeal.BANNER_BOTTOM); */
    }

    int gameCounter = 0;
    private void GameEndAd(Cell.CellOcc type) {
        gameCounter++;
        Debug.Log("incremented gamendthing in appodeal " + gameCounter + " " + afterGameCountShowAd);

        if (gameCounter == afterGameCountShowAd) {
            Appodeal.show(Appodeal.INTERSTITIAL);
            gameCounter = 0;
        }
    }
}
