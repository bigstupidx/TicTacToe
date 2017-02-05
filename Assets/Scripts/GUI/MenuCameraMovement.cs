using UnityEngine;
using DG.Tweening;

public class MenuCameraMovement : MonoBehaviour {

    /// <summary>
    /// Spawn radius of gamedones
    /// </summary>
    private const int radius = 30;
    /// <summary>
    /// How many time the camera takes to move one tile
    /// </summary>
    private const int timePerMoveOneRect = 1;
    private Camera myCamera;

    public int numberOfDoneGamePrefabs;
    private GameObject[] doneGamePrefabs;

    private GameObject doneGameParent;

    void Start() {
        myCamera = GetComponent<Camera>();
        MoveCameraToNewRandPos();

        LoadResources();

        // First search for it because if we found it there is no need to initialize it again
        doneGameParent = GameObject.Find("DoneGameParent");
        
        // We haven't initiated the donegames yet in this sesion so JUST DO IT
        if (doneGameParent == null)
            PlaceDoneGames();
    }
 
    void OnApplicationPause(bool paused) {
        // If we come back from pause reload resources just in case
        if (!paused) {
            LoadResources();
        }
    }

    

    private void LoadResources() {
        doneGamePrefabs = new GameObject[numberOfDoneGamePrefabs];
        for (int i = 0; i < numberOfDoneGamePrefabs; i++)
            doneGamePrefabs[i] = Resources.Load("Prefabs/DoneGames/DoneGame" + i) as GameObject;
    }

    private int[] possibleRotation = new int[] {
        0, 90, 180, 270
    };
    private void PlaceDoneGames() {
        doneGameParent = new GameObject("DoneGameParent");

        // Add a draw grid to the doneGame
        DrawGrid dg = doneGameParent.AddComponent<DrawGrid>();
        dg.DisableClickHandler = true;
        dg.lineObject = Resources.Load<GameObject>("Prefabs/Line");
        dg.gridManagerParent = doneGameParent.transform;

        // Add disabler
        doneGameParent.AddComponent<DoneGameDisabler>();

        DontDestroyOnLoad(doneGameParent);

        float areaMultiplier = 1.7f;

        int xAt;
        int yAt = (int) (-radius * areaMultiplier);

        // We place a donegame then we allocate a areaMultiplier times bigger space it needs in a square We place it there randomly then move on to the next one in the row
        // We do it until row is full the we take the max height of that row and move down by that much to form a new row
        // Then we start again
        // We do it until all the rows are filled
        while (yAt <= (int) (radius * areaMultiplier)) {
            int maxHeight = 0;
            xAt = (int) (-radius * areaMultiplier);

            while (xAt <= (int) (radius * areaMultiplier)) {
                int idInArray = Random.Range(0, doneGamePrefabs.Length);
                GameObject newDoneGame = Instantiate(doneGamePrefabs[idInArray], doneGameParent.transform);
                DoneGameScript dgs = newDoneGame.GetComponent<DoneGameScript>();

                // Decide which length of the dgs's size is bigger
                int biggerSize = dgs.height > dgs.width ? dgs.height : dgs.width;
                int allocatedSpace = (int) (biggerSize * areaMultiplier);

                // Where we should place the dgs
                // Get the middle of the allocated space then pick a random point in a circle
                Vector2 position = new Vector2(xAt + biggerSize / 2f, yAt + biggerSize / 2f) + Random.insideUnitCircle * (biggerSize * (areaMultiplier - 1f) / 2f);
                // It needs to be placed on 0.5 pos
                position.x = (int) position.x + 0.5f;
                position.y = (int) position.y + 0.5f;

                // Move to the next position of x and store y if it's bigger than before
                xAt += allocatedSpace;
                if (maxHeight <= allocatedSpace) maxHeight = allocatedSpace;

                // Set the new pos if the dgs
                newDoneGame.transform.position = position;

                // Set random rotation 90 180 270 or 0
                newDoneGame.transform.Rotate(new Vector3(0, 0, possibleRotation[Random.Range(0, possibleRotation.Length)]));
            }

            // Move on y axis
            yAt += maxHeight;
        }
    }

    /// <summary>
    /// Recursive function makes camera move in a radius
    /// </summary>
    private void MoveCameraToNewRandPos() {
        // Get new position where to go inside radius circle around spawn
        Vector3 randPos = Random.insideUnitCircle * radius * 2;
        randPos.z = myCamera.transform.position.z;

        // Go there and after it is ready do it again
        myCamera.transform.DOMove(randPos, timePerMoveOneRect * Vector2.Distance(Camera.main.transform.position, randPos)).SetEase(Ease.Linear).OnComplete(new TweenCallback(() => {
            MoveCameraToNewRandPos();
        }));
    }
}
