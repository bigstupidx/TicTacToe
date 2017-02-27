using System;
using UnityEngine;
using DG.Tweening;

public class BluetoothEventListener : MonoBehaviour {

    public static float JUMP_TIME_PER_ONE = 0.05f;

    public bool isServer = false;
    private bool isReady = false;
    public bool Ready {
        set {
            if (!isServer) {
                Bluetooth.Instance().Send(BluetoothMessageStrings.CLIENT_READY + "#" + value.ToString());
            }

            isReady = value;
            CheckAllReady();
        }
    }

    void Start() {
        DontDestroyOnLoad(gameObject);

        LoadResources();

        bluetoothConnectionManager = FindObjectOfType<BluetoothConnectionManager>();

        // Call the update function
        if (isServer) InvokeRepeating("SendClientUpdateMsg", 0f, 0.2f);
    }

    void OnApplicationPause(bool paused) {
        if (!paused) {
            LoadResources();
        }
    }

    private void LoadResources() {
        foundDevicePanel = Resources.Load("Prefabs/Bluetooth/FoundDevicePanel") as GameObject;
    }

    /// <summary>
    /// What the latest border's bluetooth ID is that we displayed
    /// </summary>
    private int lastBorderID = 0;

    /// <summary>
    /// Whether the person is in game or not
    /// </summary>
    private bool isInGame = false;

    private BluetoothConnectionManager bluetoothConnectionManager;


    //----------------------------
    //      Client Stuff
    //----------------------------

    private ClientCellStorage clientCellStorage;
    private ClientCellStorage ClientCellStrg {
        get {
            if (clientCellStorage == null) {
                clientCellStorage = FindObjectOfType<ClientCellStorage>();
            }
            return clientCellStorage;
        }
    }
    
    private BluetoothClientUIInScript clientUIInScript;
    private BluetoothClientUIInScript ClientUIInScript {
        get {
            if (clientUIInScript == null) clientUIInScript = GameObject.Find("Canvas").GetComponent<BluetoothClientUIInScript>();

            return clientUIInScript;
        }
    }

    /// <summary>
    /// Where the client thinks last sign was palced
    /// </summary>
    private int[] lastSignPlaced = new int[] { int.MaxValue, int.MaxValue };
    private int[] secondToLastSignPlaced = new int[] { int.MaxValue, int.MaxValue };

    private ScoringScript scoring;
    private ScoringScript Scoring {
        get {
            if (scoring == null) scoring = GameObject.Find("Scoring").GetComponent<ScoringScript>();

            return scoring;
        }
    }



    //----------------------------
    //      Server Stuff
    //----------------------------

    private bool isClientReady = false;

    private BluetoothTTTGameLogic gameLogic;
    private BluetoothTTTGameLogic GameLogic {
        get {
            if (gameLogic == null) gameLogic = FindObjectOfType<BluetoothTTTGameLogic>();

            return gameLogic;
        }
    }

    //Prefabs
    private GameObject foundDevicePanel;

    // The panel in which we put the device names
    private GameObject serverViewContent;
    private GameObject ServerViewContent {
        get {
            // If we haven't found it or it has been destroyed
            if (serverViewContent == null) serverViewContent = GameObject.Find("ServerViewContent");

            return serverViewContent;
        }
    }

    private BluetoothGrid grid;
    private BluetoothGrid Grid {
        get {
            if (grid == null) grid = FindObjectOfType<BluetoothGrid>();

            return grid;
        }
    }

    /// <summary>
    /// Last placed border's data
    /// </summary>
    private Border.BorderStorageLogic lastBorder;

    /// <summary>
    /// Should be called from BluetoothTTTGameLogic when a new border has been placed 
    /// </summary>
    public void SetLastBorder(Border.BorderStorageLogic lastBorder) {
        this.lastBorder = lastBorder;
        lastBorderID++;
    }

