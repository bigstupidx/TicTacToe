using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    /// <summary>
    /// It tries to set it's proper component to the proper color. It tries it in this order: image, text mesh pro, text.
    /// </summary>
    public void SetColorToCurrentColorMode() {
        Image img = GetComponent<Image>();

        if (img != null) {
            img.color = GetColorOfMode(PreferencesScript.Instance.currentMode);
            return;
        }

        TextMeshProUGUI text = GetComponent<TextMeshProUGUI>();

        if (text != null) {
            text.color = GetColorOfMode(PreferencesScript.Instance.currentMode);
            return;
        }

        Text txt = GetComponent<Text>();

        if (txt != null) {
            txt.color = GetColorOfMode(PreferencesScript.Instance.currentMode);
            return;
        }
    }

}
