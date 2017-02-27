using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;

public class PopupManager : Singleton<PopupManager> {
    
    private static GameObject popupPrefab;
    private static GameObject popupInstance;

    private static CanvasGroup popupInstanceGroup;
    private static Image popupInstanceImage;
    private static Text popupInstanceText;

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
    public void PopUp(string text, string buttonOne, string buttonTwo) {
        if (!popupOut) return;
        popupOut = false;

        InstantiatePopUp();

        // disable one button panel, cause this is the two button panel
        twoButtonPanel.SetActive(true);
        oneButtonPanel.SetActive(false);

        // Set text for buttons
        popupInstanceButtonTwo[0].GetComponentInChildren<Text>().text = buttonOne;
        popupInstanceButtonTwo[1].GetComponentInChildren<Text>().text = buttonTwo;

        // Set text
        popupInstanceText.text = text;

        StartPopUpAnimation();
    }

    /// <summary>
    /// Show popup with two buttons with the given button colors
    /// </summary>
    public void PopUp(string text, string buttonOne, string buttonTwo, Color colorOne, Color colorTwo) {
        PopUp(text, buttonOne, buttonTwo);

        popupInstanceButtonTwo[0].GetComponent<Image>().color = colorOne;
        popupInstanceButtonTwo[1].GetComponent<Image>().color = colorTwo;
    }

    /// <summary>
    /// Create new popup
    /// </summary>
    public void PopUp(string text, string buttonText) {
        if (!popupOut) return;
        popupOut = false;

        InstantiatePopUp();

        // disable two button panel, cause this is the one button panel
        oneButtonPanel.SetActive(true);
        twoButtonPanel.SetActive(false);

        // Set text for button
        popupInstanceButtonOne.GetComponentInChildren<Text>().text = buttonText;

        // Set text
        popupInstanceText.text = text;

        StartPopUpAnimation();
    }

    /// <summary>
    /// Show popup with one button with the given button color
    /// </summary>
    public void PopUp(string text, string buttonText, Color color) {
        PopUp(text, buttonText);

        popupInstanceButtonOne.GetComponent<Image>().color = color;
    }

    /// <summary>
    /// Makes popup
    /// After button is pressed the callback happens
    /// </summary>
    public void PopUp(string text, string buttonText, UnityAction action) {
        if (!popupOut) return;

        PopUp(text, buttonText);
        popupInstanceButtonOne.onClick.AddListener(() => {
            action.Invoke();
        });
    }

    /// <summary>
    /// Show popup with one button with the given button color
    /// </summary>
    public void PopUp(string text, string buttonText, Color color, UnityAction action) {
        PopUp(text, buttonText, action);
        
        popupInstanceButtonOne.GetComponent<Image>().color = color;
    }

    /// <summary>
    /// Makes popup with two buttons
    /// After button is pressed the callback happens
    /// </summary>
    public void PopUp(string text, string buttonOne, string buttonTwo, UnityAction action1, UnityAction action2) {
        if (!popupOut) return;

        PopUp(text, buttonOne, buttonTwo);
        popupInstanceButtonTwo[0].onClick.AddListener(() => {
            action1.Invoke();
        });
        popupInstanceButtonTwo[1].onClick.AddListener(() => {
            action2.Invoke();
        });
    }

    /// <summary>
    /// Show popup with two buttons with the given button colors
    /// </summary>
    public void PopUp(string text, string buttonOne, string buttonTwo, Color colorOne, Color colorTwo, UnityAction action1, UnityAction action2) {
        PopUp(text, buttonOne, buttonTwo, action1, action2);

        popupInstanceButtonTwo[0].GetComponent<Image>().color = colorOne;
        popupInstanceButtonTwo[1].GetComponent<Image>().color = colorTwo;
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
            popupInstance = Instantiate(popupPrefab, GameObject.Find("Canvas").transform, false);

            // Get different parts of popup
            popupInstanceGroup = popupInstance.GetComponent<CanvasGroup>();
            popupInstanceImage = popupInstance.transform.GetChild(0).GetComponent<Image>();
            popupInstanceText = popupInstanceImage.transform.GetChild(0).GetComponent<Text>();
            oneButtonPanel = popupInstanceImage.transform.GetChild(1).gameObject;
            twoButtonPanel = popupInstanceImage.transform.GetChild(2).gameObject;

            // Gut buttons
            popupInstanceButtonOne = oneButtonPanel.transform.GetChild(0).GetComponent<Button>();
            popupInstanceButtonTwo[0] = twoButtonPanel.transform.GetChild(0).GetComponent<Button>();
            popupInstanceButtonTwo[1] = twoButtonPanel.transform.GetChild(1).GetComponent<Button>();

            // Add fade out click actions
            UnityAction action = () => {
                popupInstanceGroup.DOFade(0f, 0.2f).OnComplete(() => {
                    popupOut = true;
                    popupInstanceGroup.interactable = false;
                    popupInstanceGroup.blocksRaycasts = false;
                });
                popupInstanceImage.rectTransform.DOScale(0f, 0.4f);
            };
            popupInstanceButtonOne.onClick.AddListener(action);
            popupInstanceButtonTwo[0].onClick.AddListener(action);
            popupInstanceButtonTwo[1].onClick.AddListener(action);
        }
    }

}
