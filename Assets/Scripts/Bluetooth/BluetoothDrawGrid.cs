using UnityEngine;

public class BluetoothDrawGrid : DrawGrid {

    protected new const string GRIDMANAGER_PREFAB_PATH = "Prefabs/Bluetooth/GridManager";

    public override void Start() {
        preferences = FindObjectOfType<PreferencesScript>();

        // Init grid
        gridManager = Instantiate(Resources.Load(GRIDMANAGER_PREFAB_PATH) as GameObject);
        gridManager.name = "GridManager";
        lightDark = gridManager.GetComponent<GridLightDarkMode>();

        gridParent = new GameObject("Grid");
        gridParent.transform.parent = gridManager.transform;

        // Grid attributes
        gridHeight = (int) Camera.main.orthographicSize * 2 + 3;
        gridWidth = (int) Mathf.Round(((float) Camera.main.pixelWidth / Camera.main.pixelHeight) * gridHeight) + 1;

        InitGrid();
    }

}
