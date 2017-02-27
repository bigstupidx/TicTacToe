
using GooglePlayGames;
using UnityEngine;
using DG.Tweening;
using GooglePlayGames.BasicApi.Multiplayer;

public class GooglePlayGameManager : MonoBehaviour {

    public RectTransform signInPanel;
    
	void Start () {
		// If we are already logged in disable signInPanel
        if (Social.localUser.authenticated) {
            signInPanel.gameObject.SetActive(false);
        }
	}

    public void SignInButtonPressed() {
        Social.localUser.Authenticate((bool success) => {
            Debug.Log("Login " + success);
            if (success) {
                PreferencesScript.Instance.GPFromNowCanAutoLogin();

                signInPanel.GetComponent<CanvasGroup>().DOFade(0f, 0.5f)
                .OnComplete(new TweenCallback(() => {
                    signInPanel.gameObject.SetActive(false);
                }));
            } else {
                PopupManager.Instance.PopUp("Couldn't log in!", "Ok");
            }
        });
    }
	
	public void StartWithInvitation() {
        PlayGamesPlatform.Instance.TurnBased.CreateWithInvitationScreen(1, 1, 0, OnMatchStarted);
    }

    private void OnMatchStarted(bool success, TurnBasedMatch match) {
        if (!success) {
            PopupManager.Instance.PopUp("Match couldn't be started!", "Ok");
            return;
        }
    }
}
