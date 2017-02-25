using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;
using System;

public class FirstGameManager : MonoBehaviour {

    private CanvasScaler canvasScaler;

    public RectTransform scoringRect;
    public GameObject backButton;
    public Image clickImge;
    public Image topLeftCorner;
    public Image bottomRightCorner;
    public Text helpText;
    public Text alertText;

    private Grid grid;
    private AITTTGameLogic gameLogic;
    private AIScript aiScript;

    private Coroutine alertCoroutine;
    private int alertIncrement = 0;
    private bool isAlertEnabled = true;
    private string[] yourTurnTexts = new string[] {
        "It's your turn!",
        "Place your O!",
        "Please, place!",
        "Pretty please?"
    };

    private int signCount = 0;
    private int aiWinCount = 0;
    private int humanWinCount = 0;

	void Start () {
        grid = GetComponent<Grid>();
        gameLogic = GetComponent<AITTTGameLogic>();
        aiScript = GetComponent<AIScript>();

        canvasScaler = GameObject.Find("Canvas").GetComponent<CanvasScaler>();

        Color htc = helpText.color; htc.a = 0; helpText.color = htc;

        // change think time of ai to a bit more
        gameLogic.aiMinThinkTime = 1.3f;

        // change ai difficulty to really ahrd first so the player doesnt get throught the first like 20 signs for sure
        aiScript.SetDifficulty(4);

        // subscribe to placed sign event
        grid.SignWasPlaced += SignWasPlayed;
        gameLogic.SomeoneWonGame += SomeoneWonGame;

        clickImge.DOFade(1f, 0.2f);
        clickImge.rectTransform.DOScale(1.1f, 0.6f).SetLoops(-1, LoopType.Yoyo);
	}

    private void SomeoneWonGame(Cell.CellOcc type) {
        if (type == AIScript.AIType) aiWinCount++;
        else humanWinCount++;

        // First win of anybody
        if (aiWinCount + humanWinCount == 1) {
            scoringRect.DOLocalMoveY(scoringRect.localPosition.y - 100, 2f);

            // AI won
            if (aiWinCount == 1) {
                helpText.text = "Try to prevent the AI from having three signs in a row!";
                ShowThenHideHelpText(4f);
            }
        }

        // Player won for the first time
        if (humanWinCount == 1) {
            helpText.text = "Good job!\n Now you can go back to menu with the back button on you phone!";
            ShowThenHideHelpText(5f);

            backButton.SetActive(true);
            PreferencesScript.Instance.SetTutorialToCompleted();
            aiScript.SetDifficulty(1);
        } else if (humanWinCount == 2) {
            aiScript.SetDifficulty(2);
        }
    }

    private void ShowThenHideHelpText(float showTime) {
        helpText.rectTransform.localPosition = new Vector2(helpText.rectTransform.localPosition.x, helpText.rectTransform.localPosition.y - 50f);

        helpText.rectTransform.DOLocalMoveY(helpText.rectTransform.localPosition.y + 50f, 1.3f);
        helpText.DOFade(1f, 1.3f);
        helpText.DOFade(0f, 1.3f).SetDelay(showTime);
    }

    private void HideAlertText() {
        // If player placed and it is still queued to alert him stop it
        if (alertCoroutine != null)
            StopCoroutine(alertCoroutine);

        // If alertext is shown fade it out
        if (alertText.color.a > 0) {
            alertText.DOKill();
            alertText.DOFade(0f, 1.5f);
        }
    }

    private void SignWasPlayed(int[] pos, Cell.CellOcc type) {
        signCount++;

        if (isAlertEnabled) { 
            if (type == AIScript.HumanType) {
                HideAlertText();
            } else {
                // If AI's turn just ended we want to alert player that it's his turn after some seconds
                alertCoroutine = StartCoroutine(ExecuteAfterSecond(10f, new Action(() => {
                    alertCoroutine = null;

                    alertText.text = yourTurnTexts[alertIncrement];
                    alertText.DOFade(1f, 1.5f);

                    alertIncrement++;
                    if (alertIncrement >= yourTurnTexts.Length) alertIncrement = 0; 
                })));
            }
        }

        switch (signCount) {
            case 1: // The first sign was placed so just fade out the clickimage
                clickImge.DOKill();
                clickImge.DOFade(0f, 0.4f);

                // After first sign was placed after some seconds show the current sign thingy in the top left
                StartCoroutine(ExecuteAfterSecond(45f, new Action(() => {
                    topLeftCorner.rectTransform.DOLocalMoveX(topLeftCorner.rectTransform.localPosition.x + topLeftCorner.rectTransform.rect.width, 0.8f);

                    isAlertEnabled = false;
                    HideAlertText();
                })));

                // After first sign was placed after some seconds show the go to last sign placed thingy in bot right
                StartCoroutine(ExecuteAfterSecond(75f, new Action(() => {
                    bottomRightCorner.rectTransform.DOLocalMoveX(bottomRightCorner.rectTransform.localPosition.x - bottomRightCorner.rectTransform.rect.width, 0.8f);
                })));
                break;
            case 3: // First help text
                ShowThenHideHelpText(4f);
                break;
            case 5: // move the camera out a bit
                Camera.main.DOOrthoSize(Camera.main.orthographicSize * 1.3f, 10f).SetEase(Ease.InQuart);
                FindObjectOfType<AIGridClickHandler>().isZoomEnabled = true;
                break;
            case 13: // Give move abilities

                Vector2 clickimgPos = new Vector2(-canvasScaler.referenceResolution.x * 0.25f, -canvasScaler.referenceResolution.y / 2 * 0.7f);
                DOTween.Sequence()
                    .Append(clickImge.rectTransform.DOLocalMove(clickimgPos, 0f))
                    .Append(clickImge.DOFade(1f, 0.2f))
                    .Append(clickImge.rectTransform.DOLocalMoveX(canvasScaler.referenceResolution.x * 0.25f, 1.4f))
                    .Append(clickImge.DOFade(0f, 0.2f))
                    .SetLoops(2, LoopType.Restart);

                Camera.main.GetComponent<CameraMovement>().enabled = true;
                FindObjectOfType<AIGridClickHandler>().isMovementEnabled = true;

                // After this we don't depend on signs anymore for showing text so set ai difficulty to really easy
                aiScript.SetDifficulty(0);

                break;
        }
    }

    private IEnumerator ExecuteAfterSecond(float delay, Action action) {
        yield return new WaitForSeconds(delay);
        action.Invoke();
    }
}
