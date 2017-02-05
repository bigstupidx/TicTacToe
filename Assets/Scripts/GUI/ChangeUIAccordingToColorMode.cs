
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ChangeUIAccordingToColorMode : MonoBehaviour {

    // These are the default ones if an ovject happen to not have an updateColorMode
    public Color lightModeColor = Color.white;
    public Color darkModeColor = new Color(0.14902f, 0.19608f, 0.219608f);

    /// <summary>
    /// Which images we want to update according to color mode
    /// Eery object need to have a DarkLightColor script otherwise it will use the default ones above
    /// </summary>
    [SerializeField]
    protected Image[] updateToColorModesImg;

    [SerializeField]
    protected Text[] updateToColorModesTxt;

    void Start() {
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
