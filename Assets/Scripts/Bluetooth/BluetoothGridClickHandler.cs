using UnityEngine;

public class BluetoothGridClickHandler : GridClickHandler {
    
	public override void Start () {
        gameLogic = FindObjectOfType<BluetoothTTTGameLogic>();

        fingerMoveMin = Camera.main.pixelHeight * 0.01f;
    }

    public override void ClickedAt(Vector2 clickPos) {
        // We are on client side
        if (gameLogic == null) {
            int[] pos = Grid.GetCellInGridPos(clickPos);

            // It's going to check whether it is server's or client's turn (on the server side)
            Bluetooth.Instance().Send(BluetoothMessageStrings.TRY_PLACE_AT + "#" + pos[0].ToString() + "#" + pos[1].ToString());
        } else {
            // We are server side
            if (((BluetoothTTTGameLogic) gameLogic).IsItServersTurn())
                gameLogic.WantToPlaceAt(clickPos);
        }
    }
}
