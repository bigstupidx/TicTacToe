using UnityEngine;

public class FirstGameGridClickHandler : AIGridClickHandler {

    public bool isMovementEnabled = false;

    public override void ClickedAt(Vector2 clickPos) {
        base.ClickedAt(clickPos);
    }

}
