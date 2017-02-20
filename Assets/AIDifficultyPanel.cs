using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AIDifficultyPanel : MonoBehaviour {

    private float animTime = 0.3f;

    private AIScript aiScript;
    
	void Start () {
        aiScript = FindObjectOfType<AIScript>();
	}

    public void SetDifficultyToEasy() { aiScript.SetDifficulty(1); }
    public void SetDifficultyToNormal() { aiScript.SetDifficulty(2); }
    public void SetDifficultyToHard() { aiScript.SetDifficulty(3); }

    public void DismissDifficultyPanel() {
        transform.GetChild(0).GetComponent<RectTransform>().DOScale(0, animTime);
        GetComponent<Image>().DOFade(0f, animTime).OnComplete(new TweenCallback(() => {
            gameObject.SetActive(false);
        }));
    }
}
