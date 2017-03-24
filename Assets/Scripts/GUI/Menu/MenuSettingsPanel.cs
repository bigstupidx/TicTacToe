using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class MenuSettingsPanel : MonoBehaviour {

    [SerializeField]
    private Button settingsButton;
    [SerializeField]
    private RectTransform mainUIPanel;
    private EventTrigger mainUIPanelEventTrigger;
    [SerializeField]
    private Image mainUIPanelImage;
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
    private int currentChangePanel = -1;
    
    private RectTransform rectTransform;
    private float animTime = 0.3f;
    private bool isOpen = false;
    private int backButtonActionId;

    private Tweener currentMainUITweener;
    private Tweener currentRectTransformTweener;

    void Awake() {
        if (FindObjectsOfType<MenuSettingsPanel>().Length > 1) return;
        // add ids
        for (int i = 0; i < changePanels.Length; i++) changePanels[i].id = i;

        // These need to be in awake, because of the volumes (the text is set in start)

        rectTransform = GetComponent<RectTransform>();

        // Set volumes
        soundVolumeSlider.value = PreferencesScript.Instance.GetSoundVolume();
        musicVolumeSlider.value = PreferencesScript.Instance.GetMusicVolume();

        soundVolumeSlider.onValueChanged.AddListener((float value) => { PreferencesScript.Instance.SetSoundVolume((int) value); });
        musicVolumeSlider.onValueChanged.AddListener((float value) => { PreferencesScript.Instance.SetMusicVolume((int) value); });

        ScaneManager.OnScreenAboutToChangeEvent += OnSceneAboutToChange;
        ScaneManager.OnScreenChange += OnScreenChanged;
        
        OnScreenChanged("", "Menu");
    }

    private void OnSceneAboutToChange(string from, string to) {
        CloseWithoutTweening();
    }

    private void OnScreenChanged(string from, string to) {
        // Assign all variables
        mainUIPanel = FindObjectOfType<MainUIPanel>().GetComponent<RectTransform>();
        mainUIPanelImage = mainUIPanel.GetComponent<Image>();
        mainUIPanelImage.raycastTarget = false;

        // add callback to main ui panel, so when we click on it it close the settingspanel
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((eventData) => {
            CloseSettingsPanel();
        });
        mainUIPanelEventTrigger = mainUIPanel.GetComponent<EventTrigger>();
        if (mainUIPanelEventTrigger == null) // We don't have an eventtrigger on it so add one
            mainUIPanelEventTrigger = mainUIPanel.gameObject.AddComponent<EventTrigger>();
        mainUIPanelEventTrigger.triggers.Add(entry);
        mainUIPanelEventTrigger.enabled = false;


        bool panelInstantiated = false;
        for (int i = 0; i < changePanels.Length; i++) {
            if (changePanels[i].screenName == to) {
                // We already have the one we needed to instantiated
                if (currentChangePanel == i) break;

                InstantiateChangePanel(changePanels[i]);
                panelInstantiated = true;
                break;
            }
        }

        // If we found no screens in changePanels instantiate the very first element
        if (!panelInstantiated) { // We already have it instatiated
            InstantiateChangePanel(changePanels[0]);
        }

        SettingsButton sB = FindObjectOfType<SettingsButton>();
        if (sB != null) { // We have no button -> no setup of button needed
            settingsButton = sB.GetComponent<Button>();
            settingsButton.onClick.AddListener(() => {
                ToggleSettingsPanel();
            });
        }
    }

    private void InstantiateChangePanel(SettingsChangePanel changePanel) {
        if (screenChangePanel.childCount > 0) Destroy(screenChangePanel.GetChild(0).gameObject);

        GameObject obj = Instantiate(changePanel.prefab, screenChangePanel.transform, false);
        currentChangePanel = changePanel.id;
        obj.name = changePanel.screenName + "ChangePanel";

        ChangeUIAccordingToColorMode cuiatcc = FindObjectOfType<ChangeUIAccordingToColorMode>();
        foreach (DarkLightColor c in obj.transform.GetComponentsInChildren<DarkLightColor>()) { 
            c.SetColorToCurrentColorMode();
            cuiatcc.AddGameObject(c.gameObject);
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

        if (mainUIPanel != null) currentMainUITweener = mainUIPanel.DOAnchorPosX(mainUIPanel.anchoredPosition.x - rectTransform.rect.width, animTime);
        currentRectTransformTweener = rectTransform.DOAnchorPosX(rectTransform.anchoredPosition.x - rectTransform.rect.width, animTime).OnComplete(() => {
            // add to backbutton callback
            backButtonActionId = ScaneManager.Instance.AddToBackStack(() => { CloseSettingsPanel(); });
        });

        mainUIPanelImage.raycastTarget = true;
        mainUIPanelEventTrigger.enabled = true;
    }

    /// <summary>
    /// Close te settings panel, it won't close if it is already closed
    /// </summary>
    public void CloseSettingsPanel() {
        if (!isOpen) return;

        isOpen = false;

        if (mainUIPanel != null) currentMainUITweener = mainUIPanel.DOAnchorPosX(mainUIPanel.anchoredPosition.x + rectTransform.rect.width, animTime);
        currentRectTransformTweener = rectTransform.DOAnchorPosX(rectTransform.anchoredPosition.x + rectTransform.rect.width, animTime);

        mainUIPanelImage.raycastTarget = false;
        mainUIPanelEventTrigger.enabled = false;

        ScaneManager.Instance.RemoveFromBackStack(backButtonActionId);
    }

    /// <summary>
    /// Close settings panel without tweening. Checks whether it is open or not
    /// </summary>
    private void CloseWithoutTweening() {
        if (!isOpen) return;

        isOpen = false;

        mainUIPanel.anchoredPosition = new Vector2(mainUIPanel.anchoredPosition.x + rectTransform.rect.width, mainUIPanel.anchoredPosition.y);
        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x + rectTransform.rect.width, rectTransform.anchoredPosition.y);
        
        mainUIPanelImage.raycastTarget = false;
        mainUIPanelEventTrigger.enabled = false;
    }
}

[Serializable]
internal struct SettingsChangePanel {
    public string screenName;
    public GameObject prefab;

    [HideInInspector]
    public int id;
}