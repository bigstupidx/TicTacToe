using UnityEngine;
using DG.Tweening;

public class MenuButtonPanelScript : MonoBehaviour {

    float animTime = 0.5f;

    private MenuButton localButton;
    private MenuButton bluetoothButton;
    private MenuButton localMultiButton;
    private MenuButton localAIButton;
    private MenuButton localBackButton;

    private bool isLocalForward = false;
    
	void Start () {
        Transform child = transform.GetChild(3);
        localButton = new MenuButton(child.GetComponent<RectTransform>(), child.GetComponent<CanvasGroup>());

        child = transform.GetChild(4);
        bluetoothButton = new MenuButton(child.GetComponent<RectTransform>(), child.GetComponent<CanvasGroup>());

        child = transform.GetChild(0);
        localMultiButton = new MenuButton(child.GetComponent<RectTransform>(), child.GetComponent<CanvasGroup>());

        child = transform.GetChild(1);
        localAIButton = new MenuButton(child.GetComponent<RectTransform>(), child.GetComponent<CanvasGroup>());

        child = transform.GetChild(2);
        localBackButton = new MenuButton(child.GetComponent<RectTransform>(), child.GetComponent<CanvasGroup>());
    }

    public void LocalButtonPressed() {
        if (isLocalForward) return;

        DOTween.Sequence()
            // Prepare for aniamtion
            .OnStart(new TweenCallback(() => {
                localMultiButton.rectTransform.localPosition = new Vector3(localMultiButton.rectTransform.rect.width / 2f, localMultiButton.rectTransform.localPosition.y);
                localAIButton.rectTransform.localPosition = new Vector3(localAIButton.rectTransform.rect.width / 2f, localAIButton.rectTransform.localPosition.y);
                localBackButton.rectTransform.localPosition = new Vector3(localBackButton.rectTransform.rect.width / 2f, localBackButton.rectTransform.localPosition.y);
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

internal struct MenuButton {
    public RectTransform rectTransform;
    public CanvasGroup canavasGroup;

    public MenuButton(RectTransform rectTransform, CanvasGroup canvasGroup) {
        this.rectTransform = rectTransform;
        this.canavasGroup = canvasGroup;
    }
}