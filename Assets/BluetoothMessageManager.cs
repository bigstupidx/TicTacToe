using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;

public class BluetoothMessageManager : MonoBehaviour {

    private static Color darkColor = new Color(0.25882f, 0.25882f, 0.25882f);
    private static Color lightColor = Color.white;

    private static PreferencesScript preferences;

    private static GameObject messageSpawnerObject;
    private static GameObject messagePrefab;

    private static float messageHeight = -1;

    private static List<RectTransform> messages = new List<RectTransform>();

    void Start() {
        preferences = GameObject.FindObjectOfType<PreferencesScript>();

        messageSpawnerObject = GameObject.FindWithTag("BluetoothMessageSpawner");
        messagePrefab = Resources.Load<GameObject>("Prefabs/Bluetooth/Messaging/Message");
    }

    /// <summary>
    /// Make new message
    /// </summary>
    private static GameObject InstantiateMessage(bool ownMessage) {
        // Make new message and it goes directly to 0 0
        GameObject go = GameObject.Instantiate(messagePrefab);
        go.transform.SetParent(messageSpawnerObject.transform, false);

        // If we don't know the size of the message yet store it
        if (messageHeight == -1) {
            messageHeight = go.GetComponent<RectTransform>().rect.height;
        }

        // Move every message down
        for (int i = messages.Count - 1; i >= 0; i--) {
            if (messages[i] == null) { // it has been destroyed
                messages.RemoveAt(i);
            } else {
                messages[i].DOMoveY(messages[i].position.y - messageHeight, 0.4f);
            }
        }

        
        go.GetComponent<BluetoothMessage>().Appear(ownMessage);

        // Set color according to colormode
        // go.GetComponent<SpriteRenderer>().color = preferences.currentMode == PreferencesScript.ColorMode.DARK ? darkColor : lightColor;

        // Add it to stack
        messages.Add(go.GetComponent<RectTransform>());

        return go;
    }

    public static void ShowTextMessage(string message, bool ownMessage = false) {
        GameObject messageObj = InstantiateMessage(ownMessage);

        Text txt = messageObj.transform.GetChild(0).GetComponent<Text>();
        txt.text = message;

        messageObj.transform.GetChild(1).gameObject.SetActive(false);
    }

    public static void ShowEmojiMessage(Sprite emojiSprite, bool ownMessage = false) {
        GameObject messageObj = InstantiateMessage(ownMessage);
        messageObj.transform.GetChild(0).gameObject.SetActive(false);

        Image img = messageObj.transform.GetChild(1).GetComponent<Image>();
        img.sprite = emojiSprite;
    }

}

public static class EmojiSprites {
    public static Sprite smilingEmoji;

    static EmojiSprites() {
        smilingEmoji = Resources.Load<Sprite>("Textures/GUI/Emojis/smilingEmoji");
    }

    public static Sprite GetEmoji(string name) {
        switch (name) {
            case "smilingEmoji": return smilingEmoji;
        }
        return smilingEmoji;
    }
}
