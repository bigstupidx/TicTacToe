using GooglePlayGames;
using System.Text;
using UnityEngine;

public class GPGridClickHandler : GridClickHandler {

    protected MessagePicker messagePicker;

    public override void Start() {
        gameLogic = FindObjectOfType<GPTTTGameLogic>();
        messagePicker = FindObjectOfType<GPMessagePicker>();

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
            int[] pos = Grid.GetCellInGridPos(clickPos);

            // It's going to check whether it is server's or client's turn (on the server side)

            PlayGamesPlatform.Instance.RealTime.SendMessageToAll(true, Encoding.Unicode.GetBytes(
                GPMessageStrings.TRY_PLACE_AT + "#" + pos[0].ToString() + "#" + pos[1].ToString()
            ));
        } else {
            // We are server side
            if (((GPTTTGameLogic) gameLogic).IsItServersTurn())
                ((GPTTTGameLogic) gameLogic).WantToPlaceAt(clickPos);
        }
    }

}
