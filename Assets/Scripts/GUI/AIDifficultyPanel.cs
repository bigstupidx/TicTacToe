using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AIDifficultyPanel : MonoBehaviour {

    protected float animTime = 0.3f;

    protected AIScript aiScript;
    protected RectTransform[] buttons;

    protected bool isShown = false;
    protected int selectedButton = -1;
    
	public virtual void Start () {
        aiScript = FindObjectOfType<AIScript>();

        buttons = new RectTransform[transform.childCount];
        for (int i = 1; i <= 3; i++) {
            buttons[i] = transform.GetChild(i).GetComponent<RectTransform>();

            int diff = i;
            buttons[i].GetComponent<Button>().onClick.AddListener(() => {
                SetDifficultyTo(diff);
            });
        }
        SelectButton(2, false);
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

    protected void PopUpThatGameIsInProgress() {
        PopupManager.Instance.PopUp("Can't change difficulty\nwhile game is in progress!", "OK");
    }

    /// <summary>
    /// is user is false then the onComplete method won't be invoked
    /// </summary>
    public virtual void SelectButton(int which, bool user = true, TweenCallback onComplete = null) {
        if (which == selectedButton) {
            // Same button is selected as the one we want to select so completion is immedaite
            if (onComplete != null && user) onComplete.Invoke();

            return;
        }

        selectedButton = which;

        // user pressed so do tween and do callback
        if (user) { 
            for (int i = 1; i <= 3; i++) {
                if (buttons[i].localScale != new Vector3(1f, 1f, 1f)) {
                    buttons[i].DOScale(1f, 0.1f);
                }
            }

            buttons[which].DOScale(1.1f, 0.2f).OnComplete(onComplete);
        } else { // we just called it but the user didn't press so don't do tween and don't call callback
            for (int i = 1; i <= 3; i++)
                if (buttons[i].localScale != new Vector3(1f, 1f, 1f))
                    buttons[i].localScale = new Vector3(1f, 1f, 1f);

            buttons[which].localScale = new Vector3(1.1f, 1.1f, 1.1f);
        }
    }
}
