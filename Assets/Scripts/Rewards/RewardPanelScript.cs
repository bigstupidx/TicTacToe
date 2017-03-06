using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using System;
using TMPro;

public class RewardPanelScript : MonoBehaviour {

    private CanvasScaler canvasScaler;

    public Image firstPanel;
    private TextMeshProUGUI firstPanelStaticText;
    private TextMeshProUGUI levelUpText;
    Coroutine textAnimationCoroutine;

    public TextMeshProUGUI pressToContinue;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    private GameObject rewardInstancePrefab;
    public Canvas crateCanvas;

    Coroutine confettiCoroutine;
    public GameObject[] confettis;
    
	void Start() {
        firstPanelStaticText = firstPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        levelUpText = firstPanel.transform.GetChild(1).GetComponent<TextMeshProUGUI>();

        rewardInstancePrefab = Resources.Load<GameObject>("Prefabs/Rewards/RewardInstance");
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        canvasScaler = transform.parent.GetComponent<CanvasScaler>();
    }

    private float firstPanelAnim = 1f;
    public void LevelUpAnimation() {
        textAnimationCoroutine = StartCoroutine(PlayTextAniamtion());

        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        rectTransform.localScale = new Vector3(1, 1, 1);

        int unlockCount = PreferencesScript.Instance.GetUnlockCountAtLevel(PreferencesScript.Instance.PlayerLevel);
        // If there are unlocks left
        if (unlockCount != 0) { 
            canvasGroup.DOFade(1f, 0.3f)
                .OnComplete(new TweenCallback(() => {
                    confettiCoroutine = StartCoroutine(PlayConfettis());

                    // Set level text correctly
                    levelUpText.text = "Level " + PreferencesScript.Instance.PlayerLevel;

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
                            .Append(firstPanel.rectTransform.DOAnchorPosY((canvasScaler.referenceResolution.y - firstPanel.rectTransform.rect.height) * 0.4f, firstPanelAnim * 0.7f))

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
        } else { // There are no unlocks left
            StartCoroutine(ExecuteAfterScreenPressed(() => {
                HidePanel();
            }));
        }
    }

    private IEnumerator PlayTextAniamtion() {
        while (true) {
            float h, s, v;
            Color.RGBToHSV(firstPanelStaticText.outlineColor, out h, out s, out v);
            firstPanelStaticText.outlineColor = Color.HSVToRGB(h + 0.005f, s, v);
            Debug.Log("From hsv " + h + " " + s + " " + v + " to " + firstPanelStaticText.outlineColor);
            
            Color.RGBToHSV(levelUpText.outlineColor, out h, out s, out v);
            levelUpText.outlineColor = Color.HSVToRGB(h + 0.005f, s, v);

            yield return null;
        }
    }

    private IEnumerator PlayConfettis() {
        do {
            RectTransform confetti = Instantiate(confettis[UnityEngine.Random.Range(0, confettis.Length)], transform, false).GetComponent<RectTransform>();
            confetti.localPosition = new Vector3(UnityEngine.Random.Range(0f, Camera.main.pixelWidth) - Camera.main.pixelWidth / 2f, UnityEngine.Random.Range(0f, Camera.main.pixelHeight) - Camera.main.pixelHeight / 2f);
            confetti.eulerAngles= new Vector3(UnityEngine.Random.Range(0f, 360f), confetti.eulerAngles.y, confetti.eulerAngles.z);

            float widthHeight = Camera.main.pixelHeight * UnityEngine.Random.Range(0.2f, 0.3f);
            confetti.localScale = new Vector3(widthHeight, widthHeight, widthHeight);

            yield return new WaitForSeconds(UnityEngine.Random.value);
        } while (true);
    }

    private RewardInstanceScript[] crates;
    private void BringInCrates() {
        Unlockable[] unlocks = PreferencesScript.Instance.GetUnlocksForLevelAndUnlock(PreferencesScript.Instance.PlayerLevel);
        int unlockCount = 0;

        // Count how many there are
        for (int i = 0; i < unlocks.Length && unlocks[i] != null; i++)
            unlockCount++;

        if (unlockCount == 0) {
            return;
        }

        crates = new RewardInstanceScript[unlockCount];
        
        for (int i = 0; i < unlockCount; i++) {
            RectTransform crate = Instantiate(rewardInstancePrefab, crateCanvas.transform, false).GetComponent<RectTransform>();

            crates[i] = crate.GetComponent<RewardInstanceScript>();
            crates[i].SetUnlockable(unlocks[i]);

            int at = i; // used in callback
            crate.anchoredPosition = new Vector2((canvasScaler.referenceResolution.x + crate.rect.width) / 2f, canvasScaler.referenceResolution.y * -0.1f);
            crate
                .DOAnchorPosX(((canvasScaler.referenceResolution.x - unlockCount * crate.rect.width) / (unlockCount + 1f) * (i + 1) + crate.rect.width * i) - canvasScaler.referenceResolution.x / 2f, 1f)
                .SetDelay(i * 0.3f)
                .OnComplete(new TweenCallback(() => {
                    crates[at].CrateBroughtIn();
                }));
        }

        StartCoroutine(AfterAllCratesBroken());
    }

    private void HidePanel() {
        StopCoroutine(textAnimationCoroutine);
        StopCoroutine(confettiCoroutine);

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

        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
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
