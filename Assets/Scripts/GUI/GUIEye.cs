using UnityEngine;
using UnityEngine.UI;

public class GUIEye : MonoBehaviour {

    private bool seperateEyeMovement = true;

    private GameObject leftEye;
    private GameObject leftEyeBall;
    private Image leftEyeImage;

    private GameObject rightEye;
    private GameObject rightEyeBall;
    private Image rightEyeImage;

    private float horizontalR;
    private float verticalR;

	void Start () {
        // Get the parts of the eye
        leftEye = transform.FindChild("LeftEye").gameObject;
        leftEyeBall = leftEye.transform.FindChild("EyeBall").gameObject;
        leftEyeImage = leftEyeBall.GetComponent<Image>();

        rightEye = transform.FindChild("RightEye").gameObject;
        rightEyeBall = rightEye.transform.FindChild("EyeBall").gameObject;
        rightEyeImage = rightEyeBall.GetComponent<Image>();

        // Ellipse
        horizontalR = (leftEye.GetComponent<Image>().rectTransform.rect.width - leftEyeImage.rectTransform.rect.width) / 2f * (Camera.main.pixelWidth / 1920f);
        verticalR = (leftEye.GetComponent<Image>().rectTransform.rect.height - leftEyeImage.rectTransform.rect.height) / 2f * (Camera.main.pixelWidth / 1920f);
        calculationConstant = horizontalR * horizontalR * verticalR * verticalR;

        // Randomize eye movement seperation
        seperateEyeMovement = Random.Range(0, 2) == 0;
	}

    float calculationConstant;
	void Update () {
		if (Input.touchCount == 1 || Input.GetMouseButton(0)) {
            Vector3 touchOnScreen = Input.mousePosition;

            // Left eye
            Vector2 touchPoint = touchOnScreen - leftEye.transform.position; // Touch point relative to eye

            // Solve equation system
            float xPoint = Mathf.Sqrt(calculationConstant / (verticalR * verticalR + Mathf.Pow(horizontalR * touchPoint.y / touchPoint.x, 2)));
            float yPoint = touchPoint.y / touchPoint.x * xPoint;

            if (touchOnScreen.x < leftEye.transform.position.x) {
                xPoint *= -1; yPoint *= -1;
            }

            // Set position
            leftEyeImage.rectTransform.position = new Vector3(xPoint, yPoint) + leftEye.transform.position;
            if (!seperateEyeMovement) // Only move second eye with first one if the eye movement is not seperates
                rightEyeImage.rectTransform.position = new Vector3(xPoint, yPoint) + rightEye.transform.position;

            
            // If eye movement is seperate move right eye as well
            if (seperateEyeMovement) {
                // Right eye - Same as left eye
                touchPoint = touchOnScreen - rightEye.transform.position;

                xPoint = Mathf.Sqrt(calculationConstant / (verticalR * verticalR + Mathf.Pow(horizontalR * touchPoint.y / touchPoint.x, 2)));
                yPoint = touchPoint.y / touchPoint.x * xPoint;

                if (touchOnScreen.x < rightEye.transform.position.x) {
                    xPoint *= -1; yPoint *= -1;
                }

                // Set position
                rightEyeImage.rectTransform.position = new Vector3(xPoint, yPoint) + rightEye.transform.position;
            }
        }
    }
}
