using UnityEngine;
using DG.Tweening;

public class CamColorModeToPrefs : MonoBehaviour {

    private PreferencesScript preferences;

    /// <summary>
    /// Dark background's color
    /// </summary>
    public Color darkBackgroundColor = new Color(0.129412f, 0.129412f, 0.129412f);

    /// <summary>
    /// Light background's color
    /// </summary>
    public Color lightBackgroundColor = Color.white;

    private Camera myCamera;

    void Start () {
        preferences = FindObjectOfType<PreferencesScript>();
        myCamera = GetComponent<Camera>();

        // Susbcribe to event
        preferences.ColorChangeEvent += ToMode;

        ToMode(preferences.currentMode, 0);
	}

    void OnDestroy() {
        // Unsubscribe from color event
        preferences.ColorChangeEvent -= ToMode;
    }

    public void ToDarkMode(float time) {
        myCamera.DOColor(darkBackgroundColor, time);
    }
	
    public void ToLightMode(float time) {
        myCamera.DOColor(lightBackgroundColor, time);
    }

    public void ToMode(PreferencesScript.ColorMode colorMode, float time) {
        switch (colorMode) {
            case PreferencesScript.ColorMode.LIGHT: ToLightMode(time); break;
            case PreferencesScript.ColorMode.DARK: ToDarkMode(time); break;
        }
    }
}
