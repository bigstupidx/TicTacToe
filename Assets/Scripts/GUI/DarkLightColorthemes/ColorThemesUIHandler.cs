using UnityEngine;
using UnityEngine.UI;

public class ColorThemesUIHandler : MonoBehaviour {

    private Color defaultButtonColor = new Color(0.14902f, 0.19608f, 0.21961f);

    private PreferencesScript preferences;

    void Start() {
        ResetColorsOfButtons();
        preferences = FindObjectOfType<PreferencesScript>();
        
        transform.FindChild(preferences.currentTheme.themeName).GetComponent<SetColorThemeUI>().SetClickedColor();
    }
    
    /// <summary>
    /// Resets the colors of all colortheme buttons
    /// </summary>
    public void ResetColorsOfButtons() {
        foreach (Transform child in transform) {
            child.GetComponent<Image>().color = defaultButtonColor;
        }
    }
	
}
