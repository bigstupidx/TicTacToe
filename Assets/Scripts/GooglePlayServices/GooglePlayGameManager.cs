using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using GooglePlayGames;
using GooglePlayGames.BasicApi.Multiplayer;
using GooglePlayGames.BasicApi.Nearby;
using TMPro;

public class GooglePlayGameManager : MonoBehaviour, RealTimeMultiplayerListener {
    public static float JUMP_TIME_PER_ONE = 0.05f;

    private const string CLIENT_SCENE = "GooglePlayGameClient";
    private const string SERVER_SCENE = "GooglePlayGameServer";

    public RectTransform signInPanel;
    public TextMeshProUGUI usernameText;

    private bool roomMakingShown = false;
    public Slider roomMakingSlider;
    public CanvasGroup roomMakingCanvasGroup;

    private bool isServer = false;
    private bool isQuickMatch = false;

    // ******************************************CLIENTSTUFF************************************************
    private ClientCellStorage clientCellStorage;
    private ClientCellStorage ClientCellStorage {
        get {
            if (clientCellStorage == null) clientCellStorage = FindObjectOfType<ClientCellStorage>();

            return clientCellStorage;
        }
    }

    private ScoringScript scoring;
    private ScoringScript Scoring {
        get {
            if (scoring == null) scoring = FindObjectOfType<ScoringScript>();

            return scoring;
        }
    }

    private BluetoothClientUIInScript clientUIInScript;
    private BluetoothClientUIInScript ClientUIInScript {
        get {
            if (clientUIInScript == null) clientUIInScript = GameObject.Find("Canvas").GetComponent<BluetoothClientUIInScript>();

            return clientUIInScript;
        }
    }


    // ******************************************SERVERSTUFF************************************************

    private GPTTTGameLogic gameLogic;
    private GPTTTGameLogic GameLogic {
        get {
            if (gameLogic == null) gameLogic = FindObjectOfType<GPTTTGameLogic>();

            return gameLogic;
        }
    }

    void Start () {
        DontDestroyOnLoad(gameObject);

		// If we are already logged in disable signInPanel
        if (Social.localUser.authenticated) {
            signInPanel.gameObject.SetActive(false);
        }

        ScaneManager.OnScreenChange += OnSreenChanged;

        // set ui text name
        usernameText.text = PlayGamesPlatform.Instance.GetUserDisplayName();
    }

    /// <summary>
    /// Destroys this gameobject when we get to a scene which does not belong to the googplay group
    /// </summary>
    private void OnSreenChanged(string from, string to) {
        if (to != CLIENT_SCENE && to != SERVER_SCENE && to != "GooglePlayConnectScreen") {
            Debug.Log("GPGAMEMANGER DESTROYEd");
            PlayGamesPlatform.Instance.RealTime.LeaveRoom();

            ScaneManager.OnScreenChange -= OnSreenChanged;

            Destroy(gameObject);
        }
    }

    /// <summary>
    /// The sign in button was pressed so sign in
    /// </summary>
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
	
    /// <summary>
    /// Starts the game as server with inivtation
    /// </summary>
	public void StartWithInvitation() {
        PlayGamesPlatform.Instance.RealTime.CreateWithInvitationScreen(1, 1, 0, this);
        isServer = true;
        isQuickMatch = false;

        ShowRoomMakingPanel();
    }

    public void StartWithQuickMatch() {
        PlayGamesPlatform.Instance.RealTime.CreateQuickGame(1, 1, 0, this);
        isQuickMatch = true;

        ShowRoomMakingPanel();
    }

