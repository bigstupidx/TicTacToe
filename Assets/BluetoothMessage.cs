using UnityEngine;
using DG.Tweening;

public class BluetoothMessage : MonoBehaviour {

    public float POP_UP_ANIM = 0.6f;
    private const float SHOW_TIME = 3f;

    /// <summary>
    /// Plays appearing tween animation
    /// </summary>
    /// <param name="isOwn"></param>
    public void Appear(bool isOwn) {
        GetComponent<RectTransform>().localScale = new Vector3();
        GetComponent<RectTransform>().DOScale(new Vector3(isOwn ? -1 : 1, 1, 1), POP_UP_ANIM).SetEase(Ease.OutBack);

        // If it's our own message we need to flip the text or the image because the image is flipped and we dont want the text or image to be as well
        if (isOwn) {
            for (int i = 0; i < transform.childCount; i++) transform.GetChild(i).GetComponent<RectTransform>().localScale = new Vector3(-1, 1, 1);
        }
    }
	
	void Start() {
        Invoke("DeleteMessage", SHOW_TIME);
    }

    void DeleteMessage() {
        GetComponent<RectTransform>().DOScale(0, POP_UP_ANIM).OnComplete(new TweenCallback(() => {
            Destroy(gameObject);
        }));
    }
}
