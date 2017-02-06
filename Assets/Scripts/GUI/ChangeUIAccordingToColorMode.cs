
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;

public class ChangeUIAccordingToColorMode : MonoBehaviour {

    // These are the default ones if an ovject happen to not have an updateColorMode
    public Color lightModeColor = Color.white;
    public Color darkModeColor = new Color(0.14902f, 0.19608f, 0.219608f);

    /// <summary>
    /// Which images we want to update according to color mode
    /// Eery object has to have a DarkLightColor script
    /// You can add stuf to this as well it will change them too
    /// </summary>
    [SerializeField]
    protected List<Image> updateToColorModesImg = new List<Image>();

    [SerializeField]
    protected List<Text> updateToColorModesTxt = new List<Text>();

    void Start() {
        // Add every image that has a darklightcolor component
        foreach (Image img in FindObjectsOfType<Image>()) {
            if (img.GetComponent<DarkLightColor>() != null && !updateToColorModesImg.Contains(img)) {
                updateToColorModesImg.Add(img);
            }
        }

        // Add every text that has a darklightcolor component
        foreach (Text txt in FindObjectsOfType<Text>()) {
            if (txt.GetComponent<DarkLightColor>() != null && !updateToColorModesTxt.Contains(txt)) {
                updateToColorModesTxt.Add(txt);
            }
        }

        ChangeToMode(FindObjectOfType<PreferencesScript>().currentMode, 0);
        PreferencesScript.ColorChangeEvent += ChangeToMode;
    }

    void OnDestroy() {
        PreferencesScript.ColorChangeEvent -= ChangeToMode;
    }

    /// <summary>
    /// Changes everything that we told it to to light mode
    /// </summary>
    public void ChangeToLightMode(float time) {
        foreach (Image image in updateToColorModesImg) {
            DarkLightColor dlc = image.GetComponent<DarkLightColor>();

            if (dlc != null) {
                image.DOColor(dlc.lightModeColor, time);
            } else { 
                image.DOColor(lightModeColor, time);
            }
        }

        foreach (Text txt in updateToColorModesTxt) {
            DarkLightColor dlc = txt.GetComponent<DarkLightColor>();

            // text is exactly opposite color
            if (dlc != null) {
                txt.DOColor(dlc.darkModeColor, time);
            } else {
                txt.DOColor(darkModeColor, time);
            }
        }
    }
    /// <summary>
    /// Changes everything that we told it to to dark mode
    /// </summary>
    public void ChangeToDarkMode(float time) {
        foreach (Image image in updateToColorModesImg) {
            DarkLightColor dlc = image.GetComponent<DarkLightColor>();

            if (dlc != null) {
                image.DOColor(dlc.darkModeColor, time);
            } else {
                image.DOColor(darkModeColor, time);
            }
        }

        foreach (Text txt in updateToColorModesTxt) {
            DarkLightColor dlc = txt.GetComponent<DarkLightColor>();

            // text is exactly opposite color
            if (dlc != null) {
                txt.DOColor(dlc.lightModeColor, time);
            } else {
                txt.DOColor(lightModeColor, time);
            }
        }
    }

    /// <summary>
    /// Chanes everything we told it to to tha mode we give it
    /// </summary>
    public void ChangeToMode(PreferencesScript.ColorMode colorMode, float time) {
        switch (colorMode) {
            case PreferencesScript.ColorMode.DARK: ChangeToDarkMode(time); break;
            case PreferencesScript.ColorMode.LIGHT: ChangeToLightMode(time); break;
        }
    }
}
