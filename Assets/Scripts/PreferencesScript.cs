using UnityEngine;
using System;

public class PreferencesScript : MonoBehaviour {

    private const string FIRST_USE = "FirstUse";


    // ______________________Color mode variables_________________________________

    private const string COLOR_MODE = "ColorMode";

    public ColorMode currentMode;

    /// <summary>
    /// How long tha changing animation should take
    /// </summary>
    private const float changeDuration = 0.5f;

    /// <summary>
    /// Delegate used for color changes
    /// </summary>
    public delegate void OnColorChange(ColorMode mode, float time);
    /// <summary>
    /// When we subscribe to this we can be sure that the color in SignResourceScript has already been changed
    /// </summary>
    public static OnColorChange ColorChangeEvent;

    // _______________________Which colors are chosen in colormode____________________________
    private ColorTheme currentTheme;

    private void Start() {

        DontDestroyOnLoad(gameObject);

        // If first use
        if (PlayerPrefs.GetString(FIRST_USE) == "") {
            PlayerPrefs.SetString(COLOR_MODE, ColorMode.LIGHT.ToString());
            PlayerPrefs.SetString(FIRST_USE, "IMBEINUSEDJUSTLIKEINREALLIFE");
        }
        
        // Color mode
        currentMode = (ColorMode) Enum.Parse(typeof(ColorMode), PlayerPrefs.GetString(COLOR_MODE));
        currentTheme = ColorThemes.DefaultTheme;
        UpdateSignResourceStrgColors();
    }



    public void ChangeToColorMode(ColorMode mode) {
        currentMode = mode;
        PlayerPrefs.SetString(COLOR_MODE, currentMode.ToString());

        UpdateSignResourceStrgColors();
        ColorChangeEvent(mode, changeDuration);
    }
	
    private void UpdateSignResourceStrgColors() {
        SignResourceStorage.ChangeToColorMode(currentTheme.GetXColorOfMode(currentMode), currentTheme.GetOColorOfMode(currentMode));
    }


    public enum ColorMode {
        DARK, LIGHT
    }

    /// <summary>
    /// Which color theme is chosen in ColorMode
    /// </summary>
    public struct ColorTheme {
        public Color xColorLight;
        public Color oColorLight;

        public Color xColorDark;
        public Color oColorDark;

        public ColorTheme(Color xColorLight, Color oColorLight, Color xColorDark, Color oColorDark) {
            this.xColorDark = xColorDark;
            this.oColorDark = oColorDark;
            this.xColorLight = xColorLight;
            this.oColorLight = oColorLight;
        }

        public Color GetXColorOfMode(ColorMode mode) {
            switch (mode) {
                case ColorMode.DARK: return xColorDark;
                case ColorMode.LIGHT: return xColorLight;
            }
            return Color.red;
        }

        public Color GetOColorOfMode(ColorMode mode) {
            switch (mode) {
                case ColorMode.DARK: return oColorDark;
                case ColorMode.LIGHT: return oColorLight;
            }
            return Color.blue;
        }
    }

}
