using UnityEngine;

public class GPAchievementsButton : MonoBehaviour {

    void Start() {
        // The local user is not logged in -> no achievements
        if (!Social.localUser.authenticated) {
            // if auto log in has been enabled.
            if (PreferencesScript.Instance.GPCanAutoLogin())
                StartCoroutine(CheckForAuthenticated());

            gameObject.SetActive(false);
        }
    }

    private System.Collections.IEnumerator CheckForAuthenticated() {
        yield return new WaitWhile(() => Social.localUser.authenticated);

        gameObject.SetActive(true);
    }

    public void OpenAchievements() {
        Social.ShowAchievementsUI();
    }
}
