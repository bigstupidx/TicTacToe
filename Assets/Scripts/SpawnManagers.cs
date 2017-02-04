using UnityEngine;

public class SpawnManagers : MonoBehaviour {

    public GameObject sceneManagerPrefab;
    public string sceneManagerName = "SceneManager";

    public GameObject popupManagerPrefab;
    public string popupManagerName = "PopupManager";

    public GameObject preferencesManagerPrefab;
    public string preferencesManagerName = "PreferencesManager";

	void Awake () {
        // Needed for when we return to menu dont make another one of the managers
        if (GameObject.Find(sceneManagerName) == null) { 
            GameObject sceneManager = Instantiate(sceneManagerPrefab) as GameObject;
            sceneManager.name = sceneManagerName;
        }

        if (GameObject.Find(popupManagerName) == null) {
            GameObject popupManager = Instantiate(popupManagerPrefab) as GameObject;
            popupManager.name = popupManagerName;
        }

        if (GameObject.Find(preferencesManagerName) == null) {
            GameObject preferencesManager = Instantiate(preferencesManagerPrefab) as GameObject;
            preferencesManager.name = preferencesManagerName;
        }

        Destroy(gameObject);
	}
}
