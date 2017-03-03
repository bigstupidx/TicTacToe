using UnityEngine;

public class BluetoothGridClickHandler : GridClickHandler {

    protected MessagePicker messagePicker;
    
	public override void Start () {
        gameLogic = FindObjectOfType<BluetoothTTTGameLogic>();
        messagePicker = FindObjectOfType<MessagePicker>();

        fingerMoveMin = Camera.main.pixelHeight * 0.01f;
    }

    public override void Update() {
        if (!messagePicker.IsDragging)
            base.Update();
    }

    public override void ClickedAt(Vector2 clickPos) {
        if (IsPointerOverUIObject())
            return;

        // We are on client side
        if (gameLogic == null) {
            Debug.Log("Clicked and client side");
            int[] pos = Grid.GetCellInGridPos(clickPos);

            // It's going to check whether it is server's or client's turn (on the server side)
            Bluetooth.Instance().Send(BluetoothMessageStrings.TRY_PLACE_AT + "#" + pos[0].ToString() + "#" + pos[1].ToString());
        } else {
            Debug.Log("Clicked and server side is it serves turn? " + ((BluetoothTTTGameLogic) gameLogic).IsItServersTurn());
            // We are server side
            if (((BluetoothTTTGameLogic) gameLogic).IsItServersTurn())
                ((BluetoothTTTGameLogic) gameLogic).WantToPlaceAt(clickPos);
        }
    }
}
