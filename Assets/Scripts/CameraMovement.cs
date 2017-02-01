using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {

    private const int PAN_MOUSE_BUTTON = 2;

    private bool isPanning = false;
    private Vector3 panPrevPos;
	
	void Update() {
#if UNITY_STANDALONE || UNITY_EDITOR
        // Started dragging
        if (Input.GetMouseButtonDown(PAN_MOUSE_BUTTON)) {
            isPanning = true;
            panPrevPos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        }

        // Is dragging
        if (Input.GetMouseButton(PAN_MOUSE_BUTTON) && isPanning) {
            // Calculate current position of camera in viewport pos
            // We use viewport instead of world pos because the worldpos changes as we move the camera so it just move in a direction constantly when we drag
            Vector3 panCurrPos = Camera.main.ScreenToViewportPoint(Input.mousePosition);

            // Calculate the distance between the previous frame's pos and the current one's but this we need to revert back to worldpos
            Vector3 panDelta = Camera.main.ViewportToWorldPoint(panCurrPos) - Camera.main.ViewportToWorldPoint(panPrevPos);
            transform.position -= panDelta; // Update cam pos

            // The current pos becomes the next frame's prev pos
            panPrevPos = panCurrPos;
        }

        // Dragging end
        if (Input.GetMouseButtonUp(PAN_MOUSE_BUTTON)) {
            isPanning = false;
        }
#endif
    }
}
