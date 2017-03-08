using UnityEngine;

public class FirstGameDrawGrid : AIDrawGrid {
    public override void Start() {
        base.Start();

        // Disable movement for the player at start
        Camera.main.GetComponent<CameraMovement>().enabled = false;
        gridManager.GetComponent<AIGridClickHandler>().isMovementEnabled = false;
        gridManager.GetComponent<AIGridClickHandler>().isZoomEnabled = false;
    }
}
