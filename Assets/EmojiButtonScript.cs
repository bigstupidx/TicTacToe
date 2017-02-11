﻿using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Attached to every button on the emoji drawer
/// </summary>
public class EmojiButtonScript : MonoBehaviour, IPointerClickHandler {

    private EmojiPickPanelContentScript panelContentScript;
    public bool isEnabled = false;

    private RectTransform rectTransform;
    
	void Start () {
        panelContentScript = transform.parent.GetComponent<EmojiPickPanelContentScript>();
        rectTransform = GetComponent<RectTransform>();
	}

    public void OnPointerClick(PointerEventData eventData) {
        if (isEnabled) return;

        panelContentScript.ButtonPressed(gameObject.name);
    }
}
