using UnityEngine;

public class LightDarkModeButtonScript : MonoBehaviour {

    private PreferencesScript preferences;

    void Awake() {
        preferences = FindObjectOfType<PreferencesScript>();

        // If we haven't completed tutorial just disable this because it would be too much for the player
        if (!preferences.IsTutorialCompleted())
            gameObject.SetActive(false);
    }

	void Start () {
        // If the starting mode is dark we need to flip the button
        if (preferences.currentMode == PreferencesScript.ColorMode.DARK) {
            GetComponent<Animator>().SetTrigger("FlipWithoutAnimation");
        }
	}
	
    public void ChangeToDarkMode() {
        preferences.ChangeToColorMode(PreferencesScript.ColorMode.DARK);
    }

    public void ChangeToLightMode() {
        preferences.ChangeToColorMode(PreferencesScript.ColorMode.LIGHT);
    }
	
}
