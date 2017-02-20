using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FirstGameManager : MonoBehaviour {

    public Image clickImge;
    public Text helpText;

    private Grid grid;
    private AITTTGameLogic gameLogic;
    private AIScript aiScript;
    private int signCount = 0;

	void Start () {
        grid = GetComponent<Grid>();
        gameLogic = GetComponent<AITTTGameLogic>();
        aiScript = GetComponent<AIScript>();

        Color htc = helpText.color; htc.a = 0; helpText.color = htc;

        // change think time of ai to a bit more
        gameLogic.aiMinThinkTime = 1.3f;

        // change ai difficulty to really easy
        aiScript.SetDifficulty(0);

        // subscribe to placed sign event
        grid.SignWasPlaced += SignWasPlayed;

        clickImge.DOFade(1f, 0.2f);
        clickImge.rectTransform.DOScale(1.1f, 0.6f).SetLoops(-1, LoopType.Yoyo);
	}

    private void SignWasPlayed(int[] pos, Cell.CellOcc type) {
        signCount++;
        
        switch (signCount) {
            case 1: // The first sign was placed so just fade out the clickimage
                clickImge.DOKill();
                clickImge.DOFade(0f, 0.4f);
                break;
            case 3: // First help text
                helpText.rectTransform.DOLocalMoveY(helpText.rectTransform.localPosition.y + 50f, 1.3f);
                helpText.DOFade(1f, 1.3f);

                DOTween.Sequence()
                    .Insert(4f, helpText.DOFade(0f, 1.3f))
                    .Insert(4f, helpText.rectTransform.DOLocalMoveY(helpText.rectTransform.localPosition.y + 50f, 1.3f));
                break;
            case 5: // move the camera out a bit
                Camera.main.DOOrthoSize(Camera.main.orthographicSize * 1.3f, 10f).SetEase(Ease.InQuart);
                FindObjectOfType<AIGridClickHandler>().isZoomEnabled = true;
                break;
            case 10: // Give move and zoom ailities

                Vector2 clickimgPos = new Vector2(-Camera.main.pixelWidth / 2 * 0.5f, -Camera.main.pixelHeight / 2 * 0.7f);
                DOTween.Sequence()
                    .Append(clickImge.rectTransform.DOLocalMove(clickimgPos, 0f))
                    .Append(clickImge.DOFade(1f, 0.2f))
                    .Append(clickImge.rectTransform.DOLocalMoveX(clickimgPos.x + Camera.main.pixelWidth / 2f, 1.4f))
                    .Append(clickImge.DOFade(0f, 0.2f))
                    .SetLoops(3, LoopType.Restart);

                Camera.main.GetComponent<CameraMovement>().enabled = true;
                FindObjectOfType<AIGridClickHandler>().isMovementEnabled = true;
                break;
        }
    }
}
