using UnityEngine;
using DG.Tweening;

public class AIDifficultyPanel : MonoBehaviour {

    private float animTime = 0.3f;

    private AIScript aiScript;
    private BackButton backButton;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private RectTransform[] buttons;

    private bool isShown = false;
    private int selectedButton = 2;
    
	void Start () {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = transform.GetChild(0).GetComponent<RectTransform>();
        aiScript = FindObjectOfType<AIScript>();
        backButton = FindObjectOfType<BackButton>();

        buttons = new RectTransform[rectTransform.childCount];
        for (int i = 1; i <= 3; i++) {
            buttons[i] = rectTransform.GetChild(i).GetComponent<RectTransform>();
        }

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

        buttons[aiScript.Difficulty].localScale = new Vector3(1.2f, 1.2f, 1.2f);
    }

    /// <summary>
    /// Called from the difficulty panel buttons
    /// </summary>
    public void DismissDifficultyPanel() {
        SelectButton(aiScript.Difficulty, new TweenCallback(() => {
            isShown = false;
            backButton.enabled = true;

            rectTransform.DOScale(0, animTime).OnComplete(new TweenCallback(() => {
                canvasGroup.blocksRaycasts = false;
                canvasGroup.interactable = false;
            }));
            canvasGroup.DOFade(0f, animTime);

            for (int i = 1; i <= 3; i++) {
                buttons[i].localScale = new Vector3(1f, 1f, 1f);
            }
        }));
    }

    private void SelectButton(int which, TweenCallback onComplete = null) {
        if (which == selectedButton) {
            // Same button is selected as the one we want to select so completion is immedaite
            if (onComplete != null) onComplete.Invoke();

            return;
        }

        selectedButton = which;

        for (int i = 1; i <= 3; i++) {
            if (buttons[i].localScale != new Vector3(1f, 1f, 1f)) {
                buttons[i].DOScale(1f, 0.1f);
            }
        }

        buttons[which].DOScale(1.2f, 0.2f).OnComplete(onComplete);
    }
}
