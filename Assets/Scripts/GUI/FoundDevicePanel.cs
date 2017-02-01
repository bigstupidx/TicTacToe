using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// An order is required when it comes to it's child objects: nameText, macText and then connectImage
/// </summary>
public class FoundDevicePanel : MonoBehaviour {

    private UnityEvent clickEvent;

    private Text nameText, macText;
    private EventTrigger imageTrigger;
    private string connectionAddress;

    private BluetoothConnectionManager connectionManager;

    void Awake() {
        connectionManager = FindObjectOfType<BluetoothConnectionManager>();

        nameText = transform.GetChild(0).GetComponent<Text>();
        macText = transform.GetChild(1).GetComponent<Text>();

        imageTrigger = GetComponent<EventTrigger>();
    }
    
	public void SetNameAndMac(string name, string mac, string connectionAddress) {
        nameText.text = name;
        macText.text = mac;
        this.connectionAddress = connectionAddress;
        
        imageTrigger.triggers.Clear(); // Clear in case we set it's stuff the second time

        // Add new click event
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((eventData) => { connectionManager.ConnectTo(this.connectionAddress); });
        imageTrigger.triggers.Add(entry);
    }

    public void DestroyIt() {
        Destroy(nameText.gameObject);
        Destroy(macText.gameObject);
        Destroy(imageTrigger.gameObject);
        Destroy(gameObject);
    }
}
