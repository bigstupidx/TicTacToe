using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;
using TMPro;

public class PopupManager : Singleton<PopupManager> {
    
    private static GameObject popupPrefab;
    private static GameObject popupInstance;

    private static CanvasGroup popupInstanceGroup;
    private static Image popupInstanceImage;
    private static TextMeshProUGUI popupInstanceText;

    private static Button popupInstanceButtonOne;
    private static Button[] popupInstanceButtonTwo = new Button[2];

    private static GameObject oneButtonPanel;
    private static GameObject twoButtonPanel;

    // Whether pop up is out of screen or not
    private static bool popupOut = true;
    
    void Awake() {
        DontDestroyOnLoad(gameObject);

        LoadResources();
    }

    private void LoadResources() {
        popupPrefab = Resources.Load<GameObject>("Prefabs/GUI/PopupPanel");
    }

    void OnApplicationPause(bool paused) {
        if (!paused) {
            LoadResources();
        }
    }

    /// <summary>
    /// Popup with two buttons
    /// </summary>
    public void PopUp(PopUpTwoButton attributes) {
        if (!popupOut) return;
        popupOut = false;

        InstantiatePopUp();

        // disable one button panel, cause this is the two button panel
        twoButtonPanel.SetActive(true);
        oneButtonPanel.SetActive(false);

        popupInstanceImage.color = attributes.backgroundColor;

        for (int i = 0; i < 2; i++) {
            Text text = popupInstanceButtonTwo[i].GetComponentInChildren<Text>();

            text.text = attributes.buttonText[i];
            text.color = attributes.buttonTextColor[i];

            popupInstanceButtonTwo[i].GetComponent<Image>().color = attributes.buttonColor[i];
            popupInstanceButtonTwo[i].onClick.AddListener(attributes.buttonPressed[i]);
        }

        // Set text
        popupInstanceText.text = attributes.text;
        popupInstanceText.color = attributes.textColor;

        StartPopUpAnimation();
    }

    /// <summary>
    /// Popup with one button
    /// </summary>
    public void PopUp(string text, string buttonText) {
        PopUp(new PopUpOneButton(text, buttonText));
    }

    /// <summary>
    /// Create new popup
    /// </summary>
    public void PopUp(PopUpOneButton attributes) {
        if (!popupOut) return;
        popupOut = false;

        InstantiatePopUp();

        // disable two button panel, cause this is the one button panel
        oneButtonPanel.SetActive(true);
        twoButtonPanel.SetActive(false);

        popupInstanceImage.color = attributes.backgroundColor;

        // Set text for button
        Text text = popupInstanceButtonOne.GetComponentInChildren<Text>();

        text.text = attributes.buttonText;
        text.color = attributes.buttonTextColor;

        popupInstanceButtonOne.GetComponent<Image>().color = attributes.buttonColor;
        popupInstanceButtonOne.onClick.AddListener(attributes.buttonPressed);

        // Set text
        popupInstanceText.text = attributes.text;
        popupInstanceText.color = attributes.textColor;

        StartPopUpAnimation();
    }

    /// <summary>
    /// Popup with two buttons
    /// </summary>
    public void PopUp(string text, string buttonOne, string buttonTwo) {
        PopUp(new PopUpTwoButton(text, buttonOne, buttonTwo));
    }

    private void StartPopUpAnimation() {
        // Start animation
        popupInstanceGroup.DOFade(1f, 0.2f).OnComplete(new TweenCallback(() => {
            popupInstanceGroup.interactable = true;
            popupInstanceGroup.blocksRaycasts = true;
        }));
        popupInstanceImage.rectTransform.localScale = new Vector3(0f, 0f, 0f);
        popupInstanceImage.rectTransform.DOScale(1f, 0.4f).SetEase(Ease.OutBack);
    }

