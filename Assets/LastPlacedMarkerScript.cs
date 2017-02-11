using UnityEngine;
using DG.Tweening;

public class LastPlacedMarkerScript : MonoBehaviour {

    /// <summary>
    /// How much time it takes to travel a unit
    /// </summary>
    private float timePerUnit = 0.05f;

    private SpriteRenderer spriteRenderer;

    private bool isEnabled = false;

	void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = new Color(1, 1, 1, 0);
	}
	
	public void MoveMarkerTo(Vector2 pos, Color color) {
        pos.x -= 0.5f;
        pos.y -= 0.5f;

        if (!isEnabled) {
            color.a = 0;
            spriteRenderer.color = color;
            transform.position = pos;

            // only do fading if it is not enabled
            spriteRenderer.DOFade(1, 0.2f);

            isEnabled = true;
        } else { 
            float time = Vector2.Distance(transform.position, pos) * timePerUnit;

            // Moving
            spriteRenderer.DOColor(color, time).SetEase(Ease.InQuad);
            transform.DOMove(pos, time).SetEase(Ease.Linear);

            // Disappear appear
            /* DOTween.Sequence()
                .Append(spriteRenderer.DOFade(0, 0.2f))
                .Append(transform.DOMove(pos, 0))
                .Append(spriteRenderer.DOColor(color, 0.2f)); */
        }
    }

    public void Disable() {
        isEnabled = false;
        spriteRenderer.DOFade(0, 0.2f);
    }
}
