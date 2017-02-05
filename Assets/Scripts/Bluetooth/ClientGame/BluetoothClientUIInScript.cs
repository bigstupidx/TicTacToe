using UnityEngine;
using UnityEngine.UI;

public class BluetoothClientUIInScript : UIInScript {

    private new void Start() {
        base.Start();

        currentSign = GameObject.Find("CurrentImage").GetComponent<Image>();
    }
    private new void Update() { }
    
    public void UpdateImage(Cell.CellOcc newType) {
        currentlyDisplayed = newType;

        currentSign.sprite = SignResourceStorage.GetSpriteRelatedTo(newType);

        PlayCursorSpriteUpdateAnimation();
    }

}
