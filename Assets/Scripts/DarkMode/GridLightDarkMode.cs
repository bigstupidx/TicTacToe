using UnityEngine;
using DG.Tweening;

public class GridLightDarkMode : MonoBehaviour {

    public Color lightModeColor = new Color(0.88627f, 0.88627f, 0.88627f);
    public Color darkModeColor = new Color(0.25882f, 0.25882f, 0.25882f);
    
    public void ToLightMode(float time) {
        foreach (Transform transform in transform.GetChild(0)) {
            transform.GetComponent<SpriteRenderer>().DOColor(lightModeColor, time);
        }
    }

    public void ToDarkMode(float time) {
        foreach (Transform transform in transform.GetChild(0)) {
            transform.GetComponent<SpriteRenderer>().DOColor(darkModeColor, time);
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
