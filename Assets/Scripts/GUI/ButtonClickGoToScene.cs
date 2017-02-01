using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Assigns an event to the button of this GameObject
/// The event goes to a screen
/// </summary>
[RequireComponent (typeof (Button))]
public class ButtonClickGoToScene : MonoBehaviour {

    public string sceneName;

    void Start() {
        InvokeRepeating("TryAssignEvent", 0f, 0.05f);
    }

    private void TryAssignEvent() {
        GameObject sceneManager = GameObject.Find("SceneManager");

        // If the scene manager is already here we can finally assign click scipt and stop this coroutine
        if (sceneManager != null) {
            GetComponent<Button>().onClick.AddListener(() => { sceneManager.GetComponent<ScaneManager>().GoToScene(sceneName); });
            
            CancelInvoke();
        }
    }

}
