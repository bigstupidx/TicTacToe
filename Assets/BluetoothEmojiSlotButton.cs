using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class BluetoothEmojiSlotButton : MonoBehaviour {

    public float animationTime = 0.15f;

    /// <summary>
    /// Which slit this emojibutton is assigned to
    /// </summary>
    [SerializeField]
    private int slot = 0;
    private bool toggled = false;
    public bool IsToggled { get { return toggled; } }
    public void Toggle() {
        if (toggled) toggled = false;
        else toggled = true;
    }
    public void ToggleToFalse() {
        toggled = false;
    }
    public void ToggleToTrue() {
        toggled = true;
    }

    private EmojiPickPanelScript pickPanel;
    private EmojiPickPanelContentScript pickContentPanel;
    private EmojiSlotButtonGroupScript emojiButtonGroup;

    private RectTransform rectTransform;
    private Image img;
    
	void Start () {
        pickPanel = FindObjectOfType<EmojiPickPanelScript>();
        pickContentPanel = pickPanel.GetComponentInChildren<EmojiPickPanelContentScript>();
        emojiButtonGroup = transform.parent.GetComponent<EmojiSlotButtonGroupScript>();
        rectTransform = GetComponent<RectTransform>();
        img = transform.GetChild(0).GetComponent<Image>();

        // Set current image
        img.sprite = FindObjectOfType<PreferencesScript>().GetEmojiSpriteInSlot(slot);

        // Subscribe to choose event
        pickContentPanel.OnEmojiChosen += SetImageTo;

        // On click event
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerUp;
        entry.callback.AddListener((BaseEventData bed) => {
            // On click
            DOTween.Sequence()
                    .Append(rectTransform.DOScale(0.7f, animationTime))
                    .Append(rectTransform.DOScale(1f, animationTime))
                    .OnComplete(new TweenCallback(() => {
                        // (toggled) means   if this button has been pressed and because of this the pickpanel has been opened
                        if (toggled) {
                            pickPanel.Close();
                            ToggleToFalse();
                            emojiButtonGroup.MakeEveryButtonNormal();
                        } else { // Not toggled
                            // First set every button to untoggled, because only one can be and we want this button to be
                            emojiButtonGroup.SetEveryButtonToggleFalse();
                            ToggleToTrue();
                            // Set every button small but this one
                            emojiButtonGroup.MakeUntoggledButtonsSmall();

                            if (pickPanel.IsOpen) {
                                // We don't want to open it again just simply change which is selected
                                pickContentPanel.SetNumberEnabled(slot);
                            } else {
                                // panel is not open so we first want to open it then set which is selected
                                pickPanel.Open();
                                pickContentPanel.SetNumberEnabled(slot);
                            }
                        }
                    }));
        });
        GetComponent<EventTrigger>().triggers.Add(entry);
	}

    public void SetImageTo(Sprite sprite, int slot) {
        if (slot == this.slot)
            img.sprite = sprite;
    }
}
