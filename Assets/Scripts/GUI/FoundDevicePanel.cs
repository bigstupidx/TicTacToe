using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

/// <summary>
/// An order is required when it comes to it's child objects: nameText, macText and then connectImage
/// </summary>
public class FoundDevicePanel : MonoBehaviour {

    private const float clickAnimDuration = 0.45f;

    private UnityEvent clickEvent;

    private Text nameText;
    private EventTrigger imageTrigger;
    private Image panel;
    private string connectionAddress;

    private BluetoothConnectionManager connectionManager;

    void Awake() {
        connectionManager = FindObjectOfType<BluetoothConnectionManager>();

        nameText = transform.GetChild(0).GetComponent<Text>();

        imageTrigger = GetComponent<EventTrigger>();
        panel = GetComponent<Image>();
    }
    
	public void SetNameAndMac(string name, string mac, string connectionAddress) {
        nameText.text = name;
        this.connectionAddress = connectionAddress;
        
        imageTrigger.triggers.Clear(); // Clear in case we set it's stuff the second time

        // Add new click event
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((eventData) => {
            connectionManager.ConnectTo(this.connectionAddress);
            
            // Play little click animation
            DOTween.Sequence()
                .Append(panel.rectTransform.DOScale(0.95f, clickAnimDuration / 2f))
                .Insert(0, panel.DOFade(30f / 255f, clickAnimDuration / 2f))
                .Append(panel.rectTransform.DOScale(1f, clickAnimDuration / 2f))
                .Insert(clickAnimDuration / 2f, panel.DOFade(10f / 255f, clickAnimDuration / 2f));
        });
        imageTrigger.triggers.Add(entry);
    }

    public void DestroyIt() {
        Destroy(nameText.gameObject);
        Destroy(imageTrigger.gameObject);
        Destroy(gameObject);
    }
}