    private void InstantiatePopUp() {
        // Only instantiate new popup if the old one is destroyed
        if (popupInstance == null) {
            popupInstance = Instantiate(popupPrefab, GameObject.Find("DontDestroyCanvas").transform, false);

            // Get different parts of popup
            popupInstanceGroup = popupInstance.GetComponent<CanvasGroup>();
            popupInstanceImage = popupInstance.transform.GetChild(0).GetComponent<Image>();
            popupInstanceText = popupInstanceImage.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            oneButtonPanel = popupInstanceImage.transform.GetChild(1).gameObject;
            twoButtonPanel = popupInstanceImage.transform.GetChild(2).gameObject;

            // Gut buttons
            popupInstanceButtonOne = oneButtonPanel.transform.GetChild(0).GetComponent<Button>();
            popupInstanceButtonTwo[0] = twoButtonPanel.transform.GetChild(0).GetComponent<Button>();
            popupInstanceButtonTwo[1] = twoButtonPanel.transform.GetChild(1).GetComponent<Button>();
        }


        popupInstanceButtonOne.onClick.RemoveAllListeners();
        popupInstanceButtonTwo[0].onClick.RemoveAllListeners();
        popupInstanceButtonTwo[1].onClick.RemoveAllListeners();

        // Add fade out click actions because we removed the listeners
        UnityAction action = () => {
            popupInstanceGroup.DOFade(0f, 0.2f).OnComplete(() => {
                popupInstanceGroup.interactable = false;
                popupInstanceGroup.blocksRaycasts = false;
            });
            popupInstanceImage.rectTransform.DOScale(0f, 0.4f);
            popupOut = true;
        };
        popupInstanceButtonOne.onClick.AddListener(action);
        popupInstanceButtonTwo[0].onClick.AddListener(action);
        popupInstanceButtonTwo[1].onClick.AddListener(action);
    }

}

public class PopUpOneButton {
    internal string text;
    internal string buttonText;
    internal Color buttonTextColor = new Color(0.14902f, 0.19608f, 0.21961f);
    internal Color buttonColor = new Color(0.87843f, 0.87843f, 0.87843f);
    internal Color textColor = PreferencesScript.Instance.currentMode == PreferencesScript.ColorMode.LIGHT ? new Color(0.14902f, 0.19608f, 0.21961f) : Color.white;
    internal Color backgroundColor = PreferencesScript.Instance.currentMode == PreferencesScript.ColorMode.LIGHT ? Color.white : new Color(0.14902f, 0.19608f, 0.21961f);
    internal UnityAction buttonPressed = () => { };

    public PopUpOneButton(string text, string buttonText) {
        this.text = text;
        this.buttonText = buttonText;
    }

    public PopUpOneButton Builder() { return this; }
    public PopUpOneButton SetButtonColor(Color buttonColor) {
        this.buttonColor = buttonColor;
        return this;
    }
    public PopUpOneButton SetButtonTextColor(Color buttonTextColor) {
        this.buttonTextColor = buttonTextColor;
        return this;
    }
    public PopUpOneButton SetBackgroundColor(Color backgroundColor) {
        this.backgroundColor = backgroundColor;
        return this;
    }
    public PopUpOneButton SetTextColor(Color textColor) {
        this.textColor = textColor;
        return this;
    }
    public PopUpOneButton SetButtonPressAction(UnityAction action) {
        this.buttonPressed = action;
        return this;
    }
}

public class PopUpTwoButton {
    internal string text;
    internal string[] buttonText = new string[2];
    internal Color[] buttonTextColor = new Color[] { new Color(0.14902f, 0.19608f, 0.21961f), new Color(0.14902f, 0.19608f, 0.21961f) };
    internal Color[] buttonColor = new Color[] { new Color(0.87843f, 0.87843f, 0.87843f), new Color(0.87843f, 0.87843f, 0.87843f) };
    internal Color textColor = PreferencesScript.Instance.currentMode == PreferencesScript.ColorMode.LIGHT ? new Color(0.14902f, 0.19608f, 0.21961f) : Color.white;
    internal Color backgroundColor = PreferencesScript.Instance.currentMode == PreferencesScript.ColorMode.LIGHT ? Color.white : new Color(0.14902f, 0.19608f, 0.21961f);
    internal UnityAction[] buttonPressed = new UnityAction[] { () => { }, () => { } };

    public PopUpTwoButton(string text, string buttonOne, string buttonTwo) {
        this.text = text;
        this.buttonText[0] = buttonOne;
        this.buttonText[1] = buttonTwo;
    }

    public PopUpTwoButton Builder() { return this; }
    public PopUpTwoButton SetButtonColors(Color buttonColorOne, Color buttonColorTwo) {
        this.buttonColor[0] = buttonColorOne;
        this.buttonColor[1] = buttonColorTwo;
        return this;
    }
    public PopUpTwoButton SetButtonTextColors(Color buttonTextColorOne, Color buttonTextColorTwo) {
        this.buttonTextColor[0] = buttonTextColorOne;
        this.buttonTextColor[1] = buttonTextColorTwo;
        return this;
    }
    public PopUpTwoButton SetBackgroundColor(Color backgroundColor) {
        this.backgroundColor = backgroundColor;
        return this;
    }
    public PopUpTwoButton SetTextColor(Color textColor) {
        this.textColor = textColor;
        return this;
    }
    public PopUpTwoButton SetButtonPressActions(UnityAction action1, UnityAction action2) {
        this.buttonPressed[0] = action1;
        this.buttonPressed[1] = action2;
        return this;
    }
}