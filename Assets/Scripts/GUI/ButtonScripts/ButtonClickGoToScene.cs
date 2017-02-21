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
        GetComponent<Button>().onClick.AddListener(() => {
            ScaneManager.Instance.GoToScene(sceneName);
        });
    }

}
