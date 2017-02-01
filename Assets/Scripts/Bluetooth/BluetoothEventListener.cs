using System;
using UnityEngine;
using DG.Tweening;

public class BluetoothEventListener : MonoBehaviour {

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

        foundDevicePanel = Resources.Load("Prefabs/Bluetooth/FoundDevicePanel") as GameObject;

        if (isServer) InvokeRepeating("SendClientLastSign", 0f, 0.2f);
    }

    /// <summary>
    /// Whether the person is in game or not
    /// </summary>
    private bool isInGame = false;


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

    private int[] lastSignPlaced = new int[] { int.MaxValue, int.MaxValue };


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

    /// <summary>
    /// Sends client last sign's pos. Called every half a second or so
    /// </summary>
    private void SendClientLastSign() {
        Bluetooth.Instance().Send(BluetoothMessageStrings.WHERE_LAST_PLACED + "#" + GameLogic.LastSignPlaced[0] + "#" + GameLogic.LastSignPlaced[1] + "#" + GameLogic.LastType.ToString());
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
    void FoundZeroDeviceEvent() {
        Debug.Log("FoundZeroDeviceEvent");
        Bluetooth.Instance().showMessage("FoundZeroDeviceEvent");
    }

    /// <summary>
    /// Event for the end of the current search
    /// </summary>
    void ScanFinishEvent() {
        GameObject.Find("LoadImage").GetComponent<LoadImage>().StopLoading();
        Bluetooth.Instance().showMessage("ScanFinishEvent");
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
        GameObject newPanel = Instantiate(foundDevicePanel);
        newPanel.transform.SetParent(ServerViewContent.transform, false);
        
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

        FindObjectOfType<ScaneManager>().GoToScene("ServerBluetoothGame");
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
                FindObjectOfType<BluetoothConnectionManager>().GoToLobby(Bluetooth.Instance().GetDeviceConnectedName());
                isInGame = true;
                break;
            case "STATE_CONNECTING":
                break;
            case "UNABLE TO CONNECT":
                // We are not in game, so most likely in lobby
                if (!isInGame) {
                    PopupManager.PopUp("Couldn't connect!", "OK");
                } else { 
                    // We are in game
                    PopupManager.PopUp("Lost connection!\n Going back to menu", "OK", () => {
                        FindObjectOfType<ScaneManager>().GoToSceneWithErase("Menu");
                    });
                }
                break;
        }
        Debug.Log(state);
        Bluetooth.Instance().showMessage("Connection state " + state);
    }

    /// <summary>
    /// Done sending the message event
    /// </summary>
    /// <param name="writeMessage"></param>
    void DoneSendingEvent(string writeMessage) {
        Debug.Log("writeMessage " + writeMessage);
    }

    /// <summary>
    /// Done reading the message event
    /// </summary>
    /// <param name="readMessage"></param>
    void DoneReadingEvent(string readMessage) {
        string[] differentMessages = readMessage.Split(new string[] { "|||" }, StringSplitOptions.None);

        for (int i = 0; i < differentMessages.Length - 1; i++) {
            Debug.Log("readMessage " + differentMessages[i]);
            string[] splitMessage = differentMessages[i].Split('#');

            // SERVER SERVER SERVER SERVER SERVER SERVER SERVER SERVER SERVER SERVER
            // SERVER SERVER SERVER SERVER SERVER SERVER SERVER SERVER SERVER SERVER
            if (isServer) {

                switch (splitMessage[0]) {
                    case "CLIENTREADY":
                        isClientReady = bool.Parse(splitMessage[1]);
                        CheckAllReady();
                        break;
                    case "TRYPLACEAT":
                        Vector2 pos = new Vector2(int.Parse(splitMessage[1]), int.Parse(splitMessage[2]));

                        // Is it's not server's turn try placing at pos
                        if (!GameLogic.IsItServersTurn())
                            GameLogic.WantToPlaceAt(pos);
                        break;
                    case "WHOSETURN":
                        Bluetooth.Instance().Send(BluetoothMessageStrings.TURN_OF + "#" + gameLogic.WhoseTurn.ToString());
                        break;
                }

                // CLIENT CLIENT CLIENT CLIENT CLIENT CLIENT CLIENT CLIENT CLIENT
                // CLIENT CLIENT CLIENT CLIENT CLIENT CLIENT CLIENT CLIENT CLIENT
            } else {

                switch (splitMessage[0]) {
                    case "STARTGAME":
                        // Switch scenes
                        FindObjectOfType<ScaneManager>().GoToScene("ClientBluetoothGame");
                        break;
                    case "WLP":
                        int[] lastPos = new int[] {
                            int.Parse(splitMessage[1]),
                            int.Parse(splitMessage[2])
                        };

                        // If there is a new sign that has been placed
                        if (!(lastPos[0] == lastSignPlaced[0] && lastPos[1] == lastSignPlaced[1])) {
                            lastSignPlaced[0] = lastPos[0]; lastSignPlaced[1] = lastPos[1]; // Store new sign pos as last sign
                            
                            Cell.CellOcc lastType = (Cell.CellOcc) Enum.Parse(typeof(Cell.CellOcc), splitMessage[3]);

                            // Place sign locally
                            ClientCellStrg.PlaceCellAt(lastPos, lastType);
                        }

                        break;
                    case "TURNOF":
                        Cell.CellOcc whoseTurn = (Cell.CellOcc) Enum.Parse(typeof(Cell.CellOcc), splitMessage[1]);
                        
                        ClientUIInScript.UpdateImage(whoseTurn);

                        break;
                    case "ADDBORDER":
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

                        Color c = new Color(
                            float.Parse(splitMessage[6 + countOfPoints * 2]),
                            float.Parse(splitMessage[6 + countOfPoints * 2 + 1]),
                            float.Parse(splitMessage[6 + countOfPoints * 2 + 2])
                        );

                        BluetoothClientBorder.AddBorderPoints(points, winLine, c);

                        break;
                    case "JPT":
                        Vector3 jumpTo = new Vector3(int.Parse(splitMessage[1]), int.Parse(splitMessage[2]));

                        Camera.main.transform.DOMove(jumpTo, 0.5f);

                        break;
                }

            }
        }
    }
}

public class BluetoothMessageStrings {

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
    /// ID numberOfPoint win1X win1Y win2X win2Y point1X point1Y point2X point2Y.... colorOfBorderR colorOfBorderG colorOfBorderB
    /// </summary>
    public static readonly string ADD_BORDER = "ADDBORDER";

    /// <summary>
    /// Sent by server to client where last sign was placed
    /// ID whereX whereY serverTypeSign
    /// </summary>
    public static readonly string WHERE_LAST_PLACED = "WLP";

    /// <summary>
    /// Sent by server to client to jump to a location with camera where sign has been placed
    /// ID whereX whereY
    /// </summary>
    public static readonly string JUMP_TO = "JPT";


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
}