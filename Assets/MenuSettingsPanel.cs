using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class MenuSettingsPanel : MonoBehaviour {

    [SerializeField]
    private Button settingsButton;
    [SerializeField]
    private RectTransform mainMenuPanel;
    [SerializeField]
    private Slider soundVolumeSlider;
    [SerializeField]
    private Slider musicVolumeSlider;

    /// <summary>
    /// Which panel to change when screen changes
    /// </summary>
    [SerializeField]
    private RectTransform screenChangePanel;
    [Tooltip("On what screens to change the screenChangePanel. If the screen is not in this array then the very first element will be added.")]
    [SerializeField]
    private SettingsChangePanel[] changePanels;
    private int currentChangePanel;
    
    private RectTransform rectTransform;
    private float animTime = 0.3f;
    private bool isOpen = false;

	void Awake() {
        if (FindObjectsOfType<MenuSettingsPanel>().Length > 1) return;
        // add ids
        for (int i = 0; i < changePanels.Length; i++) changePanels[i].id = i;

        // These need to be in awake, because of the volumes (the text is set in start)

        rectTransform = GetComponent<RectTransform>();

        // Set volumes
        soundVolumeSlider.value = PreferencesScript.Instance.GetSoundVolume();
        musicVolumeSlider.value = PreferencesScript.Instance.GetMusicVolume();

        soundVolumeSlider.onValueChanged.AddListener((float value) => { PreferencesScript.Instance.SetSoundVolume((int)value); });
        musicVolumeSlider.onValueChanged.AddListener((float value) => { PreferencesScript.Instance.SetMusicVolume((int) value); });

        // Instantiate the screen change for the menu screen
        InstantiateChangePanel(changePanels[1]);

        ScaneManager.OnScreenChange += OnScreenChanged;
    }

    private void OnScreenChanged(string from, string to) {
        SettingsButton sB = FindObjectOfType<SettingsButton>();
        if (sB == null) return; // We have no button -> no settings panel needed

        // Assign all variables
        mainMenuPanel = FindObjectOfType<MainUIPanel>().GetComponent<RectTransform>();

        settingsButton = sB.GetComponent<Button>();
        settingsButton.onClick.AddListener(() => {
            ToggleSettingsPanel();
        });

        for (int i = 0; i < changePanels.Length; i++) {
            if (changePanels[i].screenName == to) {
                // We already have the one we needed to instantiated
                if (currentChangePanel == i) return;

                InstantiateChangePanel(changePanels[i]);
                return;
            }
        }

        // If we found no screens in changePanels instantiate the very first element
        if (currentChangePanel == 0) return; // We already have it instatiated

        InstantiateChangePanel(changePanels[0]);
    }

    private void InstantiateChangePanel(SettingsChangePanel changePanel) {
        if (screenChangePanel.childCount > 0) Destroy(screenChangePanel.GetChild(0).gameObject);

        GameObject obj = Instantiate(changePanel.prefab, screenChangePanel.transform, false);
        currentChangePanel = changePanel.id;
        obj.name = changePanel.screenName + "ChangePanel";
        
        foreach (DarkLightColor c in obj.transform.GetComponentsInChildren<DarkLightColor>())
            c.SetColorToCurrentColorMode();
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

[Serializable]
internal struct SettingsChangePanel {
    public string screenName;
    public GameObject prefab;

    [HideInInspector]
    public int id;
}