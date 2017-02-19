using UnityEngine;
using System;

public class PreferencesScript : MonoBehaviour {

    private const string FIRST_USE = "FirstUse";

    private void Awake() {

        DontDestroyOnLoad(gameObject);

        // If first use
        if (PlayerPrefs.GetString(FIRST_USE) == "IMDEADINSIDE") {
            PlayerPrefs.SetString(COLOR_MODE, ColorMode.LIGHT.ToString());
            PlayerPrefs.SetString(THEME_MODE, "DefaultTheme");
            PlayerPrefs.SetString(EMOJI_NAME + "0", "smilingEmoji");
            PlayerPrefs.SetString(EMOJI_NAME + "1", "angryEmoji");
            PlayerPrefs.SetString(EMOJI_NAME + "2", "fistBumpEmoji");
            PlayerPrefs.SetString(EMOJI_NAME + "3", "thinkingEmoji");
            PlayerPrefs.SetString(FIRST_USE, "IMDEADINSIDEPLSHELPME");
        }

        // Color mode
        currentMode = (ColorMode) Enum.Parse(typeof(ColorMode), PlayerPrefs.GetString(COLOR_MODE));
        currentTheme = ColorThemes.GetTheme(PlayerPrefs.GetString(THEME_MODE));
        UpdateSignResourceStrgColors();
    }

    // _________________________Emojis_______________________________________________

    /// <summary>
    /// There are 4 emojis which can be chosen so after this you need to put 0...3
    /// </summary>
    private const string EMOJI_NAME = "EmojiName";

    public readonly int EMOJI_COUNT = 4;

    public string[] GetEmojiNames() {
        string[] s = new string[EMOJI_COUNT];
        for (int i = 0; i < s.Length; i++)
            s[i] = PlayerPrefs.GetString(EMOJI_NAME + i);

        return s;
    }

    public Sprite[] GetEmojiSprites() {
        Sprite[] s = new Sprite[EMOJI_COUNT];
        for (int i = 0; i < s.Length; i++)
            s[i] = EmojiSprites.GetEmoji(PlayerPrefs.GetString(EMOJI_NAME + i));

        return s;
    }

    public string GetEmojiNameInSlot(int slot) {
        return PlayerPrefs.GetString(EMOJI_NAME + slot);
    }

    public Sprite GetEmojiSpriteInSlot(int slot) {
        return EmojiSprites.GetEmoji(PlayerPrefs.GetString(EMOJI_NAME + slot));
    }

    public void SetEmojiInSlotTo(int slot, string name) {
        PlayerPrefs.SetString(EMOJI_NAME + slot, name);
        PlayerPrefs.Save();
    }


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
    public ColorTheme currentTheme;

    /// <summary>
    /// Delegate used for theme changes
    /// </summary>
    public delegate void OnThemeChange(ColorTheme newTheme, float time);
    /// <summary>
    /// Subscribe to get notification when theme changes
    /// When we subscribe to this we can be sure that the color in SignResourceScript has already been changed
    /// </summary>
    public static OnThemeChange ThemeChangeEvent;



    public void ChangeToColorMode(ColorMode mode) {
        currentMode = mode;
        PlayerPrefs.SetString(COLOR_MODE, currentMode.ToString());
        PlayerPrefs.Save();

        UpdateSignResourceStrgColors(); // First update colors because some delegate listeners use it for simplicity
        ColorChangeEvent(mode, changeDuration); // Call delaegateategateggatagegatge
    }

    public void ChangeToColorTheme(ColorTheme newTheme, string nameOfTheme) {
        currentTheme = newTheme;
        PlayerPrefs.SetString(THEME_MODE, nameOfTheme + "Theme");
        PlayerPrefs.Save();

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

        public string themeName;

        public ColorTheme(Color xColorLight, Color oColorLight, Color xColorDark, Color oColorDark, string themeName) {
            this.xColorDark = xColorDark;
            this.oColorDark = oColorDark;
            this.xColorLight = xColorLight;
            this.oColorLight = oColorLight;
            this.themeName = themeName;
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
