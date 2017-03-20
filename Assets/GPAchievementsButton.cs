using UnityEngine;

public class GPAchievementsButton : MonoBehaviour {

    private CanvasGroup canvasGroup;

    void Start() {
        canvasGroup = GetComponent<CanvasGroup>();

        // The local user is not logged in -> no achievements
        if (!Social.localUser.authenticated) {
            // if auto log in has been enabled.
            if (PreferencesScript.Instance.GPCanAutoLogin())
                StartCoroutine(CheckForAuthenticated());

            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
        }
    }

    private System.Collections.IEnumerator CheckForAuthenticated() {
        yield return new WaitWhile(() => Social.localUser.authenticated);

        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
    }

    public void OpenAchievements() {
        Social.ShowAchievementsUI();
    }
}
