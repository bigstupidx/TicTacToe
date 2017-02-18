using UnityEngine;

public class SignResourceStorage : MonoBehaviour {
    
    public static Color oColor = Color.blue;
    public static Color xColor = Color.red;

    private static Sprite oSprite;
    private static Sprite xSprite;


    void Awake () {
        LoadResources();
        DontDestroyOnLoad(gameObject);
    }

    void OnApplicationPause(bool paused) {
        if (!paused) {
            LoadResources();
        }
    }

    private void LoadResources() {
        // Get sprites from multiple sprite stuff
        Sprite[] allSprites = Resources.LoadAll<Sprite>("Textures/Animation/X/Xanimation0");
        xSprite = allSprites[allSprites.Length - 1];

        allSprites = Resources.LoadAll<Sprite>("Textures/Animation/O/OAnimation0");
        oSprite = allSprites[allSprites.Length - 1];
    }

    /// <summary>
    /// Return sprite related to the cellocc
    /// </summary>
    /// <param name="cellType"></param>
    /// <returns>may return null</returns>
    public static Sprite GetSpriteRelatedTo(Cell.CellOcc cellType) {
        switch (cellType) {
            case Cell.CellOcc.X:
                return xSprite;
            case Cell.CellOcc.O:
                return oSprite;
            default:
                return null;
        }
    }

    /// <summary>
    /// Returns the color of the cellType
    /// </summary>
    public static Color GetColorRelatedTo(Cell.CellOcc cellType) {
        switch (cellType) {
            case Cell.CellOcc.X: return xColor;
            case Cell.CellOcc.O: return oColor;
        }

        return Color.white;
    }

    /// <summary>
    /// Changes colors of signs to the given color mode
    /// </summary>
    public static void ChangeToColorMode(Color xColorNew, Color oColorNew) {
        xColor = xColorNew;
        oColor = oColorNew;
    }

}
