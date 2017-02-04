using UnityEngine;

public class LightDarkModeButtonScript : MonoBehaviour {

    private PreferencesScript preferences;

	void Start () {
        preferences = FindObjectOfType<PreferencesScript>();

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
