using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Assigns an event to the button of this GameObject
/// The event goes to a screen
/// </summary>
[RequireComponent (typeof (Button))]
public class ButtonClickGoToScene : MonoBehaviour {

    [Tooltip("Whether to delete the scene queue in SceneManager. If it gets deleted you won't be able to go back to them.")]
    public bool DeleteSceneQueue = false;

    public string sceneName;

    void Start() {
        GetComponent<Button>().onClick.AddListener(() => {
            if (DeleteSceneQueue) {
                ScaneManager.Instance.GoToSceneWithErase("Menu");
            } else { 
                ScaneManager.Instance.GoToScene(sceneName);
            }
        });
    }

}
