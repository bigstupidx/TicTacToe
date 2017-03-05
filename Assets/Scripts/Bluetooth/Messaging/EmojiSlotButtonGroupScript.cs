using UnityEngine;
using DG.Tweening;

/// <summary>
/// Groups together the four emoji slot buttons
/// </summary>
public class EmojiSlotButtonGroupScript : MonoBehaviour {
    
    private BluetoothEmojiSlotButton[] buttons;
    private RectTransform[] buttonRects;

	void Start () {
        buttons = new BluetoothEmojiSlotButton[transform.childCount];
        buttonRects = new RectTransform[transform.childCount];

        for (int i = 0; i < transform.childCount; i++) {
            buttons[i] = transform.GetChild(i).GetComponent<BluetoothEmojiSlotButton>();
            buttonRects[i] = transform.GetChild(i).GetComponent<RectTransform>();
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
