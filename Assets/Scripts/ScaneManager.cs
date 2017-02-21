using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class ScaneManager : Singleton<ScaneManager> {

    private Stack<string> prevScenes = new Stack<string>();
    
    void Start() {
        DontDestroyOnLoad(gameObject);
    }

	public void GoToScene(string name) {
        prevScenes.Push(SceneManager.GetActiveScene().name);
        SceneManager.LoadScene(name);
    }

    /// <summary>
    /// Erases history then goes to scene
    /// </summary>
    /// <param name="name"></param>
    public void GoToSceneWithErase(string name) {
        prevScenes.Clear();
        GoToScene(name);
    }

    public void GoToPreviousSene() {
        SceneManager.LoadScene(prevScenes.Pop());
    }

}
