using UnityEngine;

public class PlayerLevelCanvas : MonoBehaviour {

    private Canvas canvas;

	void Start() {
        if (FindObjectsOfType<PlayerLevelCanvas>().Length >= 2) {
            DestroyImmediate(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        canvas = GetComponent<Canvas>();

        ScaneManager.OnScreenChange += OnScreenChange;
	}

    private void OnScreenChange(string from, string to) {
        canvas.worldCamera = Camera.main;
    }
}
