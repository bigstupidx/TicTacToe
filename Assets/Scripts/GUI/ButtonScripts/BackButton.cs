using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BackButton : MonoBehaviour {

    private Button buttonScript;

    void Start() {
        buttonScript = GetComponent<Button>();

        buttonScript.onClick.AddListener(() => {
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
