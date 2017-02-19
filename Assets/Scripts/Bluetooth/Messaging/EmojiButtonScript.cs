using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Attached to every button on the emoji drawer
/// </summary>
public class EmojiButtonScript : MonoBehaviour, IPointerClickHandler {

    private EmojiPickPanelContentScript panelContentScript;
    public bool isEnabled = false;
    
	void Start () {
        panelContentScript = transform.parent.GetComponent<EmojiPickPanelContentScript>();
	}

    public void OnPointerClick(PointerEventData eventData) {
        if (isEnabled) return;

        panelContentScript.ButtonPressed(gameObject.name);
    }
}
