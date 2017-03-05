using UnityEngine;

public class DarkLightColor : MonoBehaviour {

    public Color lightModeColor = Color.white;
    public Color darkModeColor = new Color(0.14902f, 0.19608f, 0.219608f);

    public Color GetColorOfMode(PreferencesScript.ColorMode mode) {
        switch (mode) {
            case PreferencesScript.ColorMode.DARK: return darkModeColor;
            case PreferencesScript.ColorMode.LIGHT: return lightModeColor;
        }
        return Color.magenta;
    }

}