    /// <summary>
    /// All in all an update function for clientCalled every half a second or so
    /// 
    /// Sends client last sign's pos
    /// Also send last border's id
    /// Also send whose turn it is
    /// </summary>
    private void SendClientUpdateMsg() {
        if (Bluetooth.Instance().IsConnected() && isInGame) {
            // Most efficent way of concatting string
            string sent = String.Join("", new string[] {
                // last placed sign
                BluetoothMessageStrings.WHERE_LAST_PLACED,
                "#",
                GameLogic.LastSignPlaced[0].ToString(),
                "#",
                GameLogic.LastSignPlaced[1].ToString(),
                "#",
                GameLogic.LastType.ToString(),

                "|||",

                // Last placed border
                BluetoothMessageStrings.LAST_BORDER_ID.ToString(),
                "#",
                lastBorderID.ToString(),

                "|||",

                // Whose turn it is
                BluetoothMessageStrings.TURN_OF,
                "#",
                GameLogic.WhoseTurn.ToString(),

                "|||",

                // The score
                BluetoothMessageStrings.SEND_SCORE,
                "#",
                GameLogic.XScore.ToString(),
                "#",
                GameLogic.OScore.ToString()
            });

            Bluetooth.Instance().Send(sent);
        }
    }

    /// <summary>
    /// Start searching for potential clients
    /// </summary>
    public void StartSearching() {
        // Only do it for servers
        if (!isServer) return;

        // Remove every shown device first
        foreach (Transform t in ServerViewContent.transform) {
            Destroy(t.gameObject);
        }

        Debug.Log("Is server discoverable " + Bluetooth.Instance().Discoverable());
        Bluetooth.Instance().SearchDevice();
        GameObject.Find("LoadImage").GetComponent<LoadImage>().StartLoading();
    }
    
    /// <summary>
    /// Event for the end of the search devices and there is zero device
    /// </summary>
    void FoundZeroDeviceEvent() { }

    /// <summary>
    /// Event for the end of the current search
    /// </summary>
    void ScanFinishEvent() {
        GameObject.Find("LoadImage").GetComponent<LoadImage>().StopLoading();
    }

    /// <summary>
    /// Event when the search find new device
    /// </summary>
    /// <param name="Device"></param>
    void FoundDeviceEvent(string Device) {
        // split up data
        string[] data = Device.Split(new string[] { ",\n" }, StringSplitOptions.None);
        Bluetooth.Instance().MacAddresses.Add(Device); // Add to list in bluetooth

        // Create new GUI for device
        GameObject newPanel = Instantiate(foundDevicePanel, ServerViewContent.transform, false);
        
        newPanel.GetComponent<FoundDevicePanel>().SetNameAndMac(data[0], data[1], Device);
    }


    /// <summary>
    /// Check whether everyone is ready
    /// If all is ready start game
    /// </summary>
    private bool CheckAllReady() {
        if (!isServer) return false;

        if (isReady && isClientReady) {
            StartGame();
            return true;
        }

        return false;
    }

    // Starts game
    private void StartGame() {
        if (!isServer) return;

        ScaneManager.Instance.GoToScene("ServerBluetoothGame");
        Bluetooth.Instance().Send(BluetoothMessageStrings.START_GAME);
    }


    //----------------------------
    //   Server/Client Stuff
    //----------------------------

    void ConnectionStateEvent(string state) {
        //Connection State event this is the result of the connection fire after you try to Connect
        switch (state) {
            case "STATE_CONNECTED":
                // Go to lobby
                bluetoothConnectionManager.GoToLobby(Bluetooth.Instance().GetDeviceConnectedName());
                isInGame = true;
                bluetoothConnectionManager.NotConnectingAnymore();
                break;
            case "STATE_CONNECTING":
                break;
            case "UNABLE TO CONNECT":
                // We are not in game, so most likely in lobby
                if (!isInGame) {
                    PopupManager.Instance.PopUp("Couldn't connect!", "OK");
                    bluetoothConnectionManager.NotConnectingAnymore();
                } else { 
                    // We are in game
                    PopupManager.Instance.PopUp(new PopUpOneButton("Lost connection!\n Going back to menu", "OK").SetButtonPressAction(() => {
                        ScaneManager.Instance.GoToSceneWithErase("Menu");
                    }));
                    
                }
                break;
        }
    }

