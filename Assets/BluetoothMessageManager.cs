using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;

public class BluetoothMessageManager : MonoBehaviour {

    public static float POP_UP_ANIM = 0.6f;

    private static Color darkColor = new Color(0.25882f, 0.25882f, 0.25882f);
    private static Color lightColor = Color.white;

    private static PreferencesScript preferences;

    private static GameObject messageSpawnerObject;
    private static GameObject messagePrefab;

    private static float messageHeight;

    private static Stack<RectTransform> messages = new Stack<RectTransform>();

    void Start() {
        preferences = GameObject.FindObjectOfType<PreferencesScript>();

        messageSpawnerObject = GameObject.FindWithTag("BluetoothMessageSpawner");
        messagePrefab = Resources.Load<GameObject>("Prefabs/Bluetooth/Message");

        messageHeight = messagePrefab.GetComponent<RectTransform>().rect.height;
        ShowTextMessage("ASD");
    }

    /// <summary>
    /// Make new message
    /// </summary>
    private static GameObject InstantiateMessage() {
        // Move every message down
        foreach (RectTransform rectTransform in messages) {
            rectTransform.DOMoveY(rectTransform.position.y - messageHeight, 0.4f).SetDelay(1);
        }

        // Make new message and it goes directly to 0 0
        GameObject go = GameObject.Instantiate<GameObject>(messagePrefab);
        go.transform.SetParent(messageSpawnerObject.transform, false);

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.localScale = new Vector3(0, 0, 0);
        rt.DOScale(1f, POP_UP_ANIM).SetEase(Ease.OutBack).SetDelay(1);

        // Set color according to colormode
        // go.GetComponent<SpriteRenderer>().color = preferences.currentMode == PreferencesScript.ColorMode.DARK ? darkColor : lightColor;

        // Add it to stack
        messages.Push(rt);

        return go;
    }

    public static void ShowTextMessage(string message) {
        GameObject messageObj = InstantiateMessage();

        Text txt = messageObj.transform.GetChild(0).GetComponent<Text>();
        txt.text = message;

        messageObj.transform.GetChild(1).gameObject.SetActive(false);
    }

    public static void ShowEmojiMessage(Sprite emojiSprite) {
        GameObject messageObj = InstantiateMessage();
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
