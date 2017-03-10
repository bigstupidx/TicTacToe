using UnityEngine;

public class SignResourceStorage : Singleton<SignResourceStorage> {
    
    public Color oColor = Color.blue;
    public Color xColor = Color.red;

    private const string soundPath = "Sounds/Drawing/";
    private AudioClip[] oPlaceSounds;
    /// <summary>
    /// Add to this to add a new o place sound
    /// </summary>
    private string[] oPlaceSoundNames = new string[] {
        "oPlaceSound1",
        "oPlaceSound2",
        "oPlaceSound3",
        "oPlaceSound4"
    };
    private AudioClip[] xPlaceSounds;
    /// <summary>
    /// Add to this to add a new x place sound
    /// </summary>
    private string[] xPlaceSoundNames = new string[] {
        "xPlaceSound1",
        "xPlaceSound2",
        "xPlaceSound3",
        "xPlaceSound4"
    };


    public AudioClip XRandomPlaceSound {
        get {
            return xPlaceSounds[Random.Range(0, xPlaceSounds.Length)];
        }
    }

    public AudioClip ORandomPlaceSound {
        get {
            return oPlaceSounds[Random.Range(0, oPlaceSounds.Length)];
        }
    }

    /// <summary>
    /// If not given X or O it will return null
    /// </summary>
    public AudioClip GetRandomSoundFor(Cell.CellOcc type) {
        switch (type) {
            case Cell.CellOcc.X: return XRandomPlaceSound;
            case Cell.CellOcc.O: return ORandomPlaceSound;
        }

        return null;
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
        oPlaceSounds = new AudioClip[oPlaceSoundNames.Length];
        xPlaceSounds = new AudioClip[xPlaceSoundNames.Length];

        for (int i = 0; i < oPlaceSoundNames.Length; i++) {
            oPlaceSounds[i] = Resources.Load<AudioClip>(soundPath + oPlaceSoundNames[i]);
        }

        for (int i = 0; i < xPlaceSoundNames.Length; i++) {
            xPlaceSounds[i] = Resources.Load<AudioClip>(soundPath + xPlaceSoundNames[i]);
        }
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
