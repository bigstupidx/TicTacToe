using UnityEngine;

public class DisableIfFirstGame : MonoBehaviour {
	void Awake () {
        // If we haven't completed tutorial just disable this because it would be too much for the player
        if (!PreferencesScript.Instance.IsTutorialCompleted())
            gameObject.SetActive(false);
    }
}