    /// <summary>
    /// Done sending the message event
    /// </summary>
    /// <param name="writeMessage"></param>
    void DoneSendingEvent(string writeMessage) {
        Debug.Log("Bluetooth sent: " + writeMessage);
    }

    /// <summary>
    /// Done reading the message event
    /// </summary>
    /// <param name="readMessage"></param>
    void DoneReadingEvent(string readMessage) {
        string[] differentMessages = readMessage.Split(new string[] { "|||" }, StringSplitOptions.None);
        Debug.Log("Bluetooth read: " + readMessage);

        for (int i = 0; i < differentMessages.Length - 1; i++) {
            string[] splitMessage = differentMessages[i].Split('#');

            // SERVER SERVER SERVER SERVER SERVER SERVER SERVER SERVER SERVER SERVER
            // SERVER SERVER SERVER SERVER SERVER SERVER SERVER SERVER SERVER SERVER
            if (isServer) {

                switch (splitMessage[0]) {
                    case "CLIENTREADY": // client sends he is ready
                        isClientReady = bool.Parse(splitMessage[1]);
                        CheckAllReady();
                        break;
                    case "TRYPLACEAT": // client is trying to place at pos
                        Vector2 pos = new Vector2(int.Parse(splitMessage[1]), int.Parse(splitMessage[2]));

                        // Is it's not server's turn try placing at pos
                        if (!GameLogic.IsItServersTurn())
                            GameLogic.WantToPlaceAt(pos);
                        break;
                    case "WHOSETURN": // Client is asking for whose turn it is
                        Bluetooth.Instance().Send(BluetoothMessageStrings.TURN_OF + "#" + gameLogic.WhoseTurn.ToString());
                        break;
                    case "SMB": // Client is asking for latest border data
                        Bluetooth.Instance().Send(BluetoothMessageStrings.ADD_BORDER + "#" + lastBorder.ToString() + "#" + lastBorderID);
                        break;
                    case "RLS": // Client is asking to remove last sign placed
                        if (gameLogic.IsItServersTurn()) { // only do it if it is server's turn because we know then that the client placed last
                            Grid.RemoveLastSign();
                        }
                        break;
                }

            // CLIENT CLIENT CLIENT CLIENT CLIENT CLIENT CLIENT CLIENT CLIENT
            // CLIENT CLIENT CLIENT CLIENT CLIENT CLIENT CLIENT CLIENT CLIENT
            } else {

                switch (splitMessage[0]) {
                    case "STARTGAME": // Server sends that the game has been started
                        // Switch scenes
                        ScaneManager.Instance.GoToScene("ClientBluetoothGame");
                        break;
                    case "WLP": // Server sends where last sign has been placed
                        int[] lastPos = new int[] {
                            int.Parse(splitMessage[1]),
                            int.Parse(splitMessage[2])
                        };

                        if (!(lastPos[0] == lastSignPlaced[0] && lastPos[1] == lastSignPlaced[1])) {
                            secondToLastSignPlaced[0] = lastSignPlaced[0]; secondToLastSignPlaced[1] = lastSignPlaced[1];
                            lastSignPlaced[0] = lastPos[0]; lastSignPlaced[1] = lastPos[1]; // Store new sign pos as last sign
                            
                            Cell.CellOcc lastType = (Cell.CellOcc) Enum.Parse(typeof(Cell.CellOcc), splitMessage[3]);

                            // Place sign locally
                            ClientCellStrg.PlaceCellAt(lastPos, lastType);
                        }

                        break;
                    case "LBI": // Server sends what the last border's bluetooth id is                 
                        // The server has a newer border placed
                        if (lastBorderID != int.Parse(splitMessage[1])) {
                            Bluetooth.Instance().Send(BluetoothMessageStrings.SEND_ME_BORDER);
                        }

                        break;
                    case "TURNOF": // Server sends whose turn it is
                        Cell.CellOcc whoseTurn = (Cell.CellOcc) Enum.Parse(typeof(Cell.CellOcc), splitMessage[1]);
                        
                        ClientUIInScript.UpdateImage(whoseTurn);
                        break;
                    case "ADDBORDER": // Server sends border data
                        // set lates bluetooth border id
                        lastBorderID = int.Parse(splitMessage[splitMessage.Length - 1]);

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
                        
                        Cell.CellOcc winType = (Cell.CellOcc) Enum.Parse(typeof(Cell.CellOcc), splitMessage[6 + countOfPoints * 2]);

                        BluetoothClientBorder.AddBorderPoints(points, winLine, winType);

                        break;
                    case "JPT": // Server sends to jump to this pos because new game has been started
                        Vector3 jumpTo = new Vector3(int.Parse(splitMessage[1]), int.Parse(splitMessage[2]), Camera.main.transform.position.z);

                        Camera.main.transform.DOMove(jumpTo, Vector2.Distance(Camera.main.transform.position, jumpTo) * JUMP_TIME_PER_ONE);

                        break;
                    case "SSCR":
                        int xScore = int.Parse(splitMessage[1]);
                        int oScore = int.Parse(splitMessage[2]);

                        Scoring.SetScore(xScore, oScore);
                        break;
                }

            }

            // BOTH BOTH BOTH BOTH BOTH BOTH BOTH BOTH BOTH BOTH BOTH BOTH BOTH BOTH BOTH BOTH BOTH BOTH BOTH BOTH BOTH BOTH BOTH BOTH
            // BOTH BOTH BOTH BOTH BOTH BOTH BOTH BOTH BOTH BOTH BOTH BOTH BOTH BOTH BOTH BOTH BOTH BOTH BOTH BOTH BOTH BOTH BOTH BOTH
            switch (splitMessage[0]) {
                case "SMSG":
                    BluetoothMessageManager.ShowEmojiMessage(EmojiSprites.GetEmoji(splitMessage[1]));
                    break;
            }
        }
    }
}

