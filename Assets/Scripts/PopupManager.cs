using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;

public class PopupManager : Singleton<PopupManager> {
    
    private static GameObject popupPrefab;
    private static GameObject popupInstance;

    private static Image popupInstanceImage;
    private static Button popupInstanceButton;
    private static Text popupInstanceText;

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
    /// Create new popup
    /// </summary>
    public void PopUp(string text, string buttonText) {
        if (!popupOut) return;
        popupOut = false;

        // Only instantiate new popup if the old one is destroyed
        if (popupInstance == null) { 
            popupInstance = Instantiate(popupPrefab, GameObject.Find("Canvas").transform, false);

            // Get different parts of popup
            popupInstanceImage = popupInstance.GetComponent<Image>();
            popupInstanceText = popupInstance.transform.GetChild(0).GetComponent<Text>();
            popupInstanceButton = popupInstance.transform.GetChild(1).GetComponent<Button>();

            // Set event of popup button
            popupInstanceButton.onClick.AddListener(() => {
                popupInstanceImage.rectTransform.DOScale(0, 0.4f).OnComplete(() => {
                    popupOut = true;
                });
                popupInstanceButton.enabled = false;
            });
        }

        // Enable button
        popupInstanceButton.enabled = true;

        // Set starting position of popup
        popupInstance.GetComponent<RectTransform>().anchoredPosition = new Vector2(Camera.main.pixelWidth / 2f, Camera.main.pixelHeight / 2f);
        
        // Set text for button
        popupInstanceButton.transform.GetChild(0).GetComponent<Text>().text = buttonText;

        // Set text
        popupInstanceText.text = text;

        popupInstance.SetActive(true);

        // Start animation
        popupInstanceImage.rectTransform.localScale = new Vector3();
        popupInstanceImage.rectTransform.DOScale(1f, 0.4f).SetEase(Ease.OutBack);
    }

    /// <summary>
    /// Makes popup
    /// After button is pressed and animation is played the callback happens
    /// </summary>
    public void PopUp(string text, string buttonText, UnityAction action) {
        if (!popupOut) return;

        PopUp(text, buttonText);
        popupInstanceButton.onClick.AddListener(() => {
            action.Invoke();
        });
    }
	
}
