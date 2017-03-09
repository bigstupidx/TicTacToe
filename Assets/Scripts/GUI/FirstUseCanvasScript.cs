using UnityEngine;
using UnityEngine.UI;

public class FirstUseCanvasScript : MonoBehaviour {

    public Image lightLogo, darkLogo;
    
	void Start() {
        if (PreferencesScript.Instance.IsTutorialCompleted()) gameObject.SetActive(false);
	}

    public void StartButtonPressed() {
        ScaneManager.Instance.GoToScene("FirstGame");
    }
}
