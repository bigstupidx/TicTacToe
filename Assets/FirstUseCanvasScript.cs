using UnityEngine;
using UnityEngine.UI;

public class FirstUseCanvasScript : MonoBehaviour {

    private float animTime = 0.3f;

    public Image lightLogo, darkLogo;
    
	void Start() {
        if (FindObjectOfType<PreferencesScript>().IsTutorialCompleted()) gameObject.SetActive(false);
	}

    public void StartButtonPressed() {
        FindObjectOfType<ScaneManager>().GoToScene("FirstGame");
    }
}
