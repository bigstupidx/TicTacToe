using UnityEngine;
using DG.Tweening;

public class DoneGameScript : MonoBehaviour {

    // Height and width of the whole 
    public int height;
    public int width;
    
    public Cell.CellOcc winType;

    private GameObject signParent;
    
	void Start () {
        UpdateBordercolor(SignResourceStorage.GetColorRelatedTo(winType));

        signParent = transform.FindChild("Signs").gameObject;
        UpdateSignsColor(SignResourceStorage.xColor, SignResourceStorage.oColor, 0f);
    }

    void OnEnable() {
        PreferencesScript.ColorChangeEvent += ChangeToColorMode;
    }

    void OnDisable() {
        PreferencesScript.ColorChangeEvent -= ChangeToColorMode;
    }

    public void ChangeToColorMode(PreferencesScript.ColorMode colorMode, float time) {
        UpdateSignsColor(SignResourceStorage.xColor, SignResourceStorage.oColor, time);
        UpdateBordercolor(SignResourceStorage.GetColorRelatedTo(winType));
    }

    /// <summary>
    /// Updates the signs' color in this domegame
    /// </summary>
    public void UpdateSignsColor(Color xColor, Color oColor, float time) {
        foreach (Transform child in signParent.transform) {
            child.GetComponent<SpriteRenderer>().DOColor(child.name.StartsWith("X") ? xColor : oColor, time);
        }
    }

    /// <summary>
    /// Updates this gamedone's border's color
    /// </summary>
    /// <param name="borderColor"></param>
    public void UpdateBordercolor(Color borderColor) {
        transform.FindChild("Border").GetComponent<LineRenderer>().material.SetColor("_EmissionColor", borderColor);
    }
}
