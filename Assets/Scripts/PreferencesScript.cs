using UnityEngine;
using System;
using DG.Tweening;

public class PreferencesScript : MonoBehaviour {

    private const string FIRST_USE = "FirstUse";


    // Color mode variables

    private const string COLOR_MODE = "ColorMode";

    public ColorMode currentMode;

    /// <summary>
    /// How long tha changing animation should take
    /// </summary>
    private const float changeDuration = 0.5f;
    


    private void Start() {

        DontDestroyOnLoad(gameObject);

        // If first use
        if (PlayerPrefs.GetString(FIRST_USE) == "") {
            PlayerPrefs.SetString(COLOR_MODE, ColorMode.LIGHT.ToString());
            PlayerPrefs.SetString(FIRST_USE, "IMBEINUSEDJUSTLIKEINREALLIFE");
        }
        
        // Color mode
        currentMode = (ColorMode) Enum.Parse(typeof(ColorMode), PlayerPrefs.GetString(COLOR_MODE));
    }



    public void ChangeToColorMode(ColorMode mode) {
        currentMode = mode;
        PlayerPrefs.SetString(COLOR_MODE, currentMode.ToString());

        // In order: 
        // Change camera
        // Change grid
        // Change UI
        switch (mode) {
            case ColorMode.DARK:
                Camera.main.GetComponent<CamColorModeToPrefs>().ToDarkMode(changeDuration);
                FindObjectOfType<GridLightDarkMode>().ToDarkMode(changeDuration);
                FindObjectOfType<ChangeUIAccordingToColorMode>().ChangeToDarkMode(changeDuration);
                break;
            case ColorMode.LIGHT:
                Camera.main.GetComponent<CamColorModeToPrefs>().ToLightMode(changeDuration);
                FindObjectOfType<GridLightDarkMode>().ToLightMode(changeDuration);
                FindObjectOfType<ChangeUIAccordingToColorMode>().ChangeToLightMode(changeDuration);
                break;
            default:
                Debug.Log("PROBLEMO");
                break;
        }
    }
	
    public enum ColorMode {
        DARK, LIGHT
    }

}
