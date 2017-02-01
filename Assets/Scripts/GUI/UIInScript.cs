using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIInScript : MonoBehaviour {

    protected TTTGameLogic gameLogic;

    // Cursorsign
    [SerializeField]
    protected Image cursorSign;

    // For some unknown reason we always start with X. So in order to update it first we need to set it to O
    public Cell.CellOcc currentlyDisplayed = Cell.CellOcc.O;
	
    protected void Start() {
        gameLogic = FindObjectOfType<TTTGameLogic>();
    }

	protected void Update () {
        // We are not displaying the correct sprite on screen
        if (gameLogic.WhoseTurn != currentlyDisplayed) {
            currentlyDisplayed = gameLogic.WhoseTurn;

            UpdateSprite();
            PlaySpriteUpdateAnimation();
        }
	}

    /// <summary>
    /// Updates sprite according to currentlyDisplayed
    /// </summary>
    protected void UpdateSprite() {
        // Updates the sign next to the cursor
        cursorSign.sprite = TTTGameLogic.GetCurrentSprite();
    }

    protected void PlaySpriteUpdateAnimation() {
        // Play a little animation
        DOTween.Sequence()
            .Append(cursorSign.rectTransform.DOScale(1.2f, 0.5f))
            .Append(cursorSign.rectTransform.DOScale(1f, 0.1f))
            .OnComplete(new TweenCallback(() => {
            }));
    }
}
