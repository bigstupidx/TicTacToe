using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;

public class RewardInstanceScript : MonoBehaviour, IPointerClickHandler {

    private Image sunRayImage;
    private Image crateImage;
    private Image rewardImage;
    private ParticleSystem breakParticle;
    private ParticleSystem brokenParticle;
    private ParticleSystem openParticle;
    private TextMeshProUGUI text;

    private int clickCount = 0;

    private Sprite[] crateCracks;
    private Unlockable unlockable;

    public bool IsBroken() { return clickCount >= crateCracks.Length; }

    void Awake() {
        sunRayImage = transform.GetChild(0).GetComponent<Image>();
        sunRayImage.rectTransform.localScale = new Vector3();

        rewardImage = transform.GetChild(1).GetComponent<Image>();
        rewardImage.DOFade(0f, 0f);
        crateImage = transform.GetChild(2).GetComponent<Image>();
        breakParticle = transform.GetChild(3).GetComponent<ParticleSystem>();
        brokenParticle = transform.GetChild(4).GetComponent<ParticleSystem>();

        text = transform.GetChild(5).GetComponent<TextMeshProUGUI>();
        text.DOFade(0f, 0f);

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
                text.text = "New emoji!";
                break;
            case UnlockableType.Bluetooth:
                openParticle = Instantiate(Resources.Load<GameObject>("Prefabs/Particles/BluetoothParticle"), brokenParticle.gameObject.transform, false)
                    .GetComponent<ParticleSystem>();

                rewardImage.sprite = Resources.Load<Sprite>("Textures/GUI/BluetoothIcob");
                rewardImage.color = new Color(0.24706f, 0.31765f, 0.7098f);
                text.text = "Bluetooth mode unlocked!";
                break;
            // TODO change these to something else because this sucks
            case UnlockableType.EmojiSlot:
                openParticle = Instantiate(Resources.Load<GameObject>("Prefabs/Particles/EmojiParticle"), brokenParticle.gameObject.transform, false)
                    .GetComponent<ParticleSystem>();

                rewardImage.sprite = Resources.Load<Sprite>("Textures/emojiSlotUnlock");
                text.text = "New slot for emojis!";
                break;
            case UnlockableType.LocalMulti:
                rewardImage.sprite = Resources.Load<Sprite>("Textures/localMultiUnlock");
                rewardImage.color = PreferencesScript.Instance.currentMode == PreferencesScript.ColorMode.DARK ? Color.white : new Color(0.14902f, 0.19608f, 0.21961f);
                text.text = "Local multiplayer unlocked!";
                break;
            case UnlockableType.GooglePlay:
                rewardImage.sprite = Resources.Load<Sprite>("Textures/gpUnlock");
                rewardImage.color = PreferencesScript.Instance.currentMode == PreferencesScript.ColorMode.DARK ? Color.white : new Color(0.14902f, 0.19608f, 0.21961f);
                text.text = "Google play multiplayer unlocked!";
                break;
        }
    }

    /// <summary>
    /// Called when the crate has been brought in
    /// </summary>
    public void CrateBroughtIn() {
        sunRayImage.rectTransform.DOScale(1f, 0.2f);
    }

    private float shakeTime = 0.1f;
    public void OnPointerClick(PointerEventData eventData) {
        clickCount++;

        // We have broken the crate
        if (clickCount > crateCracks.Length) return;

        // We don't care if it breaks, do this everytime
        DOTween.Sequence()
            .Append(sunRayImage.rectTransform.DOScale(1.1f, shakeTime))
            .Append(sunRayImage.rectTransform.DOScale(1f, shakeTime));

        // Break
        if (clickCount == crateCracks.Length) {
            brokenParticle.Play(true);
            breakParticle.Play(true);

            crateImage.gameObject.SetActive(false);

            rewardImage.DOFade(1f, 0.2f);
            text.DOFade(1f, 0.2f);
            return;
        }
        crateImage.sprite = crateCracks[clickCount];

        crateImage.rectTransform.DOShakeAnchorPos(shakeTime, 30f, 8, 50f);
        crateImage.rectTransform.DOShakeRotation(shakeTime, new Vector3(0, 0, 40f), 10, 30f);

        breakParticle.Play(true);
    }
}
