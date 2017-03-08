using UnityEngine;
using DG.Tweening;

public class AIDifficultyPanel : MonoBehaviour {

    private float animTime = 0.3f;

    private AIScript aiScript;
    private BackButton backButton;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;

    private bool isShown = false;
    
	void Start () {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = transform.GetChild(0).GetComponent<RectTransform>();
        aiScript = FindObjectOfType<AIScript>();
        backButton = FindObjectOfType<BackButton>();

        PopupDifficultyPanel();
	}

    void Update() {
        if (isShown && Input.GetKeyDown(KeyCode.Escape)) {
            DismissDifficultyPanel();
        }
    }

    public void SetDifficultyToEasy() {
        if (aiScript.IsGameInProgress()) {
            PopUpThatGameIsInProgress();
            return;
        }
        aiScript.SetDifficulty(1);
    }
    public void SetDifficultyToNormal() {
        if (aiScript.IsGameInProgress()) {
            PopUpThatGameIsInProgress();
            return;
        }
        aiScript.SetDifficulty(2);
    }
    public void SetDifficultyToHard() {
        if (aiScript.IsGameInProgress()) {
            PopUpThatGameIsInProgress();
            return;
        }
        aiScript.SetDifficulty(3);
    }

    private void PopUpThatGameIsInProgress() {
        PopupManager.Instance.PopUp("Can't change difficulty\nwhile game is in progress!", "OK");
    }

    public void PopupDifficultyPanel() {
        isShown = true;
        backButton.enabled = false;

        rectTransform.DOScale(1f, animTime).SetEase(Ease.OutBack).OnComplete(new TweenCallback(() => {
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
        }));
        canvasGroup.DOFade(1f, animTime);
    }

    public void DismissDifficultyPanel() {
        isShown = false;
        backButton.enabled = true;

        rectTransform.DOScale(0, animTime).OnComplete(new TweenCallback(() => {
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        }));
        canvasGroup.DOFade(0f, animTime);

    }
}
