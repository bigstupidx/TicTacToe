using UnityEngine;

public class AIGridClickHandler : GridClickHandler {

    public override void ClickedAt(Vector2 clickPos) {
        if (gameLogic.WhoseTurn == AIScript.HumanType)
            base.ClickedAt(clickPos);
    }
}
