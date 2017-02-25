using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class MenuButtonPanelScript : MonoBehaviour {
    
    // Both of those below are dictated by this, those are only precentages while this controls all timings
    float animTime = 0.4f;
    // The time gap between the in and out animation happend
    float inOtDelay = 0.2f;
    // The time gap between the buttons going in and out
    float gapBetween = 0.2f;

    [SerializeField]
    [Tooltip("Which button is pressed in order to birng out second panel")]
    private MenuButton mainButton;

    [SerializeField]
    [Tooltip("The back button of the second panel")]
    private MenuButton backButton;

    [SerializeField]
    [Tooltip("All of the buttons of the main button panel")]
    private MenuButton[] firstPanelButtons;

    [SerializeField]
    [Tooltip("The buttons to bring out")]
    private MenuButton[] secondPanelButtons;

    private bool isSecondPanelShown = false;

    void Awake() {
        // If we haven't completed the tutorial don't bring in the menus
        if (!PreferencesScript.Instance.IsTutorialCompleted())
            GetComponent<DOTweenAnimation>().DOKill();
    }
    
	void Start () {
        // Get canvasgroup
        mainButton.canavasGroup = mainButton.rectTransform.GetComponent<CanvasGroup>();
        backButton.canavasGroup = backButton.rectTransform.GetComponent<CanvasGroup>();

        int mainAt = -1;
        for (int i = 0; i < firstPanelButtons.Length; i++) { 
            firstPanelButtons[i].canavasGroup = firstPanelButtons[i].rectTransform.GetComponent<CanvasGroup>();

            if (firstPanelButtons[i] == mainButton) {
                mainAt = i;
            }
        }

        // If the main button is not at the start of the array bring it there
        if (mainAt != 0) { 
            MenuButton[] temp = new MenuButton[firstPanelButtons.Length];
            temp[0] = firstPanelButtons[mainAt];

            int at = 0;
            while (at < firstPanelButtons.Length) {
                if (at < mainAt) {
                    temp[at + 1] = firstPanelButtons[at];
                } else if (at == mainAt) {
                    at++;
                } else {
                    temp[at] = firstPanelButtons[at];
                }

                at++;
            }

            firstPanelButtons = temp;
        }

        for (int i = 0; i < secondPanelButtons.Length; i++)
            secondPanelButtons[i].canavasGroup = secondPanelButtons[i].rectTransform.GetComponent<CanvasGroup>();

        // Assign events
        mainButton.rectTransform.GetComponent<Button>().onClick.AddListener(new UnityAction(() => {
            ShowSecondPanel();
        }));

        backButton.rectTransform.GetComponent<Button>().onClick.AddListener(new UnityAction(() => {
            HideSecondPanel();
        }));
    }

    public void ShowSecondPanel() {
        if (isSecondPanelShown) return;

        isSecondPanelShown = true;

        Sequence seq = DOTween.Sequence()
            // Prepare for aniamtion
            .OnStart(new TweenCallback(() => {
                // Set all to starting pos and make it interactable
                for (int i = 0; i < secondPanelButtons.Length; i++) {
                    secondPanelButtons[i].rectTransform.localPosition = new Vector3(secondPanelButtons[i].rectTransform.rect.width / 2f, secondPanelButtons[i].rectTransform.localPosition.y);
                    secondPanelButtons[i].canavasGroup.interactable = true;
                    secondPanelButtons[i].canavasGroup.blocksRaycasts = true;
                }
                
                // Set main buttons to uninteractable
                for (int i = 0; i < firstPanelButtons.Length; i++) {
                    firstPanelButtons[i].canavasGroup.interactable = false;
                    firstPanelButtons[i].canavasGroup.blocksRaycasts = false;
                }
            }))
            

            // Put everything in place - Afetr part
            .OnComplete(new TweenCallback(() => {
                for (int i = 0; i < firstPanelButtons.Length; i++) {
                    firstPanelButtons[i].rectTransform.localPosition = new Vector3(-firstPanelButtons[i].rectTransform.rect.width * 2f, firstPanelButtons[i].rectTransform.localPosition.y);
                }
            }));
        
        // out animation (move and fade)
        for (int i = 0; i < firstPanelButtons.Length; i++) {
            float atTime = animTime * (i * gapBetween);

            seq.Insert(atTime, firstPanelButtons[i].rectTransform.DOLocalMoveX(-firstPanelButtons[i].rectTransform.rect.width / 2f, animTime));
            seq.Insert(atTime, firstPanelButtons[i].canavasGroup.DOFade(0f, animTime));
        }

        // in animations (move and fade)
        for (int i = 0; i < secondPanelButtons.Length; i++) {
            float atTime = (animTime * (((firstPanelButtons.Length - 1 + i) * gapBetween) + inOtDelay));

            seq.Insert(atTime, secondPanelButtons[i].rectTransform.DOLocalMoveX(0, animTime));
            seq.Insert(atTime, secondPanelButtons[i].canavasGroup.DOFade(1f, animTime));
        }
    }

    public void HideSecondPanel() {
        if (!isSecondPanelShown) return;

        isSecondPanelShown = false;

        Sequence seq = DOTween.Sequence()
            // Prepare for aniamtion
            .OnStart(new TweenCallback(() => {
                // Set all to starting pos and make it interactable
                for (int i = 0; i < firstPanelButtons.Length; i++) {
                    firstPanelButtons[i].rectTransform.localPosition = new Vector3(-firstPanelButtons[i].rectTransform.rect.width / 2f, firstPanelButtons[i].rectTransform.localPosition.y);
                    firstPanelButtons[i].canavasGroup.interactable = true;
                    firstPanelButtons[i].canavasGroup.blocksRaycasts = true;
                }

                // Set second buttons to uninteractable
                for (int i = 0; i < secondPanelButtons.Length; i++) {
                    secondPanelButtons[i].canavasGroup.interactable = false;
                    secondPanelButtons[i].canavasGroup.blocksRaycasts = false;
                }
            }))


            // Put everything in place - Afetr part
            .OnComplete(new TweenCallback(() => {
                for (int i = 0; i < secondPanelButtons.Length; i++) {
                    secondPanelButtons[i].rectTransform.localPosition = new Vector3(0, secondPanelButtons[i].rectTransform.localPosition.y);
                }
            }));

        // out animation (move and fade)
        for (int i = 0; i < secondPanelButtons.Length; i++) {
            float atTime = animTime * (i * gapBetween);

            seq.Insert(atTime, secondPanelButtons[i].rectTransform.DOLocalMoveX(secondPanelButtons[i].rectTransform.rect.width / 2f, animTime));
            seq.Insert(atTime, secondPanelButtons[i].canavasGroup.DOFade(0f, animTime));
        }

        // in animations (move and fade)
        for (int i = 0; i < firstPanelButtons.Length; i++) {
            float atTime = (animTime * (((secondPanelButtons.Length - 1 + i) * gapBetween) + inOtDelay));

            seq.Insert(atTime, firstPanelButtons[i].rectTransform.DOLocalMoveX(0, animTime));
            seq.Insert(atTime, firstPanelButtons[i].canavasGroup.DOFade(1f, animTime));
        }
    }
}

[Serializable]
internal struct MenuButton {
    public RectTransform rectTransform;
    [HideInInspector]
    public CanvasGroup canavasGroup;

    public MenuButton(RectTransform rectTransform, CanvasGroup canvasGroup) {
        this.rectTransform = rectTransform;
        this.canavasGroup = canvasGroup;
    }

    public static bool operator ==(MenuButton one, MenuButton two) {
        return one.rectTransform.gameObject.name == two.rectTransform.gameObject.name;
    }

    public static bool operator !=(MenuButton one, MenuButton two) {
        return one.rectTransform.gameObject.name != two.rectTransform.gameObject.name;
    }

    public override bool Equals(object obj) {
        if (obj is MenuButton) {
            return this == (MenuButton) obj;
        }

        return base.Equals(obj);
    }

    public override int GetHashCode() {
        return base.GetHashCode();
    }
}