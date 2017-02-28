using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class RewardInstanceScript : MonoBehaviour, IPointerClickHandler {

    private Image crateImage;
    private Image rewardImage;
    private ParticleSystem breakParticle;
    private ParticleSystem brokenParticle;
    private ParticleSystem openParticle;

    private int clickCount = 0;

    private Sprite[] crateCracks;
    private Unlockable unlockable;

    public bool IsBroken() { return clickCount >= crateCracks.Length; }

    void Awake() {
        crateImage = transform.GetChild(1).GetComponent<Image>();
        breakParticle = transform.GetChild(2).GetComponent<ParticleSystem>();
        brokenParticle = transform.GetChild(3).GetComponent<ParticleSystem>();

        rewardImage = transform.GetChild(0).GetComponent<Image>();
        rewardImage.DOFade(0f, 0f);

        // load sprites
        crateCracks = Resources.LoadAll<Sprite>("Textures/CrateCrack");

        // Give random rottion
        crateImage.rectTransform.eulerAngles = new Vector3(0f, 0f, Random.Range(0f, 6f));
	}

    public void SetUnlockable(Unlockable unlockable) {
        this.unlockable = unlockable;
        
        switch (unlockable.type) {
            case UnlockableType.Emoji:
                openParticle = Instantiate(Resources.Load<GameObject>("Prefabs/Particles/EmojiParticle"), brokenParticle.gameObject.transform, false)
                    .GetComponent<ParticleSystem>();

                rewardImage.sprite = EmojiSprites.GetEmoji(unlockable.extra);
                break;
            case UnlockableType.Bluetooth:
                openParticle = Instantiate(Resources.Load<GameObject>("Prefabs/Particles/BluetoothParticle"), brokenParticle.gameObject.transform, false)
                    .GetComponent<ParticleSystem>();

                rewardImage.sprite = Resources.Load<Sprite>("Textures/GUI/BluetoothIcob");
                rewardImage.color = new Color(0.24706f, 0.31765f, 0.7098f);
                break;
            // TODO change this to something else because this sucks
            case UnlockableType.LocalMulti:
                openParticle = Instantiate(Resources.Load<GameObject>("Prefabs/Particles/EmojiParticle"), brokenParticle.gameObject.transform, false)
                    .GetComponent<ParticleSystem>();

                rewardImage.sprite = Resources.Load<Sprite>("Textures/GUI/personIcon");
                rewardImage.color = new Color(0.24706f, 0.31765f, 0.7098f);
                break;
        }
    }

    private float shakeTime = 0.1f;
    public void OnPointerClick(PointerEventData eventData) {
        clickCount++;

        // We have broken the crate
        if (clickCount > crateCracks.Length) return;

        // Break
        if (clickCount == crateCracks.Length) {
            brokenParticle.Play(true);
            breakParticle.Play(true);

            crateImage.gameObject.SetActive(false);

            rewardImage.DOFade(1f, 0.2f);
            return;
        }
        crateImage.sprite = crateCracks[clickCount];

        crateImage.rectTransform.DOShakeAnchorPos(shakeTime, 30f, 8, 50f);
        crateImage.rectTransform.DOShakeRotation(shakeTime, new Vector3(0, 0, 40f), 10, 30f);

        breakParticle.Play(true);
    }
}
