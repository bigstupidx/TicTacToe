using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AIDifficultyPanel : MonoBehaviour {

    private float animTime = 0.3f;

    private AIScript aiScript;
    private RectTransform[] buttons;

    private bool isShown = false;
    private int selectedButton = -1;
    
	void Start () {
        aiScript = FindObjectOfType<AIScript>();

        buttons = new RectTransform[transform.childCount];
        for (int i = 1; i <= 3; i++) {
            buttons[i] = transform.GetChild(i).GetComponent<RectTransform>();

            int diff = i;
            buttons[i].GetComponent<Button>().onClick.AddListener(() => {
                SetDifficultyTo(diff);
            });
        }
        SelectButton(2);
	}

    public void SetDifficultyToEasy() {
        SetDifficultyTo(1);
    }
    public void SetDifficultyToNormal() {
        SetDifficultyTo(2);
    }
    public void SetDifficultyToHard() {
        SetDifficultyTo(3);
    }
    public void SetDifficultyTo(int diff) {
        if (aiScript.IsGameInProgress()) {
            PopUpThatGameIsInProgress();
            return;
        }

        aiScript.SetDifficulty(diff);
        SelectButton(diff);
    }

    private void PopUpThatGameIsInProgress() {
        PopupManager.Instance.PopUp("Can't change difficulty\nwhile game is in progress!", "OK");
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

        buttons[which].DOScale(1.1f, 0.2f).OnComplete(onComplete);
    }
}
