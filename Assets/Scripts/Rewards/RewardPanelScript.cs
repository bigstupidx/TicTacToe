using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using System;
using TMPro;

public class RewardPanelScript : MonoBehaviour {

    private CanvasScaler canvasScaler;

    public Image firstPanel;
    public TextMeshProUGUI pressToContinue;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    private GameObject rewardInstancePrefab;
    
	void Start() {
        rewardInstancePrefab = Resources.Load<GameObject>("Prefabs/Rewards/RewardInstance");
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        canvasScaler = transform.parent.GetComponent<CanvasScaler>();
    }

    private float firstPanelAnim = 1f;
    public void LevelUpAnimation() {
        rectTransform.localScale = new Vector3(1, 1, 1);
        canvasGroup.DOFade(1f, 0.3f)
            .OnComplete(new TweenCallback(() => {
                // Set level text correctly
                firstPanel.transform.FindChild("LevelText").GetComponent<TextMeshProUGUI>().text = "Level " + PreferencesScript.Instance.PlayerLevel;

                // Start press to coninue animation
                DOTween.Sequence()
                    .Append(pressToContinue.rectTransform.DOScale(1.1f, 0.4f).SetEase(Ease.InSine))
                    .Append(pressToContinue.rectTransform.DOScale(1.0f, 0.4f).SetEase(Ease.OutSine))
                    .SetLoops(20, LoopType.Yoyo);

                // needs to be here because we want to start it as a coroutine in firstpanel sequence
                Action showRewards = new Action(() => {
                    pressToContinue.DOFade(0f, 0.2f);

                    // First panel goes up
                    DOTween.Sequence()
                        .Append(firstPanel.rectTransform.DOScale(1f, firstPanelAnim * 0.7f))
                        .Append(firstPanel.rectTransform.DOAnchorPosY((canvasScaler.referenceResolution.y - firstPanel.rectTransform.rect.height) / 2f, firstPanelAnim * 0.7f))

                        // Bring in the crates
                        .OnComplete(new TweenCallback(() => {
                            BringInCrates();
                        }));
                });

                // First panel
                DOTween.Sequence()
                    .Insert(0f, firstPanel.rectTransform.DOScaleX(1.7f, firstPanelAnim * 0.8f).SetEase(Ease.OutBounce))
                    .Insert(0f, firstPanel.rectTransform.DOScaleY(1.7f, firstPanelAnim * 1.2f).SetEase(Ease.OutBounce))

                    .AppendCallback(new TweenCallback(() => StartCoroutine(ExecuteAfterScreenPressed(showRewards))))
                    .Append(pressToContinue.DOFade(1f, 0.1f));
            }));
    }

    private RewardInstanceScript[] crates;
    private void BringInCrates() {
        Unlockable[] unlocks = PreferencesScript.Instance.GetUnlocksForLevelAndUnlock(PreferencesScript.Instance.PlayerLevel);
        int unlockCount = 0;

        // Count how many there are
        for (int i = 0; i < unlocks.Length && unlocks[i] != null; i++)
            unlockCount++;

        crates = new RewardInstanceScript[unlockCount];
        
        for (int i = 0; i < unlockCount; i++) {
            RectTransform crate = Instantiate(rewardInstancePrefab, transform, false).GetComponent<RectTransform>();

            crates[i] = crate.GetComponent<RewardInstanceScript>();
            crates[i].SetUnlockable(unlocks[i]);

            crate.anchoredPosition = new Vector2((canvasScaler.referenceResolution.x + crate.rect.width) / 2f, canvasScaler.referenceResolution.y * -0.1f);
            crate
                .DOAnchorPosX(((canvasScaler.referenceResolution.x - unlockCount * crate.rect.width) / (unlockCount + 1f) * (i + 1) + crate.rect.width * i) - canvasScaler.referenceResolution.x / 2f, 1f)
                .SetDelay(i * 0.3f);
        }

        StartCoroutine(AfterAllCratesBroken());
    }

    private void HidePanel() {

        pressToContinue.DOKill();
        // Destroy crates
        for (int i = 0; i < crates.Length; i++) {
            if (crates[i] != null) {
                Destroy(crates[i].gameObject);
            }
        }
        // reset firstpanel
        firstPanel.rectTransform.localScale = new Vector3(0f, 0f, 1f);
        firstPanel.rectTransform.anchoredPosition = new Vector2(0, 0);


        rectTransform.DOScale(0f, 0.4f);
        canvasGroup.DOFade(0f, 0.4f);
    }

    private IEnumerator AfterAllCratesBroken() {
        bool allBroken = false;
        if (crates.Length == 0) allBroken = true;
        while (!allBroken) { 
            if (crates == null) yield return null;

            allBroken = true;
            for (int i = 0; i < crates.Length; i++)
                if (!crates[i].IsBroken())
                    allBroken = false;
            yield return null;
        }

        // So at this point we surely know that all of the crates have been broken
        yield return new WaitForSeconds(1f);
        
        pressToContinue.DOFade(1f, 0.2f);
        StartCoroutine(ExecuteAfterScreenPressed(new Action(() => {
            HidePanel();
        })));
    }

    private IEnumerator ExecuteAfterScreenPressed(Action action) {
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

        action.Invoke();
    }
}
