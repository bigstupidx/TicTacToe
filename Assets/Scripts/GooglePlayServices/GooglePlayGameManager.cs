
using GooglePlayGames;
using UnityEngine;
using DG.Tweening;
using GooglePlayGames.BasicApi.Multiplayer;
using System;

public class GooglePlayGameManager : MonoBehaviour, RealTimeMultiplayerListener {

    public RectTransform signInPanel;

    private bool isServer = false;
    
	void Start () {
        DontDestroyOnLoad(gameObject);

		// If we are already logged in disable signInPanel
        if (Social.localUser.authenticated) {
            signInPanel.gameObject.SetActive(false);
        }
	}

    public void SignInButtonPressed() {
        Social.localUser.Authenticate((bool success) => {
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
        PlayGamesPlatform.Instance.RealTime.CreateWithInvitationScreen(1, 1, 0, this);
        isServer = true;
    }

    public void OnRoomSetupProgress(float percent) {
        
    }

    public void OnRoomConnected(bool success) {
        if (!success) {
            isServer = false;
            PopupManager.Instance.PopUp("Failed to make room!", "OK");
        } else {
            if (isServer) { 
                ScaneManager.Instance.GoToScene("GooglePlayGameServer");
            } else {
                ScaneManager.Instance.GoToScene("GooglePlayGameClient");
            }
        }
    }

    /// <summary>
    /// Called when the the self player leaves
    /// </summary>
    public void OnLeftRoom() {
        isServer = false;
    }

    /// <summary>
    /// Called when the other player leaves
    /// </summary>
    public void OnParticipantLeft(Participant participant) {
        // There are only two players so surely it's the other player
        PopupManager.Instance.PopUp(new PopUpOneButton("Other player left!\nGoing back to the main menu.", "OK").SetButtonPressAction(() => {
            ScaneManager.Instance.GoToSceneWithErase("Menu");
        }));
    }

    public void OnPeersConnected(string[] participantIds) {
        throw new NotImplementedException();
    }

    public void OnPeersDisconnected(string[] participantIds) {
        throw new NotImplementedException();
    }

    public void OnRealTimeMessageReceived(bool isReliable, string senderId, byte[] data) {
        throw new NotImplementedException();
    }
}
