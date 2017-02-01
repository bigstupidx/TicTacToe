using UnityEngine;
using UnityEngine.UI;

public class BluetoothClientUIInScript : UIInScript {

    private new void Start() {
        cursorSign = GameObject.Find("CurrentImage").GetComponent<Image>();
    }
    private new void Update() { }
    
    public void UpdateImage(Cell.CellOcc newType) {
        currentlyDisplayed = newType;

        cursorSign.sprite = SignResourceStorage.GetSpriteRelatedTo(newType);

        PlaySpriteUpdateAnimation();
    }

}
