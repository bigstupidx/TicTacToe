using UnityEngine;
using DG.Tweening;

public class MenuCameraMovement : MonoBehaviour {

    private const float radius = 30;
    private const float timePerMove = 20;
    private Camera myCamera;

	void Start () {
        myCamera = GetComponent<Camera>();
        MoveCameraToNewRandPos();
	}

    private void MoveCameraToNewRandPos() {
        // Get new position where to go inside radius circle around spawn
        Vector3 randPos = Random.insideUnitCircle * radius;
        randPos.z = myCamera.transform.position.z;

        // Go there and after it is ready do it again
        myCamera.transform.DOMove(randPos, timePerMove).SetEase(Ease.Linear).OnComplete(new TweenCallback(() => {
            MoveCameraToNewRandPos();
        }));
    }
}
