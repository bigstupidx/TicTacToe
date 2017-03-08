using AppodealAds.Unity.Api;
using AppodealAds.Unity.Common;

public class AppodealManager : Singleton<AppodealManager> {
    
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
        for (int i = 0; i < hideOnScreens.Length; i++) {
            if (hideOnScreens[i] == to) {
                Appodeal.hide(Appodeal.BANNER_BOTTOM);
                return;
            }
        }

        Appodeal.show(Appodeal.BANNER_BOTTOM);
    }
}
