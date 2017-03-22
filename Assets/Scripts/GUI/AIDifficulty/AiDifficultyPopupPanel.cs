using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class AiDifficultyPopupPanel : AIDifficultyPanel {

    private Image wholeScreenPanel;
    private RectTransform smallPanel;

    public override void Start() {
        base.Start();

        smallPanel = transform.parent.GetComponent<RectTransform>();
        wholeScreenPanel = smallPanel.parent.GetComponent<Image>();
    }

    public override void SelectButton(int which, bool user = true, TweenCallback onComplete = null) {
        // Hwe first dismiss the panel and only after that should we invoke the oncomplete
        TweenCallback dismissPanel = () => {
            // After that we need to dismiss the panel
            DOTween.Sequence()
                .Insert(0f, smallPanel.DOScale(0f, animTime))
                .Insert(animTime * 0.3f, wholeScreenPanel.DOFade(0f, animTime))
                .OnComplete(
                    new TweenCallback(() => {
                        if (onComplete != null) onComplete.Invoke();
                        wholeScreenPanel.gameObject.SetActive(false);

                        // Set the one in settingspanel to the current one
                        FindObjectOfType<AIDifficultyPanel>().SelectButton(selectedButton, false);
                    }
                ));
        };

        base.SelectButton(which, user, dismissPanel);
    }
}
