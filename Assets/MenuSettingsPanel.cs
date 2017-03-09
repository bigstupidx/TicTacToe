using UnityEngine;
using DG.Tweening;

public class MenuSettingsPanel : MonoBehaviour {

    public RectTransform mainMenuPanel;
    
    private RectTransform rectTransform;
    private float animTime = 0.3f;
    private bool isOpen = false;

	void Start () {
        rectTransform = GetComponent<RectTransform>();
    }

    public void ToggleSettingsPanel() {
        if (isOpen) {
            CloseSettingsPanel();
        } else {
            OpenSettingPanel();
        }
    }
	
	public void OpenSettingPanel() {
        if (isOpen) return;

        isOpen = true;

        mainMenuPanel.DOAnchorPosX(mainMenuPanel.anchoredPosition.x - rectTransform.rect.width, animTime);
        rectTransform.DOAnchorPosX(rectTransform.anchoredPosition.x - rectTransform.rect.width, animTime);
    }

    public void CloseSettingsPanel() {
        if (!isOpen) return;

        isOpen = false;

        mainMenuPanel.DOAnchorPosX(mainMenuPanel.anchoredPosition.x + rectTransform.rect.width, animTime);
        rectTransform.DOAnchorPosX(rectTransform.anchoredPosition.x + rectTransform.rect.width, animTime);
    }
}
