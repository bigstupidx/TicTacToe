
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using TMPro;

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

    [SerializeField]
    protected List<TextMeshProUGUI> updateToColorTextMeshPro = new List<TextMeshProUGUI>();

    void Start() { // Thee can only be done in start
        // Add every image that has a darklightcolor component
        foreach (Image img in FindObjectsOfType<Image>()) {
            if (!updateToColorModesImg.Contains(img) && img.GetComponent<DarkLightColor>() != null) {
                updateToColorModesImg.Add(img);
            }
        }

        // Add every text that has a darklightcolor component
        foreach (Text txt in FindObjectsOfType<Text>()) {
            if (!updateToColorModesTxt.Contains(txt) && txt.GetComponent<DarkLightColor>() != null) {
                updateToColorModesTxt.Add(txt);
            }
        }

        foreach (TextMeshProUGUI text in FindObjectsOfType<TextMeshProUGUI>()) {
            if (!updateToColorTextMeshPro.Contains(text) && text.GetComponent<DarkLightColor>() != null) {
                updateToColorTextMeshPro.Add(text);
            }
        }

        ChangeToMode(PreferencesScript.Instance.currentMode, 0);
        PreferencesScript.ColorChangeEvent += ChangeToMode;
    }

    void OnDestroy() {
        PreferencesScript.ColorChangeEvent -= ChangeToMode;
    }

    /// <summary>
    /// Adds the possible component from the gamobject to the proper lists
    /// </summary>
    public void AddGameObject(GameObject obj) {
        Image img = obj.GetComponent<Image>();
        Text txt = obj.GetComponent<Text>();
        TextMeshProUGUI txtMP = obj.GetComponent<TextMeshProUGUI>();

        if (img != null) updateToColorModesImg.Add(img);
        else if (txt != null) updateToColorModesTxt.Add(txt);
        else if (txtMP != null) updateToColorTextMeshPro.Add(txtMP);
    }

    /// <summary>
    /// Simply ads it to the correct list but does not check whether it is in that list
    /// </summary>
    /// <param name="obj"></param>
    public void AddObject(object obj) {
        if (obj is Image) updateToColorModesImg.Add((Image) obj);
        else if (obj is Text) updateToColorModesTxt.Add((Text) obj);
        else if (obj is TextMeshProUGUI) updateToColorTextMeshPro.Add((TextMeshProUGUI) obj);
    }

    /// <summary>
    /// Changes everything that we told it to to light mode
    /// </summary>
    public void ChangeToLightMode(float time) {
        for (int i = updateToColorModesImg.Count - 1; i >= 0; i--) {
            if (updateToColorModesImg[i] == null) {
                updateToColorModesImg.RemoveAt(i);
                continue;
            }

            DarkLightColor dlc = updateToColorModesImg[i].GetComponent<DarkLightColor>();

            if (dlc != null) {
                updateToColorModesImg[i].DOColor(dlc.lightModeColor, time);
            } else {
                updateToColorModesImg[i].DOColor(lightModeColor, time);
            }
        }

        for (int i = updateToColorModesTxt.Count - 1; i >= 0; i--) {
            if (updateToColorModesTxt[i] == null) {
                updateToColorModesTxt.RemoveAt(i);
                continue;
            }

            DarkLightColor dlc = updateToColorModesTxt[i].GetComponent<DarkLightColor>();

            // text is exactly opposite color
            if (dlc != null) {
                updateToColorModesTxt[i].DOColor(dlc.lightModeColor, time);
            } else {
                updateToColorModesTxt[i].DOColor(lightModeColor, time);
            }
        }

        for (int i = updateToColorTextMeshPro.Count - 1; i >= 0; i--) {
            if (updateToColorTextMeshPro[i] == null) {
                updateToColorTextMeshPro.RemoveAt(i);
                continue;
            }

            DarkLightColor dlc = updateToColorTextMeshPro[i].GetComponent<DarkLightColor>();

            // text is exactly opposite color
            if (dlc != null) {
                updateToColorTextMeshPro[i].DOColor(dlc.lightModeColor, time);
            } else {
                updateToColorTextMeshPro[i].DOColor(lightModeColor, time);
            }
        }
    }
    /// <summary>
    /// Changes everything that we told it to to dark mode
    /// </summary>
    public void ChangeToDarkMode(float time) {
        for (int i = updateToColorModesImg.Count - 1; i >= 0; i--) {
            if (updateToColorModesImg[i] == null) {
                updateToColorModesImg.RemoveAt(i);
                continue;
            }

            DarkLightColor dlc = updateToColorModesImg[i].GetComponent<DarkLightColor>();

            if (dlc != null) {
                updateToColorModesImg[i].DOColor(dlc.darkModeColor, time);
            } else {
                updateToColorModesImg[i].DOColor(darkModeColor, time);
            }
        }

        for (int i = updateToColorModesTxt.Count - 1; i >= 0; i--) {
            if (updateToColorModesTxt[i] == null) {
                updateToColorModesTxt.RemoveAt(i);
                continue;
            }

            DarkLightColor dlc = updateToColorModesTxt[i].GetComponent<DarkLightColor>();

            // text is exactly opposite color
            if (dlc != null) {
                updateToColorModesTxt[i].DOColor(dlc.darkModeColor, time);
            } else {
                updateToColorModesTxt[i].DOColor(darkModeColor, time);
            }
        }

        for (int i = updateToColorTextMeshPro.Count - 1; i >= 0; i--) {
            if (updateToColorTextMeshPro[i] == null) {
                updateToColorTextMeshPro.RemoveAt(i);
                continue;
            }

            DarkLightColor dlc = updateToColorTextMeshPro[i].GetComponent<DarkLightColor>();

            // text is exactly opposite color
            if (dlc != null) {
                updateToColorTextMeshPro[i].DOColor(dlc.darkModeColor, time);
            } else {
                updateToColorTextMeshPro[i].DOColor(darkModeColor, time);
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
