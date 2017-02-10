using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MessagePicker : MonoBehaviour {

    /// <summary>
    /// How much time it takes for the msgs to show after you click the button (also used for hiding)
    /// </summary>
    private const float MSG_SHOW_ANIMATION = 0.2f;

    private const float canvasWidth = 1920f;
    private const float canvasHeight = 1080f;

    // Dragging infos
    bool isDragging = false;
    public bool IsDragging {
        get { return isDragging; }
    }
    Vector3 dragStart = new Vector3();
    Vector3 dragEnd = new Vector3();

    private LineRenderer lineRenderer;
    private RectTransform rectTransform;
    private Image fullPanelImage;

    [Tooltip("What messages to show")]
    public MessagePickerMessage[] messages;

    public GameObject textPrefab;
    public GameObject imgPrefab;

    // Geometry stuff :'(
    [Tooltip("1 = full screen, 0 = nothing")]
    [Range(0, 1)]
    public float textCircleRadius = 0.1f;

    void Start() {
        lineRenderer = GetComponent<LineRenderer>();
        rectTransform = GetComponent<RectTransform>();

        fullPanelImage = GameObject.Find("FullPanel").GetComponent<Image>();
        fullPanelImage.gameObject.SetActive(false);
        // Set to full screen
        /* fullPanelImage.GetComponent<RectTransform>().sizeDelta = new Vector2(Camera.main.pixelWidth, Camera.main.pixelHeight);
        fullPanelImage.GetComponent<RectTransform>().position = new Vector2(+Camera.main.pixelWidth / 2f, +Camera.main.pixelHeight / 2f); */

        // Set radius to correct size
        textCircleRadius *= canvasHeight;

        InitMessagePickerUI();
    }

    void InitMessagePickerUI() {
        float oneMessageRad = Mathf.PI / messages.Length; // How much degree of a half circle one message gets

        float radOfMessage = Mathf.PI / 2 - oneMessageRad / 2f; // Where the first message is
        for (int i = 0; i < messages.Length; i++) {
            // this is caluclated by trigonometry, the x coordinate of the message
            float x = Mathf.Sqrt(textCircleRadius * textCircleRadius / (1 + Mathf.Pow(Mathf.Tan(radOfMessage), 2)));
            // the y coordinate of the message
            float y = Mathf.Tan(radOfMessage) * x;

            // Make text gameobject
            GameObject go;
            if (!messages[i].isEmoji) { 
                go = Instantiate(textPrefab);
                go.name = messages[i].message;

                Text text = go.GetComponent<Text>();
                text.text = messages[i].message;
            } else {
                go = Instantiate(imgPrefab);
                go.name = messages[i].message;

                Image img = go.GetComponent<Image>();
                img.sprite = EmojiSprites.GetEmoji(messages[i].message);
            }

            go.transform.SetParent(transform, false);

            messages[i].rectTransform = go.GetComponent<RectTransform>();
            messages[i].rectTransform.localScale = new Vector3(); // By default the messages are inside the the message icon


            // Store message's pos
            messages[i].onCirclePosition = new Vector3(x, y);

            // Store message's degrees
            messages[i].toRad = radOfMessage + oneMessageRad / 2f;
            messages[i].fromRad = messages[i].toRad - oneMessageRad;

            // Shift next message this amount
            radOfMessage -= oneMessageRad;
        }
    }

    void Update() {
        if (isDragging && lineRenderer.enabled) {
            // Update line renderer
            dragEnd = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            dragEnd.z = -5; // So it is not obstructed by anything
            
            lineRenderer.SetPosition(1, dragEnd);
        }
    }
    
    /// <summary>
    /// Called from eventtrigger in unity editor
    /// </summary>
    public void BeginDrag() {
        isDragging = true;

        dragStart = Camera.main.ScreenToWorldPoint(transform.position);
        dragStart.z = -5;

        lineRenderer.SetPosition(0, dragStart);
        lineRenderer.enabled = true;

        // Show messages with tweening
        for (int i = 0; i < messages.Length; i++) {
            messages[i].rectTransform.DOLocalMove(messages[i].onCirclePosition, MSG_SHOW_ANIMATION);
            messages[i].rectTransform.DOScale(1, MSG_SHOW_ANIMATION);
        }
        fullPanelImage.gameObject.SetActive(true);
        fullPanelImage.DOFade(0.3f, MSG_SHOW_ANIMATION);
    }

    /// <summary>
    /// Called from eventtrigger in unity editor
    /// </summary>
    public void EndDrag() {
        // Reset linerenderer
        lineRenderer.SetPosition(0, new Vector3());
        lineRenderer.SetPosition(1, new Vector3());
        lineRenderer.enabled = false;
        
        // Get radian of our swipe
        float rad = Mathf.Atan2(dragEnd.y - dragStart.y, dragEnd.x - dragStart.x);
        
        // Se wich message we wanted by going through the messages and comparing this rad to the start and end rad stored in messages
        foreach (MessagePickerMessage mpm in messages) {
            // We found the message
            if (rad >= mpm.fromRad && rad < mpm.toRad) {
                SendMessage(mpm);
                break; // We dont need to go further because we foudn it
            }
        }
        
        // Hide messages with tweening
        for (int i = 0; i < messages.Length; i++) {
            messages[i].rectTransform.DOLocalMove(new Vector3(0, 0, 0), MSG_SHOW_ANIMATION);
            messages[i].rectTransform.DOScale(0, MSG_SHOW_ANIMATION);
        }
        fullPanelImage.DOFade(0f, MSG_SHOW_ANIMATION).OnComplete(new TweenCallback(() => {
            fullPanelImage.gameObject.SetActive(false);
            isDragging = false;
        }));
    }
	
    /// <summary>
    /// Send a message via bluetooth
    /// </summary>
    private void SendMessage(MessagePickerMessage mpm) {
        // Show it for yourself
        // Not emoji -> show text message
        if (!mpm.isEmoji) { 
            BluetoothMessageManager.ShowTextMessage(mpm.message, true);
        } else { // it is an emoji
            BluetoothMessageManager.ShowEmojiMessage(EmojiSprites.GetEmoji(mpm.message), true);
        }

        // Send it vie bluetooth
        Bluetooth.Instance().Send(BluetoothMessageStrings.SEND_MESSAGE + "#" + mpm.isEmoji.ToString() + "#" + mpm.message);
    }

}

[System.Serializable]
public struct MessagePickerMessage {
    [Tooltip("The name of emoji if it's emoji. Otherwise text message")]
    public string message;
    [Tooltip("Whether it's an emoji or a message")]
    public bool isEmoji;

    [HideInInspector]
    public float fromRad;
    [HideInInspector]
    public float toRad;

    /// <summary>
    /// Where it is by default on the circle
    /// used for tweening
    /// </summary>
    [HideInInspector]
    public Vector3 onCirclePosition;

    [HideInInspector]
    public RectTransform rectTransform;
}