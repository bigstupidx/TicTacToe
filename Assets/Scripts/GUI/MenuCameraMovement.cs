using UnityEngine;
using DG.Tweening;

public class MenuCameraMovement : MonoBehaviour {

    private const int radius = 30;
    private const int timePerMoveOneRect = 1;
    private Camera myCamera;

    public int numberOfDoneGamePrefabs;
    private GameObject[] doneGamePrefabs;

    private GameObject doneGameParent;

    // A gameobject in dontdestroyonload which stores it in a game session so we don't have to do the heavy calculation every time
    private DoneGamesStorageMenu doneGamesStorage;

	void Start () {
        myCamera = GetComponent<Camera>();
        MoveCameraToNewRandPos();
        
        LoadResources();

        doneGamesStorage = FindObjectOfType<DoneGamesStorageMenu>();
        // We opened game first time so we need the calcculatio nadn we need a new storage object
        if (doneGamesStorage == null) {
            GameObject newStorage = Instantiate(Resources.Load<GameObject>("Prefabs/GUI/MenuDoneGamesStorage"));
            DontDestroyOnLoad(newStorage);

            doneGamesStorage = newStorage.GetComponent<DoneGamesStorageMenu>();
        }

        PlaceDoneGames();
	}

    void OnApplicationPause(bool paused) {
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

        // If we have some in storage we don't need to calculate it again
        if (doneGamesStorage.IsThereAnyStored()) {
            foreach (DoneGamesStorageMenu.MenuDoneGame doneGame in doneGamesStorage.GetStoredDoneGames()) {
                GameObject doneGameObject = Instantiate(doneGamePrefabs[doneGame.menuStorageID]);

                doneGameObject.transform.parent = doneGameParent.transform;
                doneGameObject.transform.position = doneGame.position;

                // Set random rotation 90 180 270 or 0
                doneGameObject.transform.Rotate(new Vector3(0, 0, possibleRotation[Random.Range(0, possibleRotation.Length)]));
            }
        } else { // We have nothing in storage so calculate and store
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
                    GameObject newDoneGame = Instantiate(doneGamePrefabs[idInArray]);
                    DoneGameScript dgs = newDoneGame.GetComponent<DoneGameScript>();
                    newDoneGame.transform.parent = doneGameParent.transform;

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

                    doneGamesStorage.AddNewDoneGame(position, idInArray);
                }

                // Move on y axis
                yAt += maxHeight;
            }
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
