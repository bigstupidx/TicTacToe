using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class GridClickHandler : MonoBehaviour {

    protected const float zoomSpeed = 0.03f;
    protected const float minOrthSize = 4f;
    protected const float maxOrthSize = 15f;
    
    protected TTTGameLogic gameLogic;

    protected float fingerMoveMin; // How much the finger needs to move in pixels in order for the camera to be moved
    protected bool zooming = false;

    public virtual void Start() {
        gameLogic = FindObjectOfType<TTTGameLogic>();

        fingerMoveMin = Camera.main.pixelHeight * 0.01f;
    }

    Vector2 moveAmount;
    Vector3 fingerPrevPos;

    public virtual void Update() {
        if (GridClickHandler.IsPointerOverUIObject())
            return;

        // Ended zooming
        if (Input.touchCount == 0 && zooming) zooming = false;

        if (Input.touchCount == 2) { // Zooming
            zooming = true;

            // Store both touches.
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // Find the position in the previous frame of each touch.
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // Find the magnitude of the vector (the distance) between the touches in each frame.
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // Find the difference in the distances between each frame.
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            // Change the orthographic size based on the change in distance between the touches.
            Camera.main.orthographicSize += deltaMagnitudeDiff * zoomSpeed;

            // Make sure the orthographic size stays between the given numbers
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, minOrthSize, maxOrthSize);
        } else if (Input.touchCount == 1 && !zooming) {
            Touch touch = new Touch();
            touch = Input.GetTouch(0);
            
            if (touch.phase == TouchPhase.Began) {
                moveAmount.x = 0; moveAmount.y = 0;
                fingerPrevPos = Camera.main.ScreenToViewportPoint(touch.position);
                // Move grid
            } else if (touch.phase == TouchPhase.Moved) {
                moveAmount += new Vector2(Mathf.Abs(touch.deltaPosition.x), Mathf.Abs(touch.deltaPosition.y));

                if (moveAmount.x > fingerMoveMin || moveAmount.y > fingerMoveMin) {
                    // Set finger pos in viewport coords
                    Vector3 fingerPos = Camera.main.ScreenToViewportPoint(touch.position);
                    Vector3 fingerDelta = Camera.main.ViewportToWorldPoint(fingerPos) - Camera.main.ViewportToWorldPoint(fingerPrevPos);

                    Camera.main.transform.position -= fingerDelta;

                    // Set finger's prev pos in viewport coords
                    fingerPrevPos = Camera.main.ScreenToViewportPoint(touch.position);
                }

            } else if (Input.touchCount == 1 && touch.phase == TouchPhase.Ended) {
                // Not moved finger
                if (moveAmount.x <= fingerMoveMin && moveAmount.y <= fingerMoveMin) {
                    Vector2 clickPos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);

                    ClickedAt(clickPos);
                }
            }
        }

#if UNITY_STANDALONE || UNITY_EDITOR
        if (Input.GetMouseButtonUp(0)) {
            Vector2 clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            ClickedAt(clickPos);
        }
#endif
    }

    public virtual void ClickedAt(Vector2 clickPos) {
        gameLogic.WantToPlaceAt(clickPos);
    }

    public static bool IsPointerOverUIObject() {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}
