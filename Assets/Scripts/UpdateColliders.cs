using UnityEngine;

public class UpdateColliders : MonoBehaviour {
    
	void Start () {
        // Get colliders. They should be children of the camera
        BoxCollider2D bottom = transform.FindChild("BottomCollider").GetComponent<BoxCollider2D>();
        BoxCollider2D top = transform.FindChild("TopCollider").GetComponent<BoxCollider2D>();
        BoxCollider2D left = transform.FindChild("LeftCollider").GetComponent<BoxCollider2D>();
        BoxCollider2D right = transform.FindChild("RightCollider").GetComponent<BoxCollider2D>();

        // Get world size of camera
        float camHeight = Camera.main.orthographicSize * 2;
        float camWidth = camHeight * ((float) Camera.main.pixelWidth / Camera.main.pixelHeight);

        // Set size and position correctly to scene
        bottom.gameObject.transform.position = new Vector3(0, -camHeight / 2f - 0.5f);
        bottom.size = new Vector2(camWidth, 1f);

        top.gameObject.transform.position = new Vector3(0, camHeight / 2f + 0.5f);
        top.size = new Vector2(camWidth, 1f);

        left.gameObject.transform.position = new Vector3(-camWidth / 2 - 0.5f, 0);
        left.size = new Vector2(1f, camHeight + 2); // +2 so it covers edges

        right.gameObject.transform.position = new Vector3(camWidth / 2 + 0.5f, 0);
        right.size = new Vector2(1f, camHeight + 2);
    }
}
