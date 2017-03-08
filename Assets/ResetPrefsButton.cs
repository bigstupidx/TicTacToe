using UnityEngine;
using UnityEngine.EventSystems;

public class ResetPrefsButton : MonoBehaviour, IPointerClickHandler {
    public void OnPointerClick(PointerEventData eventData) {
        PreferencesScript.Instance.ResetPreferences();
    }
}
