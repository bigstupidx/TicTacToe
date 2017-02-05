using UnityEngine;

/// <summary>
/// First child should be x and the second should be o color
/// </summary>
public class SetColorThemeUI : MonoBehaviour {

    public string colorThemeName;

    private PreferencesScript.ColorTheme currentTheme;
    
	void Awake () {
        // We need to to this in awake because in start which lightdark to use will be set

        // Get the theme
        currentTheme = ColorThemes.GetTheme(colorThemeName + "Theme");

        // Set all children's color which nee dto be set
        DarkLightColor xDL = transform.GetChild(0).GetComponent<DarkLightColor>();
        DarkLightColor oDL = transform.GetChild(1).GetComponent<DarkLightColor>();

        xDL.lightModeColor = currentTheme.xColorLight;
        xDL.darkModeColor = currentTheme.xColorDark;

        oDL.lightModeColor = currentTheme.oColorLight;
        oDL.darkModeColor = currentTheme.oColorDark;
    }
}
