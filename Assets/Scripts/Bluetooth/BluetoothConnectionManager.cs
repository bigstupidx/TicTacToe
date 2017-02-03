using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// Contains function used for both client and server
/// </summary>
public class BluetoothConnectionManager : MonoBehaviour {

    // When in lobby indicates whether ready button has been pressed or not
    private bool ready = false;
    private bool Ready {
        set {
            if (!bluetoothObject) return;

            ready = value;
            bluetoothScript.Ready = ready;
        }
        get {
            return ready;
        }
    }

    // UI needed for managing stuff
    public GameObject startPanel;
    public GameObject serverPanel;
    public GameObject clientPanel;
    public GameObject lobbyPanel;
    public Image bluetoothEnabledImage;
    public Text bluetoothNameText;

    // Prefabs for server and client
    public GameObject serverListenerPrefab;
    public GameObject clientListenerPrefab;

    // Storing server or client prefab
    private GameObject bluetoothObject;
    private BluetoothEventListener bluetoothScript;

    void Start() {
        // Log if something has been missed in unity explorer
        if (!bluetoothEnabledImage) Debug.Log("Bluetooth enabled image missing!");
        if (!bluetoothNameText) Debug.Log("Bluetooth name text missing!");
        if (!startPanel) Debug.Log("Start panel missing!");
        if (!serverPanel) Debug.Log("Server panel missing!");
        if (!clientPanel) Debug.Log("Client panel missing!");
        if (!lobbyPanel) Debug.Log("Lobby panel missing!");

        InitGUI();

        //TODO find out why we can't connect
        // Disable bluetooth because we can't connect to anything otherwise
        if (Bluetooth.Instance().IsEnabled()) Bluetooth.Instance().DisableBluetooth();
    }

    void Update() {
        UpdateBluetoothGUI();
    }

    public void ReadyUp() {
        Ready = true;

        GameObject readyButton = lobbyPanel.transform.Find("ReadyButton").gameObject;
        readyButton.GetComponent<Button>().interactable = false;
        readyButton.GetComponent<Image>().color = Color.green;
    }

    /// <summary>
    /// In case of an error or simply coming back
    /// </summary>
    public void BackFromLobby() {
        lobbyPanel.SetActive(false);
        startPanel.SetActive(true);
        Destroy(bluetoothObject);
    }

    /// <summary>
    /// Goes to lobby
    /// </summary>
    /// <param name="connectedName">Name of partner with whom it connected</param>
    public void GoToLobby(string connectedName) {
        serverPanel.SetActive(false);
        clientPanel.SetActive(false);
        lobbyPanel.SetActive(true);

        lobbyPanel.transform.Find("ConnectedNameText").GetComponent<Text>().text = connectedName;
    }

    //----------------------------
    //      Client Stuff
    //----------------------------

    /// <summary>
    /// Instantiates client prefab
    /// </summary>
    public void StartClient() {
        // Enable bluetooth if it's not enabled
        if (!Bluetooth.Instance().IsEnabled()) {
            bluetoothEnabledImage.transform.DOShakePosition(1f, 1f, 3, 5f);
            bluetoothEnabledImage.transform.DOShakeRotation(1f, 60f, 4, 5f, true);
            return;
        }

        // prefab
        bluetoothObject = Instantiate(clientListenerPrefab);
        bluetoothObject.name = "BluetoothEventListener";

        bluetoothScript = bluetoothObject.GetComponent<BluetoothEventListener>();

        // GUI
        startPanel.SetActive(false);
        clientPanel.SetActive(true);
        Bluetooth.Instance().Discoverable();
    }

    public void BackFromClientPanel() {
        Destroy(bluetoothObject);
        clientPanel.SetActive(false);
        startPanel.SetActive(true);
    }

    //----------------------------
    //      Server Stuff
    //----------------------------

    /// <summary>
    /// Switches from server to start panel, used in click event on back button
    /// </summary>
    public void BackFromServerPanel() {
        Destroy(bluetoothObject);
        serverPanel.SetActive(false);
        startPanel.SetActive(true);
    }

    /// <summary>
    /// Attempts connecting to the address given
    /// </summary>
    /// <param name="address"></param>
    public void ConnectTo(string address) {
        Bluetooth.Instance().Connect(address);
    }

    /// <summary>
    /// Starts server and starts searching for clients
    /// </summary>
    public void StartServer() {
        // Enable bluetooth if it's not enabled
        if (!Bluetooth.Instance().IsEnabled()) {
            bluetoothEnabledImage.transform.DOShakePosition(1f, 1f, 3, 5f);
            bluetoothEnabledImage.transform.DOShakeRotation(1f, 60f, 4, 5f, true);
            return;
        }

        // Make server gameobject and get script
        bluetoothObject = Instantiate(serverListenerPrefab);
        bluetoothObject.name = "BluetoothEventListener";
        bluetoothScript = bluetoothObject.GetComponent<BluetoothEventListener>();

        startPanel.SetActive(false);
        serverPanel.SetActive(true);

        // Start search
        StartSearching();
    }

    /// <summary>
    /// Called from the spinning image, so that's why it's needed to put it in a seperate function
    /// </summary>
    public void StartSearching() {
        bluetoothScript.StartSearching();
    }

    //----------------------------
    //      GUI Stuff
    //----------------------------

    /// <summary>
    /// Sets default stuff for GUI
    /// </summary>
    private void InitGUI() {
        startPanel.SetActive(true);
        serverPanel.SetActive(false);
        clientPanel.SetActive(false);
        lobbyPanel.SetActive(false);

        UpdateBluetoothGUI();

        bluetoothNameText.text = Bluetooth.Instance().DeviceName();
    }

    /// <summary>
    /// If on turns it off, if off turns it on
    /// Couldn't remember what to call it exactly
    /// </summary>
    public void SwitchBluetoothEnabled() {
        if (Bluetooth.Instance().IsEnabled()) {
            Bluetooth.Instance().DisableBluetooth();
        } else {
            Bluetooth.Instance().EnableBluetooth();
            Bluetooth.Instance().Discoverable();
        }
    }

    // Makes it visible in the GUI that the bluetooth is enabled or not
    public void UpdateBluetoothGUI() {
        if (Bluetooth.Instance().IsEnabled()) {
            bluetoothEnabledImage.color = new Color(0.055f, 0.2265f, 0.5549f);
        } else {
            bluetoothEnabledImage.color = Color.black;
        }
    }
	
}
