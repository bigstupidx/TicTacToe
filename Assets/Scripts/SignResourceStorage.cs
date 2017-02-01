using UnityEngine;

public class SignResourceStorage : MonoBehaviour {
    
    protected static Color oColor = Color.blue;
    protected static Color xColor = Color.red;

    private static Sprite oSprite;
    private static Sprite xSprite;

    private static GameObject oFireworkPrefab;
    private static GameObject xFireworkPrefab;

    private static ParticleSystem oFireworkInstance;
    public static ParticleSystem OFirework {
        get { return oFireworkInstance; }
    }
    private static ParticleSystem xFireworkInstance;
    public static ParticleSystem XFirework {
        get { return xFireworkInstance; }
    }


    void Awake () {
        // Get sprites from multiple sprite stuff
        Sprite[] allSprites = Resources.LoadAll<Sprite>("Textures/XAnimation");
        xSprite = allSprites[allSprites.Length - 1];

        allSprites = Resources.LoadAll<Sprite>("Textures/OAnimation");
        oSprite = allSprites[allSprites.Length - 1];

        // Fireworks
        oFireworkPrefab = Resources.Load<GameObject>("Prefabs/Fireworks/FireworkBlue");
        xFireworkPrefab = Resources.Load<GameObject>("Prefabs/Fireworks/FireworkRed");

        oFireworkInstance = Instantiate(oFireworkPrefab).GetComponent<ParticleSystem>();
        xFireworkInstance = Instantiate(xFireworkPrefab).GetComponent<ParticleSystem>();
    }

    /// <summary>
    /// Play firework of type at pos
    /// </summary>
    public static void PlayFireworkAt(Vector2 pos, Cell.CellOcc type) {
        GetFireworkRelatedTo(type).transform.position = pos;
        GetFireworkRelatedTo(type).Play();
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
    /// Return firework prefab related to the cellocc
    /// </summary>
    /// <param name="cellType"></param>
    /// <returns>may return null</returns>
    public static GameObject GetFireworkPrefabRelatedTo(Cell.CellOcc cellType) {
        switch (cellType) {
            case Cell.CellOcc.X:
                return xFireworkPrefab;
            case Cell.CellOcc.O:
                return oFireworkPrefab;
            default:
                return null;
        }
    }

    /// <summary>
    /// Return firework particle system related to the cellocc
    /// </summary>
    /// <param name="cellType"></param>
    /// <returns>may return null</returns>
    public static ParticleSystem GetFireworkRelatedTo(Cell.CellOcc cellType) {
        switch (cellType) {
            case Cell.CellOcc.X:
                return xFireworkInstance;
            case Cell.CellOcc.O:
                return oFireworkInstance;
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

}
