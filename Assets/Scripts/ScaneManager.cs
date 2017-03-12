using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.Events;
using DG.Tweening;

public class ScaneManager : Singleton<ScaneManager> {

    private Stack<string> prevScenes = new Stack<string>();

    public delegate void ScreenChange(string from, string to);
    public static event ScreenChange OnScreenChange;

    private List<BackButtonAction> backButtonStack = new List<BackButtonAction>();
    private int idGenerator = 0;
    public bool backButtonEnabled = true;

    private UnityAction afterLoadedAction;
    
    void Start() {
        DontDestroyOnLoad(gameObject);
    }

    void Update() {
        // the back button on android
        if (Input.GetKeyDown(KeyCode.Escape)) {
            // We have an action to perform with back button
            if (backButtonStack.Count > 0) {
                BackButtonAction backButtonAction = backButtonStack[backButtonStack.Count - 1];
                backButtonStack.RemoveAt(backButtonStack.Count - 1);

                Debug.Log("Back button action stack count " + backButtonStack.Count);

                backButtonAction.action.Invoke();
                if (backButtonAction.callback != null) backButtonAction.callback.Invoke();
            } else if(prevScenes.Count > 0 && backButtonEnabled) { // we have a screen to go back to
                // Do something based on which screen we are on
                switch (SceneManager.GetActiveScene().name) {
                    case "Game":
                        GameObject.Find("GameManager").GetComponent<SaveLoadGame>().WriteEverything();
                        break;
                }

                GoToSceneWithoutAddingToStack(prevScenes.Pop());
            }
        }
    }

    /// <summary>
    /// Do something after scene was loaded
    /// </summary>
    public void GoToSceneThenDo(string name, UnityAction action) {
        afterLoadedAction = action;
        GoToScene(name);
    }

	public void GoToScene(string name) {
        prevScenes.Push(SceneManager.GetActiveScene().name);

        GoToSceneWithoutAddingToStack(name);
    }

    private System.Collections.IEnumerator SceneLoaded(string from, string name) {
        // the loading of scene ends the next frame from when it started so we need two of these
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        // go through the back button actions and remove the ones we need to
        for (int i = backButtonStack.Count - 1; i >= 0; i--) {
            if (backButtonStack[i].removeAtSceneChange) {
                backButtonStack.RemoveAt(i);
            }
        }

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

    public void GoToSceneWithoutAddingToStack(string name) {
        string from = SceneManager.GetActiveScene().name;

        SceneManager.LoadScene(name, LoadSceneMode.Single);

        StartCoroutine(SceneLoaded(from, name));
    }

    /// <summary>
    /// Add to stack of back button. Returns the id with which it canbe removed from the stack
    /// </summary>
    public int AddToBackStack(UnityAction action, bool removeAtSceneChange = true) {
        return AddToBackStackWithCallback(action, null, removeAtSceneChange);
    }

    public int AddToBackStackWithCallback(UnityAction action, UnityAction callback, bool removeAtSceneChange = true) {
        idGenerator++;

        backButtonStack.Add(new BackButtonAction(idGenerator, action, callback, removeAtSceneChange));

        return idGenerator;
    }

    /// <summary>
    /// Remove the backbuttonaction from stack with the id
    /// </summary>
    public void RemoveFromBackStack(int id) {
        for (int i = backButtonStack.Count - 1; i >= 0; i--) {
            if (backButtonStack[i].id == id) { 
                backButtonStack.RemoveAt(i);
                return;
            }
        }
    }

}

internal struct BackButtonAction {
    public UnityAction action;
    public UnityAction callback;
    public int id;
    public bool removeAtSceneChange;

    public BackButtonAction(int id, UnityAction action, bool removeAtSceneChange) {
        this.action = action;
        this.id = id;
        callback = null;
        this.removeAtSceneChange = removeAtSceneChange;
    }

    public BackButtonAction(int id, UnityAction action, UnityAction callback, bool removeAtSceneChange) {
        this.action = action;
        this.id = id;
        this.callback = callback;
        this.removeAtSceneChange = removeAtSceneChange;
    }
}
