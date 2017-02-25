using UnityEngine;
using DG.Tweening;
using System;

public class MenuButtonPanelScript : MonoBehaviour {

    float animTime = 0.5f;

    [SerializeField]
    private MenuButton localButton;
    [SerializeField]
    private MenuButton onlineButton;
    [SerializeField]
    private MenuButton bluetoothButton;
    [SerializeField]
    private MenuButton googlePlayButton;
    [SerializeField]
    private MenuButton onlineBackButton;
    [SerializeField]
    private MenuButton localMultiButton;
    [SerializeField]
    private MenuButton localAIButton;
    [SerializeField]
    private MenuButton localBackButton;

    private bool isLocalForward = false;

    void Awake() {
        // If we haven't completed the tutorial don't brin in the menus
        if (!PreferencesScript.Instance.IsTutorialCompleted())
            GetComponent<DOTweenAnimation>().DOKill();
    }
    
	void Start () {
        localAIButton.canavasGroup = localButton.rectTransform.gameObject.GetComponent<CanvasGroup>();
        onlineButton.canavasGroup = onlineButton.rectTransform.gameObject.GetComponent<CanvasGroup>();
        bluetoothButton.canavasGroup = bluetoothButton.rectTransform.gameObject.GetComponent<CanvasGroup>();
        googlePlayButton.canavasGroup = googlePlayButton.rectTransform.gameObject.GetComponent<CanvasGroup>();
        onlineBackButton.canavasGroup = onlineBackButton.rectTransform.gameObject.GetComponent<CanvasGroup>();
        localMultiButton.canavasGroup = localMultiButton.rectTransform.gameObject.GetComponent<CanvasGroup>();
        localAIButton.canavasGroup = localAIButton.rectTransform.gameObject.GetComponent<CanvasGroup>();
        localBackButton.canavasGroup = localBackButton.rectTransform.gameObject.GetComponent<CanvasGroup>();
    }

    public void LocalButtonPressed() {
        if (isLocalForward) return;

        DOTween.Sequence()
            // Prepare for aniamtion
            .OnStart(new TweenCallback(() => {
                localMultiButton.rectTransform.localPosition = new Vector3(localMultiButton.rectTransform.rect.width / 2f, localMultiButton.rectTransform.localPosition.y);
                localAIButton.rectTransform.localPosition = new Vector3(localAIButton.rectTransform.rect.width / 2f, localAIButton.rectTransform.localPosition.y);
                localBackButton.rectTransform.localPosition = new Vector3(localBackButton.rectTransform.rect.width / 2f, localBackButton.rectTransform.localPosition.y);

                bluetoothButton.canavasGroup.interactable = false;
                localButton.canavasGroup.interactable = false;

                localMultiButton.canavasGroup.interactable = true;
                localAIButton.canavasGroup.interactable = true;
                localBackButton.canavasGroup.interactable = true;

                // if it's not here it will affect how animation looks (it will be disabled)
                localBackButton.canavasGroup.interactable = true;
            }))

            // out animation
            .Insert(0f, localButton.rectTransform.DOLocalMoveX(-localButton.rectTransform.rect.width / 2f, animTime))
            .Insert(0f, localButton.canavasGroup.DOFade(0f, animTime))
            .Insert(animTime * 0.2f, bluetoothButton.rectTransform.DOLocalMoveX(-bluetoothButton.rectTransform.rect.width / 2f, animTime))
            .Insert(animTime * 0.2f, bluetoothButton.canavasGroup.DOFade(0f, animTime))

            // in animation
            .Insert(animTime * 0.5f, localMultiButton.rectTransform.DOLocalMoveX(0, animTime))
            .Insert(animTime * 0.5f, localMultiButton.canavasGroup.DOFade(1f, animTime))
            .Insert(animTime * 0.7f, localAIButton.rectTransform.DOLocalMoveX(0, animTime))
            .Insert(animTime * 0.7f, localAIButton.canavasGroup.DOFade(1f, animTime))
            .Insert(animTime * 0.8f, localBackButton.rectTransform.DOLocalMoveX(0, animTime))
            .Insert(animTime * 0.8f, localBackButton.canavasGroup.DOFade(1f, animTime))

            // Put everything in place - Afetr part
            .OnComplete(new TweenCallback(() => {
                localButton.rectTransform.localPosition = new Vector3(-localButton.rectTransform.rect.width * 2f, localButton.rectTransform.localPosition.y);
                bluetoothButton.rectTransform.localPosition = new Vector3(-bluetoothButton.rectTransform.rect.width * 2f, bluetoothButton.rectTransform.localPosition.y);
                isLocalForward = true;
            }));
    }

    public void LocalBackButtonPressed() {
        if (!isLocalForward) return;

        DOTween.Sequence()
            // Prepare for aniamtion
            .OnStart(new TweenCallback(() => {
                localButton.rectTransform.localPosition = new Vector3(-localButton.rectTransform.rect.width / 2f, localButton.rectTransform.localPosition.y);
                bluetoothButton.rectTransform.localPosition = new Vector3(-bluetoothButton.rectTransform.rect.width / 2f, bluetoothButton.rectTransform.localPosition.y);
                
                bluetoothButton.canavasGroup.interactable = true;
                localButton.canavasGroup.interactable = true;

                localMultiButton.canavasGroup.interactable = false;
                localAIButton.canavasGroup.interactable = false;
                localBackButton.canavasGroup.interactable = false;
            }))

            // out animation
            .Insert(0, localMultiButton.rectTransform.DOLocalMoveX(localMultiButton.rectTransform.rect.width / 2f, animTime))
            .Insert(0, localMultiButton.canavasGroup.DOFade(0f, animTime))
            .Insert(animTime * 0.2f, localAIButton.rectTransform.DOLocalMoveX(localAIButton.rectTransform.rect.width / 2f, animTime))
            .Insert(animTime * 0.2f, localAIButton.canavasGroup.DOFade(0f, animTime))
            .Insert(animTime * 0.3f, localBackButton.rectTransform.DOLocalMoveX(localBackButton.rectTransform.rect.width / 2f, animTime))
            .Insert(animTime * 0.3f, localBackButton.canavasGroup.DOFade(0f, animTime))

            // in animation
            .Insert(animTime * 0.5f, localButton.rectTransform.DOLocalMoveX(0, animTime))
            .Insert(animTime * 0.5f, localButton.canavasGroup.DOFade(1f, animTime))
            .Insert(animTime * 0.7f, bluetoothButton.rectTransform.DOLocalMoveX(0, animTime))
            .Insert(animTime * 0.7f, bluetoothButton.canavasGroup.DOFade(1f, animTime))

            // Put everything in place - Afetr part
            .OnComplete(new TweenCallback(() => {
                localMultiButton.rectTransform.localPosition = new Vector3(0, localMultiButton.rectTransform.localPosition.y);
                localAIButton.rectTransform.localPosition = new Vector3(0, localAIButton.rectTransform.localPosition.y);
                localBackButton.rectTransform.localPosition = new Vector3(0, localBackButton.rectTransform.localPosition.y);
                localBackButton.canavasGroup.interactable = false;
                isLocalForward = false;
            }));
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
}