public static class BluetoothMessageStrings {

    //_________________________________ S -> C || C -> S ___________________________
    /// <summary>
    /// Sent by both client and server and it simply sends a message then displays it
    /// ID path
    /// </summary>
    public static readonly string SEND_MESSAGE = "SMSG";

    //________________________________ S -> C __________________________________

    /// <summary>
    /// Sent by server to client to start game
    /// ID
    /// </summary>
    public static readonly string START_GAME = "STARTGAME";

    /// <summary>
    /// Sent by server to client: whose turn is it
    /// ID whoseTurn
    /// </summary>
    public static readonly string TURN_OF = "TURNOF";

    /// <summary>
    /// Sent by server to client to add a border
    /// ID numberOfPoint win1X win1Y win2X win2Y point1X point1Y point2X point2Y.... winType bluetoothID
    /// </summary>
    public static readonly string ADD_BORDER = "ADDBORDER";

    /// <summary>
    /// Sent by server to client where last sign was placed
    /// ID whereX whereY serverTypeSign
    /// </summary>
    public static readonly string WHERE_LAST_PLACED = "WLP";

    /// <summary>
    /// Sent by server to client what last border's bluetooth ID is
    /// ID borderID
    /// </summary>
    public static readonly string LAST_BORDER_ID = "LBI";

    /// <summary>
    /// Sent by server to client to jump to a location with camera where sign has been placed
    /// ID whereX whereY
    /// </summary>
    public static readonly string JUMP_TO = "JPT";

    /// <summary>
    /// Sent by server to client and it sends the current score
    /// ID scoreX scoreO
    /// </summary>
    public static readonly string SEND_SCORE = "SSCR";


    //___________________________ C -> S _________________________________

    /// <summary>
    /// Sent by client to server to set it's ready state
    /// ID isReady
    /// </summary>
    public static readonly string CLIENT_READY = "CLIENTREADY";

    /// <summary>
    /// Sent by client to server to try place at pos
    /// ID coordX coordY
    /// </summary>
    public static readonly string TRY_PLACE_AT = "TRYPLACEAT";

    /// <summary>
    /// Sent by client to server asks whether it's his turn
    /// ID
    /// </summary>
    public static readonly string WHOSE_TURN = "WHOSETURN";

    /// <summary>
    /// Sent by client to server when it asks for the lates border
    /// ID
    /// </summary>
    public static readonly string SEND_ME_BORDER = "SMB";
}