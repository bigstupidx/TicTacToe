using UnityEngine;

public class MusicByButton : MonoBehaviour {
    
	public void ShowComposers() {
        PopupManager.Instance.PopUp("Music from:\n" +
            "soundcloud.com/laserost,\nsyncopika,\njlwinn8videos on YouTube,\nmarksparling.com", "Ok");
    }
}
