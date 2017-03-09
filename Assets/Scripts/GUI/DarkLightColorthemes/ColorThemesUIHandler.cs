using UnityEngine;
using UnityEngine.UI;

public class ColorThemesUIHandler : MonoBehaviour {

    private Color defaultButtonColor = new Color(0.22745f, 0.30588f, 0.3451f);

    void Awake() {
        // If we haven't completed tutorial just disable this because it would be too much for the player
        if (!PreferencesScript.Instance.IsTutorialCompleted())
            gameObject.SetActive(false);
    }
 
    void Start() {
        ResetColorsOfButtons();
        
        transform.FindChild(PreferencesScript.Instance.currentTheme.themeName).GetComponent<SetColorThemeUI>().SetClickedColor();
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
