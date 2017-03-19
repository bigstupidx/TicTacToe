using UnityEngine;

public class SignResourceStorage : Singleton<SignResourceStorage> {
    
    public Color oColor = Color.blue;
    public Color xColor = Color.red;

    public RandomSound oPlaceSounds;
    public RandomSound xPlaceSounds;


    public void PlayXRandomPlaceSound(AudioSource source) {
        xPlaceSounds.PlayRandomAudioNonRepeat(source);
    }

    public void PlayORandomPlaceSound(AudioSource source) {
        oPlaceSounds.PlayRandomAudioNonRepeat(source);
    }
    
    public void PlayRandomSoundFor(AudioSource source, Cell.CellOcc type) {
        switch (type) {
            case Cell.CellOcc.X: PlayXRandomPlaceSound(source); break;
            case Cell.CellOcc.O: PlayORandomPlaceSound(source); break;
        }
    }


    private Sprite oSprite;
    private Sprite xSprite;

    void Awake () {
        if (FindObjectsOfType<SignResourceStorage>().Length >= 2) {
            Destroy(gameObject);

            return;
        }

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

        // Load sounds
        oPlaceSounds = Resources.Load<RandomSound>("Sounds/Drawing/RandomOPlaceSound");
        xPlaceSounds = Resources.Load<RandomSound>("Sounds/Drawing/RandomXPlaceSound");
    }

    /// <summary>
    /// Return sprite related to the cellocc
    /// </summary>
    /// <param name="cellType"></param>
    /// <returns>may return null</returns>
    public Sprite GetSpriteRelatedTo(Cell.CellOcc cellType) {
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
    public Color GetColorRelatedTo(Cell.CellOcc cellType) {
        switch (cellType) {
            case Cell.CellOcc.X: return xColor;
            case Cell.CellOcc.O: return oColor;
        }

        return Color.magenta;
    }

    /// <summary>
    /// Changes colors of signs to the given color mode
    /// </summary>
    public void ChangeToColorMode(Color xColorNew, Color oColorNew) {
        xColor = xColorNew;
        oColor = oColorNew;
    }

    /// <summary>
    /// Returns: X -> O ; O -> X ; other -> NONE
    /// </summary>
    /// <param name="cellType"></param>
    /// <returns></returns>
    public static Cell.CellOcc GetOppositeOfSign(Cell.CellOcc cellType) {
        if (cellType == Cell.CellOcc.X) return Cell.CellOcc.O;
        else if (cellType == Cell.CellOcc.O) return Cell.CellOcc.X;

        return Cell.CellOcc.NONE;
    }

    /// <summary>
    /// Returns whether the given type is a sign type (not sign types is for example NONE)
    /// </summary>
    public static bool IsSignType(Cell.CellOcc type) {
        return type == Cell.CellOcc.X || type == Cell.CellOcc.O;
    }

}
