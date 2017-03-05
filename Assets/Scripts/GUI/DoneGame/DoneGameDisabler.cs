using UnityEngine;
using UnityEngine.SceneManagement;

public class DoneGameDisabler : MonoBehaviour {

    public string[] scenesOnWhichToShow = new string[] {
        "Menu", "BluetoothConnect", "GooglePlayConnectScreen"
    };

    void Start() {
        SceneManager.activeSceneChanged += SceneChanged;
    }

    private void SceneChanged(Scene sceneFrom, Scene sceneTo) {
        bool areWe = AreWeOnScenesOnWhichToShow(sceneTo.name);

        // We are an a scene where it should be active and it is not so activate it
        if (areWe && !gameObject.activeSelf) {
            gameObject.SetActive(true);
        } else if (!areWe && gameObject.activeSelf) { // We are on a scene on which we don't need to show it but it is still shown
            gameObject.SetActive(false);
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
