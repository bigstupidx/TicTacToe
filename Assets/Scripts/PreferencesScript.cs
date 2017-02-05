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

    /// <summary>
    /// Delegate used for color changes
    /// </summary>
    public delegate void OnColorChange(ColorMode mode, float time);
    public OnColorChange ColorChangeEvent;

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

        ColorChangeEvent(mode, changeDuration);
    }
	
    public enum ColorMode {
        DARK, LIGHT
    }

}
