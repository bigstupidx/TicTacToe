using UnityEngine;
using UnityEngine.UI;

public class BluetoothClientUIInScript : UIInScript {

    private new void Start() {
        base.Start();

        currentSign = GameObject.Find("CurrentImage").GetComponent<Image>();
        UpdateImage(Cell.CellOcc.O);
    }
    private new void Update() { }
    
    public void UpdateImage(Cell.CellOcc newType) {
        currentlyDisplayed = newType;

        currentSign.sprite = SignResourceStorage.GetSpriteRelatedTo(newType);
        currentSign.color = SignResourceStorage.GetColorRelatedTo(newType);

        if (newType != currentlyDisplayed)
            PlayCursorSpriteUpdateAnimation();
    }

}
