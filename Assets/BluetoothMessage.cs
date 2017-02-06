using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class BluetoothMessage : MonoBehaviour {

    private const float SHOW_TIME = 3f;
	
	void Start() {
        Invoke("DeleteMessage", SHOW_TIME);
    }

    void DeleteMessage() {
        GetComponent<RectTransform>().DOScale(0, BluetoothMessageManager.POP_UP_ANIM).OnComplete(new TweenCallback(() => {
            Destroy(gameObject);
        }));
    }
}
