using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BackButton : MonoBehaviour {

    private Button buttonScript;
    public bool disabled = false;

    void Start() {
        buttonScript = GetComponent<Button>();

        buttonScript.onClick.AddListener(() => {
            if (disabled) return;
            

            // Do something based on which screen we backed from
            switch (SceneManager.GetActiveScene().name) {
                
            }
        });
    }

    void Update() {
        // Works on Android as back button
        if (Input.GetKeyDown(KeyCode.Escape)) {
            buttonScript.onClick.Invoke();
        }
    }
	
}
