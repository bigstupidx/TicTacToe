using UnityEngine;

public class SpawnManagers : MonoBehaviour {

    public GameObject sceneManagerPrefab;
    public string sceneManagerName = "SceneManager";

    public GameObject popupManagerPrefab;
    public string popupManagerName = "PopupManager";

	void Start () {
        // Needed for when we return to menu dont make another one of the managers
        if (GameObject.Find(sceneManagerName) == null) { 
            GameObject sceneManager = Instantiate(sceneManagerPrefab) as GameObject;
            sceneManager.name = sceneManagerName;
        }

        if (GameObject.Find(popupManagerName) == null) {
            GameObject popupManager = Instantiate(popupManagerPrefab) as GameObject;
            popupManager.name = popupManagerName;
        }

        Destroy(gameObject);
	}
}
