using UnityEngine;
using System;

public class PreferencesScript : MonoBehaviour {

    private const string FIRST_USE = "FirstUse";


    // ______________________Color mode variables_________________________________

    private const string COLOR_MODE = "ColorMode";
    private const string THEME_MODE = "ThemeMode";

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

    /// <summary>
    /// Delegate used for theme changes
    /// </summary>
    public delegate void OnThemeChange(ColorTheme newTheme, float time);
    /// <summary>
    /// Subscribe to get notification when theme changes
    /// When we subscribe to this we can be sure that the color in SignResourceScript has already been changed
    /// </summary>
    public static OnThemeChange ThemeChangeEvent;

    private void Start() {

        DontDestroyOnLoad(gameObject);

        // If first use
        if (PlayerPrefs.GetString(FIRST_USE) == "IMBEINUSEDJUSTLIKEINREALLIFE") {
            PlayerPrefs.SetString(COLOR_MODE, ColorMode.LIGHT.ToString());
            PlayerPrefs.SetString(THEME_MODE, "DefaultTheme");
            PlayerPrefs.SetString(FIRST_USE, "IMDEADINSIDE");
        }
        
        // Color mode
        currentMode = (ColorMode) Enum.Parse(typeof(ColorMode), PlayerPrefs.GetString(COLOR_MODE));
        currentTheme = ColorThemes.GetTheme(PlayerPrefs.GetString(THEME_MODE));
        UpdateSignResourceStrgColors();
    }



    public void ChangeToColorMode(ColorMode mode) {
        currentMode = mode;
        PlayerPrefs.SetString(COLOR_MODE, currentMode.ToString());

        UpdateSignResourceStrgColors(); // First update colors because some delegate listeners use it for simplicity
        ColorChangeEvent(mode, changeDuration); // Call delaegateategateggatagegatge
    }

    public void ChangeToColorTheme(ColorTheme newTheme, string nameOfTheme) {
        currentTheme = newTheme;
        PlayerPrefs.SetString(THEME_MODE, nameOfTheme + "Theme");

        UpdateSignResourceStrgColors(); // First update colors because some delegate listeners use it for simplicity
        ThemeChangeEvent(newTheme, changeDuration);// Call delaegateategateggasdasdsfeewedscxycasaatagegatge
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
