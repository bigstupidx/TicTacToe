using GooglePlayGames;

public class GPMessagePicker : MessagePicker {

    public override void SendMessage(MessagePickerMessageWithUI mpm) {
        // Show it for yourself
        // Not emoji -> show text message
        BluetoothMessageManager.ShowEmojiMessage(EmojiSprites.GetEmoji(mpm.message), true);

        // Send it via gp
        PlayGamesPlatform.Instance.RealTime.SendMessageToAll(true, System.Text.Encoding.Unicode.GetBytes(GPMessageStrings.SEND_MSG + "#" + mpm.message));
    }
}
