using UnityEngine;

public class GPAchievementsButton : MonoBehaviour {

    void Start() {
        // The local user is not logged in -> no achievements
        if (!Social.localUser.authenticated) {
            gameObject.SetActive(false);
        }
    }

    public void OpenAchievements() {
        Social.ShowAchievementsUI();
    }
}
