using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

/// <summary>
/// First child should be x and the second should be o color
/// </summary>
public class SetColorThemeUI : MonoBehaviour {

    private const float CLICK_ANIM_TIME = 0.3f;

    public string colorThemeName;
    private PreferencesScript.ColorTheme currentTheme;

    private RectTransform rectTransform;
    private Image image;
    private EventTrigger eventTrigger;

    private ColorThemesUIHandler colorThemesHandler;

    public Color clickedColor = new Color(0.32941f, 0.43137f, 0.47843f);

    void Awake () {
        // We need to do this in awake because in start which lightdark to use will be set

        // Get the theme
        currentTheme = ColorThemes.GetTheme(colorThemeName + "Theme");

        // Set all children's color which nee dto be set
        DarkLightColor xDL = transform.GetChild(0).GetComponent<DarkLightColor>();
        DarkLightColor oDL = transform.GetChild(1).GetComponent<DarkLightColor>();

        xDL.lightModeColor = currentTheme.xColorLight;
        xDL.darkModeColor = currentTheme.xColorDark;

        oDL.lightModeColor = currentTheme.oColorLight;
        oDL.darkModeColor = currentTheme.oColorDark;

        // Fields
        image = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        eventTrigger = gameObject.AddComponent<EventTrigger>();
        colorThemesHandler = transform.parent.GetComponent<ColorThemesUIHandler>();
    }

    void Start() {
        // Add trigger
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((eventData) => {
            // Reset all button's color first
            colorThemesHandler.ResetColorsOfButtons();
            // Set color theme
            PreferencesScript.Instance.ChangeToColorTheme(currentTheme, colorThemeName);
            DOTween.Sequence()
                .Append(rectTransform.DOScale(0.8f, CLICK_ANIM_TIME / 2f))
                .Append(rectTransform.DOScale(1f, CLICK_ANIM_TIME / 2f))
                .Insert(0, image.DOColor(clickedColor, CLICK_ANIM_TIME / 2f));
        });
        eventTrigger.triggers.Add(entry);
    }

    /// <summary>
    /// Sets this button's color to clickedColor
    /// </summary>
    public void SetClickedColor() {
        image.color = clickedColor;
    }
}
