using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.Events;

public class ScaneManager : Singleton<ScaneManager> {

    private Stack<string> prevScenes = new Stack<string>();

    public delegate void ScreenChange(string from, string to);
    public static event ScreenChange OnScreenChange;

    private UnityAction afterLoadedAction;
    
    void Start() {
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Do something after scene was loaded
    /// </summary>
    public void GoToSceneThenDo(string name, UnityAction action) {
        afterLoadedAction = action;
        GoToScene(name);
    }

	public void GoToScene(string name) {
        string from = SceneManager.GetActiveScene().name;
        prevScenes.Push(from);
        SceneManager.LoadScene(name);

        StartCoroutine(SceneLoaded(from, name));
    }

    private System.Collections.IEnumerator SceneLoaded(string from, string name) {
        // the loading of scene ends the next frame from when it started so we need two of these
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        if (afterLoadedAction != null) {
            afterLoadedAction.Invoke();
            afterLoadedAction = null;
        }
        
        if (OnScreenChange != null) OnScreenChange(from, name);
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
        string to = prevScenes.Pop(), from = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(to);

        StartCoroutine(SceneLoaded(from, to));
    }

}
