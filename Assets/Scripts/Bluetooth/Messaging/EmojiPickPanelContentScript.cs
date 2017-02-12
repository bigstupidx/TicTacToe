using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// This is the logical class of emoji panel
/// The other one is the graphical
/// </summary>
public class EmojiPickPanelContentScript : MonoBehaviour {

    private readonly Color disabledColor = new Color(0.61961f, 0.61961f, 0.61961f);

    /// <summary>
    /// The real height of the panel in scrollrect
    /// </summary>
    private float realHeight;
    private float widthHeight = 200;
    private float imagePadding;

    private int rowCount;
    private int colCount;

    private GameObject messageOnDrawerPrefab;

    private PreferencesScript preferences;

    private RectTransform rect;
    private ScrollRect scrollRect;
    private GameObject currentlyEnabled;
    private int currentSlot = 0;

    /// <summary>
    /// Which emoji was chosen and to which slot it was assigned to
    /// </summary>
    public delegate void EmojiChosen(Sprite emoji, int number);
    public event EmojiChosen OnEmojiChosen;
    
	void Start () {
        rect = GetComponent<RectTransform>();
        messageOnDrawerPrefab = Resources.Load<GameObject>("Prefabs/Bluetooth/Messaging/MessageOnDrawer");
        scrollRect = transform.parent.GetComponent<ScrollRect>();
        preferences = FindObjectOfType<PreferencesScript>();

        // adding images to drawer fomr here

        colCount = (int) (rect.rect.width / widthHeight) - 1; // -1 so we can have some padding around the images
        rowCount = Mathf.CeilToInt((float) EmojiSprites.emojiPaths.Length / colCount);

        realHeight = (rowCount + 1) * (widthHeight + imagePadding) + imagePadding;

        rect.sizeDelta = new Vector2(rect.rect.width, realHeight);

        imagePadding = (rect.rect.width - (widthHeight * colCount)) / (colCount + 1); // plus one so we can have pad on the most left side as well

        int at = 0;
        float xPos;
        float yPos = -imagePadding;
        // Going from left to right, top to bottom
        for (int i = 0; i < rowCount && at < EmojiSprites.emojiPaths.Length; i++) {
            xPos = imagePadding;
            for (int k = 0; k < colCount && at < EmojiSprites.emojiPaths.Length; k++) {
                // making gameobject
                GameObject msg = Instantiate(messageOnDrawerPrefab, transform, false);
                msg.name = EmojiSprites.emojiPaths[at];

                // Setting size and pos
                RectTransform msgrect = msg.GetComponent<RectTransform>();
                msgrect.sizeDelta = new Vector2(widthHeight, widthHeight);
                msgrect.anchoredPosition = new Vector2(xPos, yPos);

                // Setting sprite
                Image img = msg.transform.GetChild(1).GetComponent<Image>();
                img.sprite = EmojiSprites.GetEmoji(EmojiSprites.emojiPaths[at]);
                img.color = disabledColor;


                // Incrementing variables
                at++;
                xPos += (widthHeight + imagePadding);
            }

            yPos -= (widthHeight + imagePadding);
        }

        // Set it to top
        scrollRect.verticalNormalizedPosition = 0.95f;
	}

    /// <summary>
    /// Sets the numberth emoji to be enabled and when something is chosen from the emojipanel it will assign that to preferences
    /// </summary>
    public void SetNumberEnabled(int number) {
        currentSlot = number;

        SetEnabled(preferences.GetEmojiNameInSlot(number));
    }

    /// <summary>
    /// Just enables the image name corresponds to, but be aware that when emoji is clicked you won't know which slot that will be assigned to
    /// </summary>
    public void SetEnabled(string name) {
        if (currentlyEnabled != null) { 
            currentlyEnabled.GetComponent<RectTransform>().DOScale(1, 0.3f);
            currentlyEnabled.GetComponent<Image>().color = disabledColor;
            currentlyEnabled.transform.parent.GetComponent<EmojiButtonScript>().isEnabled = false;
        }

        currentlyEnabled = transform.FindChild(name).GetChild(1).gameObject;

        currentlyEnabled.GetComponent<RectTransform>().DOScale(1.2f, 0.3f);
        currentlyEnabled.GetComponent<Image>().color = Color.white;
        currentlyEnabled.transform.parent.GetComponent<EmojiButtonScript>().isEnabled = true;

        // Set perferences
        preferences.SetEmojiInSlotTo(currentSlot, currentlyEnabled.transform.parent.name);
        
        DOTween.To(() => scrollRect.verticalNormalizedPosition, (float value) => scrollRect.verticalNormalizedPosition = value, 1 - (Mathf.Abs(currentlyEnabled.transform.parent.gameObject.GetComponent<RectTransform>().anchoredPosition.y) / realHeight), 0.2f);
    }

    /// <summary>
    /// Called by children when they were pressed
    /// </summary>
    public void ButtonPressed(string name) {
        if (OnEmojiChosen != null)
            OnEmojiChosen(EmojiSprites.GetEmoji(name), currentSlot);

        SetEnabled(name);
    }
}
