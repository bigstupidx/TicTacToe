using UnityEngine;
using UnityEngine.EventSystems;

public class ResetPrefsButton : MonoBehaviour, IPointerClickHandler {
    public void OnPointerClick(PointerEventData eventData) {
        if (gameObject.name == "ResetPrefButton")
            PreferencesScript.Instance.ResetForDebugPrefs();
        else
            PreferencesScript.Instance.ResetPreferences();
    }
}
