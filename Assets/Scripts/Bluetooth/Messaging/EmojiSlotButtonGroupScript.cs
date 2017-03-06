using UnityEngine;
using DG.Tweening;

/// <summary>
/// Groups together the four emoji slot buttons
/// </summary>
public class EmojiSlotButtonGroupScript : MonoBehaviour {
    
    private BluetoothEmojiSlotButton[] buttons;
    private RectTransform[] buttonRects;

	void Start () {
        buttons = new BluetoothEmojiSlotButton[PreferencesScript.Instance.EMOJI_COUNT];
        buttonRects = new RectTransform[buttons.Length];

        for (int i = 0; i < buttons.Length; i++) {
            buttons[i] = transform.GetChild(i).GetComponent<BluetoothEmojiSlotButton>();
            buttonRects[i] = transform.GetChild(i).GetComponent<RectTransform>();
        }

        // because the childs are in order we need to disable the remainig one we don't use because they are not unlocked yet
        for (int i = buttons.Length; i < transform.childCount; i++) {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }
	
    /// <summary>
    /// Makes every child button a tad smaller
    /// </summary>
    public void MakeEveryButtonsSmall() {
        for (int i = 0; i < buttonRects.Length; i++)
            buttonRects[i].DOScale(0.7f, buttons[i].animationTime * 2f);
    }

    public void MakeUntoggledButtonsSmall() {
        // First we have to make every button normal
        MakeEveryButtonNormal();

        // Then we can mae every one small
        for (int i = 0; i < buttonRects.Length; i++)
            if (!buttons[i].IsToggled)
                buttonRects[i].DOScale(0.7f, buttons[i].animationTime * 2f);
    }

    public void MakeEveryButtonNormal() {
        for (int i = 0; i < buttonRects.Length; i++)
            buttonRects[i].DOScale(1, buttons[i].animationTime * 2f);
    }

    public void SetEveryButtonToggleFalse() {
        foreach (BluetoothEmojiSlotButton button in buttons)
            button.ToggleToFalse();
    }

    public void SetEveryButtonToggleTrue() {
        foreach (BluetoothEmojiSlotButton button in buttons)
            button.ToggleToTrue();
    }

}