    private float roomMakingShowTime = 0.4f;
    public void ShowRoomMakingPanel() {
        // make interactable
        roomMakingShown = true;
        roomMakingCanvasGroup.interactable = true;
        roomMakingCanvasGroup.blocksRaycasts = true;

        // do tweening
        roomMakingCanvasGroup.DOFade(1f, roomMakingShowTime);
        roomMakingCanvasGroup.transform.GetChild(0).GetComponent<RectTransform>().DOScale(1f, roomMakingShowTime).SetEase(Ease.OutBack);
    }
    public void HideRoomMakingPanel() {
        roomMakingShown = false;

        // do tweening and then make uninteractable so it doesn't detect click if happened during it was shown
        roomMakingCanvasGroup.DOFade(0f, roomMakingShowTime).OnComplete(new TweenCallback(() => {
            roomMakingCanvasGroup.interactable = false;
            roomMakingCanvasGroup.blocksRaycasts = false;
        }));
        roomMakingCanvasGroup.transform.GetChild(0).GetComponent<RectTransform>().DOScale(0f, roomMakingShowTime).SetEase(Ease.OutBack);
    }

    /// <summary>
    /// Called while setting up the room
    /// </summary>
    public void OnRoomSetupProgress(float percent) {
        roomMakingSlider.value = percent;
    }

