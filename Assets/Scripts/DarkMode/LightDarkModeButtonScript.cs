using UnityEngine;

public class LightDarkModeButtonScript : MonoBehaviour {

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
