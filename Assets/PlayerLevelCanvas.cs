using UnityEngine;

public class PlayerLevelCanvas : MonoBehaviour {

    private Canvas canvas;

	void Start() {
        canvas = GetComponent<Canvas>();

        ScaneManager.OnScreenChange += OnScreenChange;
	}

    private void OnScreenChange(string from, string to) {
        canvas.worldCamera = Camera.main;
    }
}
