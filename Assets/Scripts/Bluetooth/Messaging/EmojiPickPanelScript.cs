using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

/// <summary>
/// The UI stuff of the emojidrawer
/// </summary>
public class EmojiPickPanelScript : MonoBehaviour {

    private float animationTime = 0.3f;

    private RectTransform rect;
    private EmojiSlotButtonGroupScript buttonGroup;

    private bool isOpen = false;
    public bool IsOpen { get { return isOpen; } }
    private int backButtonStackId;


	void Start () {
        rect = GetComponent<RectTransform>();
        buttonGroup = FindObjectOfType<EmojiSlotButtonGroupScript>();
	}
    
    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            if (!GridClickHandler.IsPointerOverUIObject() && isOpen) {
                Close();
                buttonGroup.SetEveryButtonToggleFalse();
                buttonGroup.MakeEveryButtonNormal();
            }
        }
    }
	
    /// <summary>
    /// Closes the drawer
    /// </summary>
	public void Close() {
        if (!isOpen) return;

        rect.DOAnchorPos(new Vector2(-100, rect.anchoredPosition.y), animationTime);
        rect.DOSizeDelta(new Vector2(0, rect.sizeDelta.y), animationTime);
        buttonGroup.SetEveryButtonToggleFalse();
        buttonGroup.MakeEveryButtonNormal();

        isOpen = false;

        ScaneManager.Instance.RemoveFromBackStack(backButtonStackId);
    }

    /// <summary>
    /// Opens the drawer
    /// </summary>
    public void Open() {
        if (isOpen) return;

        rect.DOAnchorPos(new Vector2(-980, rect.anchoredPosition.y), animationTime);
        rect.DOSizeDelta(new Vector2(1759, rect.sizeDelta.y), animationTime);

        isOpen = true;

        backButtonStackId = ScaneManager.Instance.AddToBackStack(() => {
            Close();
        });
    }

    public void Toggle() {
        if (isOpen) Close();
        else Open();
    }
    
}
