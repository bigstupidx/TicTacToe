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


	void Start () {
        rect = GetComponent<RectTransform>();
        buttonGroup = FindObjectOfType<EmojiSlotButtonGroupScript>();
	}

    private bool isMoved = false;
    void Update() {
        if (Input.touchCount == 1) { 
            Touch one = Input.GetTouch(0);
            
            if (one.phase == TouchPhase.Moved) isMoved = true;

            // If not clicked on not UI and the drawer is open just close it for convinience
            if (one.phase == TouchPhase.Ended && !isMoved) { 
                if (!GridClickHandler.IsPointerOverUIObject() && isOpen) { 
                    Close();
                    buttonGroup.SetEveryButtonToggleFalse();
                    buttonGroup.MakeEveryButtonNormal();
                }

                isMoved = false;
            }
        }
    }
	
    /// <summary>
    /// Closes the drawer
    /// </summary>
	public void Close() {
        rect.DOAnchorPos(new Vector2(-100, rect.anchoredPosition.y), animationTime);
        rect.DOSizeDelta(new Vector2(0, rect.sizeDelta.y), animationTime);

        isOpen = false;
    }

    /// <summary>
    /// Opens the drawer
    /// </summary>
    public void Open() {
        rect.DOAnchorPos(new Vector2(-980, rect.anchoredPosition.y), animationTime);
        rect.DOSizeDelta(new Vector2(1759, rect.sizeDelta.y), animationTime);

        isOpen = true;
    }

    public void Toggle() {
        if (isOpen) Close();
        else Open();
    }
    
}
