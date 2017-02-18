using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ScoringScript : MonoBehaviour {

    private float ANIMATION = .2f;

    private Text xText;
    private Text oText;

    private int currentX = 0;
    private int currentO = 0;

    private float height;

    void Start() {
        xText = transform.FindChild("XText").GetComponent<Text>();
        oText = transform.FindChild("OText").GetComponent<Text>();

        height = oText.rectTransform.rect.height;

        transform.FindChild("XImage").GetComponent<Image>().color = SignResourceStorage.xColor;
        transform.FindChild("OImage").GetComponent<Image>().color = SignResourceStorage.oColor;
    }

    public void SetScore(int x, int o) {
        Text txt;
        string s;
        // We need to refresh o
        if (currentX == x) {
            s = o.ToString();
            txt = oText;
            currentO = o;
        } else { // we need to refresh x
            s = x.ToString();
            txt = xText;
            currentX = x;
        }
        // aniamtion
        DOTween.Sequence()
                .Insert(0, txt.rectTransform.DOLocalMoveY(height / 2f, ANIMATION))
                .Insert(0f, txt.DOFade(0, ANIMATION))
                .OnComplete(new TweenCallback(() => {
                    txt.rectTransform.localPosition = new Vector2(txt.rectTransform.localPosition.x, -height / 2f);
                    txt.text = s;
                    DOTween.Sequence()
                        .Insert(0f, txt.rectTransform.DOLocalMoveY(0, ANIMATION))
                        .Insert(0f, txt.DOFade(1, ANIMATION));
            })
        );
    }
}
