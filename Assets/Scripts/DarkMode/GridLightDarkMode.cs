using UnityEngine;
using DG.Tweening;

public class GridLightDarkMode : MonoBehaviour {

    public Color lightModeColor = new Color(0.88627f, 0.88627f, 0.88627f);
    public Color darkModeColor = new Color(0.12941f, 0.12941f, 0.12941f);

    void Start() {
        PreferencesScript.ColorChangeEvent += ToMode;
    }

    void OnDestroy() {
        PreferencesScript.ColorChangeEvent -= ToMode;
    }
    
    public void ToLightMode(float time) {
        foreach (Transform transform in transform) {
            transform.GetComponent<SpriteRenderer>().DOColor(lightModeColor, time);
        }
    }

    public void ToDarkMode(float time) {
        foreach (Transform transform in transform) {
            transform.GetComponent<SpriteRenderer>().DOColor(darkModeColor, time);
        }
    }

    public void ToMode(PreferencesScript.ColorMode mode, float time) {
        switch (mode) {
            case PreferencesScript.ColorMode.LIGHT:
                ToLightMode(time);
                break;
            case PreferencesScript.ColorMode.DARK:
                ToDarkMode(time);
                break;
        }
    }

    /// <summary>
    /// Returns this objects corresponding color to the colormode
    /// </summary>
    public Color GetCorrespondingColor(PreferencesScript.ColorMode colorMode) {
        switch (colorMode) {
            case PreferencesScript.ColorMode.LIGHT: return lightModeColor;
            case PreferencesScript.ColorMode.DARK: return darkModeColor;
            default: return Color.magenta;
        }
    }
	
}
