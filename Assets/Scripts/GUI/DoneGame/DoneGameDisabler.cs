using UnityEngine;
using UnityEngine.SceneManagement;

public class DoneGameDisabler : MonoBehaviour {

    public Vector3 camOnLastScenePosition;

    public string[] scenesOnWhichToShow = new string[] {
        "Menu", "BluetoothConnect", "GooglePlayConnectScreen"
    };

    void Start() {
        SceneManager.activeSceneChanged += SceneChanged;
        ScaneManager.OnScreenAboutToChangeEvent += OnSceneAboutToChange;
    }

    private void OnSceneAboutToChange(string from, string to) {
        camOnLastScenePosition = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z);
    }

    private void SceneChanged(Scene sceneFrom, Scene sceneTo) {
        bool areWe = AreWeOnScenesOnWhichToShow(sceneTo.name);

        // We are an a scene where it should be active and it is not so activate it
        if (areWe && !gameObject.activeSelf) {
            gameObject.SetActive(true);
        } else if (!areWe && gameObject.activeSelf) { // We are on a scene on which we don't need to show it but it is still shown
            gameObject.SetActive(false);
        }

        if (gameObject.activeSelf) {
            Camera.main.transform.position = camOnLastScenePosition;
        }

    }

    /// <summary>
    /// Are we on scenes on which we need to show the donegames
    /// </summary>
    /// <returns></returns>
    private bool AreWeOnScenesOnWhichToShow(string currentScene) {
        foreach (string s in scenesOnWhichToShow)
            if (s == currentScene) return true;

        return false;
    }
}
