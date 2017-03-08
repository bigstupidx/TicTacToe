using UnityEngine;

public class LightDarkModeButtonScript : MonoBehaviour {

    void Awake() {
        // If we haven't completed tutorial just disable this because it would be too much for the player
        if (!PreferencesScript.Instance.IsTutorialCompleted())
            gameObject.SetActive(false);
    }

	void Start () {
        // If the starting mode is dark we need to flip the button
        if (PreferencesScript.Instance.currentMode == PreferencesScript.ColorMode.DARK) {
            GetComponent<Animator>().SetTrigger("FlipWithoutAnimation");
        }
	}
	
    public void ChangeToDarkMode() {
        PreferencesScript.Instance.ChangeToColorMode(PreferencesScript.ColorMode.DARK);
    }

    public void ChangeToLightMode() {
        PreferencesScript.Instance.ChangeToColorMode(PreferencesScript.ColorMode.LIGHT);
    }
	
}
