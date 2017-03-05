using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIInScript : MonoBehaviour {
    protected TTTGameLogic gameLogic;

    // Cursorsign
    [SerializeField]
    protected Image currentSign;

    protected void Start() {
        gameLogic = FindObjectOfType<TTTGameLogic>();
        currentlyDisplayed = Cell.CellOcc.BLOCKED;
    }


    // Set it to BLOCKED so we always update it at first
    protected Cell.CellOcc currentlyDisplayed = Cell.CellOcc.BLOCKED;

	protected void Update () {
        // We are not displaying the correct sprite on screen
        if (gameLogic.WhoseTurn != currentlyDisplayed) {
            currentlyDisplayed = gameLogic.WhoseTurn;

            UpdateSprite();
            PlayCursorSpriteUpdateAnimation();
        }
	}

    /// <summary>
    /// Updates sprite according to currentlyDisplayed
    /// </summary>
    protected void UpdateSprite() {
        // Updates the sign next to the cursor
        currentSign.sprite = gameLogic.GetCurrentSprite();
        currentSign.color = SignResourceStorage.GetColorRelatedTo(gameLogic.WhoseTurn);
    }

    protected void PlayCursorSpriteUpdateAnimation() {
        // Play a little animation
        DOTween.Sequence()
            .Append(currentSign.rectTransform.DOScale(1.2f, 0.5f))
            .Append(currentSign.rectTransform.DOScale(1f, 0.1f))
            .OnComplete(new TweenCallback(() => {
            }));
    }
}
