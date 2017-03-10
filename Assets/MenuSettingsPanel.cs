using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MenuSettingsPanel : MonoBehaviour {

    public RectTransform mainMenuPanel;
    public Slider soundVolumeSlider;
    public Slider musicVolumeSlider;
    
    private RectTransform rectTransform;
    private float animTime = 0.3f;
    private bool isOpen = false;

	void Awake() {
        // These need to be in awake, because of the volumes (the text is set in start)

        rectTransform = GetComponent<RectTransform>();

        // Set volumes
        soundVolumeSlider.value = PreferencesScript.Instance.GetSoundVolume();
        musicVolumeSlider.value = PreferencesScript.Instance.GetMusicVolume();

        soundVolumeSlider.onValueChanged.AddListener((float value) => { PreferencesScript.Instance.SetSoundVolume((int)value); });
        musicVolumeSlider.onValueChanged.AddListener((float value) => { PreferencesScript.Instance.SetMusicVolume((int) value); });
    }

    void Update() {
        // close panel on back button on click of screen
        if (isOpen && ((Input.touchCount > 0 && !GridClickHandler.IsPointerOverUIObject()) || Input.GetKeyDown(KeyCode.Escape))) {
            CloseSettingsPanel();
        }
    }

    /// <summary>
    /// Toggle the settings panel
    /// </summary>
    public void ToggleSettingsPanel() {
        if (isOpen) {
            CloseSettingsPanel();
        } else {
            OpenSettingPanel();
        }
    }
	
    /// <summary>
    /// Open the sttings panel, it won't open if it is already open
    /// </summary>
	public void OpenSettingPanel() {
        if (isOpen) return;

        isOpen = true;

        mainMenuPanel.DOAnchorPosX(mainMenuPanel.anchoredPosition.x - rectTransform.rect.width, animTime);
        rectTransform.DOAnchorPosX(rectTransform.anchoredPosition.x - rectTransform.rect.width, animTime);
    }

    /// <summary>
    /// Close te settings panel, it won't close if it is already closed
    /// </summary>
    public void CloseSettingsPanel() {
        if (!isOpen) return;

        isOpen = false;

        mainMenuPanel.DOAnchorPosX(mainMenuPanel.anchoredPosition.x + rectTransform.rect.width, animTime);
        rectTransform.DOAnchorPosX(rectTransform.anchoredPosition.x + rectTransform.rect.width, animTime);
    }
}
