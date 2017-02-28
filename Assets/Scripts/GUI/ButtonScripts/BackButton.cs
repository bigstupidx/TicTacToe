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

            ScaneManager.Instance.GoToPreviousSene();

            // Do something based on which screen we backed from
            switch (SceneManager.GetActiveScene().name) {
                case "Game":
                    GameObject.Find("GameManager").GetComponent<SaveLoadGame>().WriteEverything();
                    break;
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
