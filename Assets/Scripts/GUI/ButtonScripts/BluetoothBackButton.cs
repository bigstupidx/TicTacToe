using UnityEngine;
using UnityEngine.UI;

public class BluetoothBackButton : MonoBehaviour {

    private Button buttonScript;

    void Start() {
        buttonScript = GetComponent<Button>();

        buttonScript.onClick.AddListener(() => {
            // Disable bluetooth
            Bluetooth.Instance().Stop();
            Bluetooth.Instance().DisableBluetooth();

            GameObject.Find("SceneManager").GetComponent<ScaneManager>().GoToSceneWithErase("Menu");
        });
    }

    void Update() {
        // Works on Android as back button
        if (Input.GetKeyDown(KeyCode.Escape)) {
            buttonScript.onClick.Invoke();
        }
    }

}
