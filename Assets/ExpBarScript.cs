using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ExpBarScript : MonoBehaviour {

    private bool isPulledDown = false;
    public bool IsPulledDown { get { return isPulledDown; } }

    private RectTransform rectTransform;
    public Text levelText;
    public Text maxExpText;
    public Text currExpText;
    public Text addExpText;
    public Slider expSlider;
    
	void Start() {
        if (FindObjectsOfType<ExpBarScript>().Length > 1) {
            Destroy(gameObject);
            return;
        }

        // move parent canvas to dondestroyonload
        DontDestroyOnLoad(transform.parent.gameObject);

        // Inits
        rectTransform = GetComponent<RectTransform>();

        // Set starting values
        UpdateLevelText(PreferencesScript.Instance.PlayerLevel);
        UpdateMaxExpText(PreferencesScript.Instance.ExpForNextLevel());
        UpdateCurrExp(PreferencesScript.Instance.PlayerEXP, PreferencesScript.Instance.ExpForNextLevel(), false);
	}

    /// <summary>
    /// Only displays the little text thingy, doesn't update other text or the expbar
    /// </summary>
    public void AddExpAnimation(int addExp) {
        GameObject go = Instantiate(addExpText.gameObject, transform, false);

        Text aeText = go.GetComponent<Text>();
        aeText.text = "+" + addExp;
        DOTween.Sequence()
            .Insert(0f, aeText.DOFade(1f, 0f))
            .Insert(0.05f, aeText.rectTransform.DOAnchorPosY(aeText.rectTransform.anchoredPosition.y + aeText.rectTransform.rect.height, 2f))
            .Insert(0.3f, aeText.DOFade(0f, 1.3f))
            .OnComplete(new TweenCallback(() => {
                Destroy(go);
            }));
    }

    public void UpdateLevelText(int level) {
        levelText.text = level.ToString();
    }

    public void UpdateMaxExpText(int maxExp) {
        maxExpText.text = "/" + maxExp.ToString();
    }

    /// <summary>
    /// Updates both bar and text
    /// </summary>
    public void UpdateCurrExp(int currExp, int maxExp, bool levelUp) {
        currExpText.text = currExp.ToString();

        if (!levelUp)
            expSlider.DOValue(currExp / (float) maxExp, 0.5f);
        else
            DOTween.Sequence()
                .Append(expSlider.DOValue(1f, 0.5f))
                .Append(expSlider.DOValue(0, 0f).SetDelay(0.2f))
                .Append(expSlider.DOValue(currExp / (float) maxExp, 0.5f).SetDelay(0.05f));
    }

    public void UpdateLevelUpTexts(int level, int maxExp, int currExp) {
        UpdateLevelText(level);
        UpdateMaxExpText(maxExp);
        UpdateCurrExp(currExp, maxExp, true);
    }

    public void ToggleExpBar() {
        if (isPulledDown) PushUpExpBar();
        else PullDownExpBar();
    }

    public void PullDownExpBar(TweenCallback callback) {
        rectTransform.DOAnchorPosY(-rectTransform.rect.height / 2f, 0.3f)
            .OnComplete(callback);
        isPulledDown = true;
    }

    public void PullDownExpBar() {
        PullDownExpBar(null);
    }

    public void PushUpExpBar(TweenCallback callback) {
        rectTransform.DOAnchorPosY(rectTransform.rect.height / 2f, 0.3f)
            .OnComplete(callback);
        isPulledDown = false;
    }

    public void PushUpExpBar() {
        PushUpExpBar(null);
    }
}