    /// <summary>
    /// Called when the connection to the room happend, successful or not
    /// </summary>
    public void OnRoomConnected(bool success) {
        if (!success) {
            isServer = false;
            PopupManager.Instance.PopUp("Failed to make room!", "OK");
            HideRoomMakingPanel();
        } else {
            // if we are in quick match decide who the server should be
            if (isQuickMatch) {
                string myID = PlayGamesPlatform.Instance.RealTime.GetSelf().ParticipantId;
                string otherPlayerID = "";

                foreach (Participant p in PlayGamesPlatform.Instance.RealTime.GetConnectedParticipants()) {
                    // We are searching for the other player so their id cant match
                    if (p.ParticipantId.CompareTo(myID) != 0) {
                        otherPlayerID = p.ParticipantId;
                    }
                }
                Debug.Log("My id is " + myID + "  other id it " + otherPlayerID);

                // So my id preceeds the other player's id: i'm the server, he is not
                if (myID.CompareTo(otherPlayerID) < 0) {
                    isServer = true;
                }
            }

            if (isServer) { 
                ScaneManager.Instance.GoToScene(SERVER_SCENE);
            } else {
                ScaneManager.Instance.GoToScene(CLIENT_SCENE);
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

    public void OnPeersConnected(string[] participantIds) { }

    public void OnPeersDisconnected(string[] participantIds) {
        PopupManager.Instance.PopUp(new PopUpOneButton("Opponent left the game!", "OK")
                .SetButtonPressAction(() => {
                    ScaneManager.Instance.GoToSceneWithErase("Menu");
                })
            );
    }

    public void OnRealTimeMessageReceived(bool isReliable, string senderId, byte[] data) {
        string message = System.Text.Encoding.Unicode.GetString(data);
        string[] splitMessage = message.Split('#');

        switch (splitMessage[0]) {
            // CLIENT CLIENT CLIENT CLIENT CLIENT CLIENT CLIENT CLIENT CLIENT
            // CLIENT CLIENT CLIENT CLIENT CLIENT CLIENT CLIENT CLIENT CLIENT
            case GPMessageStrings.JUMP_TO: // server sends that we have to jump somewhere with camera
                Vector3 jumpTo = new Vector3(int.Parse(splitMessage[1]), int.Parse(splitMessage[2]), Camera.main.transform.position.z);

                Camera.main.transform.DOMove(jumpTo, Vector2.Distance(Camera.main.transform.position, jumpTo) * JUMP_TIME_PER_ONE);
                break;
            case GPMessageStrings.SIGN_PLACED: // Server sends that a sign was placed
                int[] lastPos = new int[] {
                    int.Parse(splitMessage[1]),
                    int.Parse(splitMessage[2])
                };
                Cell.CellOcc lastType = (Cell.CellOcc) System.Enum.Parse(typeof(Cell.CellOcc), splitMessage[3]);

                // Place sign locally
                ClientCellStorage.PlaceCellAt(lastPos, lastType);
                break;
            case GPMessageStrings.ADD_BORDER: // Server sends that a border was added
                // Get data and add a border locally
                int countOfPoints = int.Parse(splitMessage[1]);
                int[,] points = new int[countOfPoints, 2];
                float[,] winLine = new float[2, 2];

                for (int k = 0; k < countOfPoints; k++) {
                    points[k, 0] = int.Parse(splitMessage[2 * (k + 3)]);
                    points[k, 1] = int.Parse(splitMessage[2 * (k + 3) + 1]);
                }

                winLine[0, 0] = float.Parse(splitMessage[2]);
                winLine[0, 1] = float.Parse(splitMessage[3]);
                winLine[1, 0] = float.Parse(splitMessage[4]);
                winLine[1, 1] = float.Parse(splitMessage[5]);

                Cell.CellOcc winType = (Cell.CellOcc) System.Enum.Parse(typeof(Cell.CellOcc), splitMessage[6 + countOfPoints * 2]);

                BluetoothClientBorder.AddBorderPoints(points, winLine, winType);

                break;
            case GPMessageStrings.SEND_SCORE: // Server sends score
                int xScore = int.Parse(splitMessage[1]);
                int oScore = int.Parse(splitMessage[2]);

                Scoring.SetScore(xScore, oScore);
                break;
            case GPMessageStrings.TURN_OF: // Server sends whose turn it is
                Cell.CellOcc whoseTurn = (Cell.CellOcc) System.Enum.Parse(typeof(Cell.CellOcc), splitMessage[1]);

                ClientUIInScript.UpdateImage(whoseTurn);
                break;


            // SERVER SERVER SERVER SERVER SERVER SERVER SERVER SERVER SERVER SERVER
            // SERVER SERVER SERVER SERVER SERVER SERVER SERVER SERVER SERVER SERVER

            case GPMessageStrings.TRY_PLACE_AT: // client is trying to place at pos
                Vector2 pos = new Vector2(int.Parse(splitMessage[1]), int.Parse(splitMessage[2]));

                // If it's not server's turn try placing at pos
                if (!GameLogic.IsItServersTurn())
                    GameLogic.WantToPlaceAt(pos);
                break;


            // BOTH BOTH BOTH BOTH BOTH BOTH BOTH BOTH BOTH BOTH BOTH BOTH BOTH BOTH BOTH BOTH BOTH BOTH BOTH BOTH BOTH BOTH BOTH BOTH
            // BOTH BOTH BOTH BOTH BOTH BOTH BOTH BOTH BOTH BOTH BOTH BOTH BOTH BOTH BOTH BOTH BOTH BOTH BOTH BOTH BOTH BOTH BOTH BOTH
            case GPMessageStrings.SEND_MSG:
                BluetoothMessageManager.ShowEmojiMessage(EmojiSprites.GetEmoji(splitMessage[1]));
                break;
        }
    }
}

public class GPMessageStrings {
    //_________________________________ S -> C || C -> S ___________________________
    /// <summary>
    /// Sent by both client and the server
    /// ID path
    /// </summary>
    public const string SEND_MSG = "MSG";


    //________________________________ S -> C __________________________________

    /// <summary>
    /// Sent by server to client to jump to a location with camera where sign has been placed
    /// ID whereX whereY
    /// </summary>
    public const string JUMP_TO = "JPT";

    /// <summary>
    /// Sent by server to client that a sign was placed on the field
    /// ID posX posY type
    /// </summary>
    public const string SIGN_PLACED = "SWP";

    /// <summary>
    /// Sent by server to client to add a border
    /// ID numberOfPoint win1X win1Y win2X win2Y point1X point1Y point2X point2Y.... winType
    /// </summary>
    public const string ADD_BORDER = "ADB";

    /// <summary>
    /// Sent by server to client and it sends the current score
    /// ID scoreX scoreO
    /// </summary>
    public const string SEND_SCORE = "SSCR";

    /// <summary>
    /// Sent by server to client: whose turn is it
    /// ID whoseTurn
    /// </summary>
    public const string TURN_OF = "TURNOF";


    //___________________________ C -> S _________________________________

    /// <summary>
    /// Sent by client to server to try place at pos
    /// ID coordX coordY
    /// </summary>
    public const string TRY_PLACE_AT = "TPA";
}