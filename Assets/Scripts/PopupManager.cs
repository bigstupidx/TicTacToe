using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;

public class PopupManager : MonoBehaviour {

    private static float timePerHalfScreen = .5f;
    
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
    public static void PopUp(string text, string buttonText) {
        if (!popupOut) return;
        popupOut = false;

        // Only instantiate new popup if the old one is destroyed
        if (popupInstance == null) { 
            popupInstance = Instantiate(popupPrefab);

            // Get different parts of popup
            popupInstanceImage = popupInstance.GetComponent<Image>();
            popupInstanceText = popupInstance.transform.GetChild(0).GetComponent<Text>();
            popupInstanceButton = popupInstance.transform.GetChild(1).GetComponent<Button>();

            // Set the parent of popuppanel to canvas of the current scene WHICH HAS TO BE CALLED CANVAS!!!!
            popupInstance.transform.SetParent(GameObject.Find("Canvas").transform, false);

            // Set event of popup button
            popupInstanceButton.onClick.AddListener(() => {
                popupInstance.transform.DOMoveY(Camera.main.pixelHeight + popupInstanceImage.rectTransform.rect.height / 2f, timePerHalfScreen).OnComplete(() => {
                    popupOut = true;
                });
                popupInstanceButton.enabled = false;
            });
        }

        // Enable button
        popupInstanceButton.enabled = true;

        // Set starting position of popup
        popupInstance.transform.position = new Vector2(Camera.main.pixelWidth / 2f, -popupInstanceImage.rectTransform.rect.height / 2f);
        
        // Set text for button
        popupInstanceButton.transform.GetChild(0).GetComponent<Text>().text = buttonText;

        // Set text
        popupInstanceText.text = text;

        popupInstance.SetActive(true);

        // Start animation
        popupInstance.transform.DOMoveY(Camera.main.pixelHeight / 2f, timePerHalfScreen);
    }

    /// <summary>
    /// Makes popup
    /// After button is pressed and animation is played the callback happens
    /// </summary>
    public static void PopUp(string text, string buttonText, UnityAction action) {
        if (!popupOut) return;

        PopUp(text, buttonText);
        popupInstanceButton.onClick.AddListener(() => {
            action.Invoke();
        });
    }
	
}
