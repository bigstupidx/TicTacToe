using UnityEngine;
using System.Collections;

public class DrawGrid : MonoBehaviour {

    // How many more lines to draw an each side of the rectangle
    protected const int GRID_BOUND = 10;
    protected const float lineWidth = .05f;
    protected const string GRIDMANAGER_PREFAB_PATH = "Prefabs/GridManager";

    public GameObject lineObject;
    protected GameObject gridParent;
    protected GameObject gridManager;

    public int gridWidth;
    public int gridHeight;

    public virtual void Start() {
        // Init grid
        gridManager = Instantiate(Resources.Load(GRIDMANAGER_PREFAB_PATH) as GameObject);
        gridManager.name = "GridManager";

        gridParent = new GameObject("Grid");
        gridParent.transform.parent = gridManager.transform;

        // Grid attributes
        gridHeight = (int) Camera.main.orthographicSize * 2 + 3;
        gridWidth = (int) Mathf.Round(((float) Camera.main.pixelWidth / Camera.main.pixelHeight) * gridHeight) + 1;

        InitGrid();
	}

    protected void Update() {
        // Update the grid's position
        Vector3 newPos = Camera.main.transform.position;
        // Add position so the grid looks like it's infinite
        newPos.x -= newPos.x - (int) newPos.x;
        newPos.y -= newPos.y - (int) newPos.y;
        newPos.z = transform.position.z;

        gridParent.transform.position = newPos;
    }

    // Draws out lines
    protected void InitGrid() {
        // The bounds of the grid
        int minX = -gridWidth / 2;
        int minY = -gridHeight / 2;
        int maxX = gridWidth / 2;
        int maxY = gridHeight / 2;

        // Vertical
        for (int i = -GRID_BOUND * 2; i < gridWidth + GRID_BOUND * 2; i++) {
            DrawLine(new Vector3(minX + i, minY), new Vector3(minX + i, maxY), new Vector3(lineWidth, gridHeight + 2 * GRID_BOUND));
        }

        // Horizontal
        for (int i = -GRID_BOUND; i < gridHeight + GRID_BOUND; i++) {
            DrawLine(new Vector3(minX, minY + i), new Vector3(maxX, minY + i), new Vector3(gridWidth + 4 * GRID_BOUND, lineWidth));
        }
    }

    // Simply draws a line with instantiating an object
    protected void DrawLine(Vector3 start, Vector3 end, Vector3 scale) {
        GameObject line = Instantiate(lineObject, new Vector3(), Quaternion.identity) as GameObject;

        line.transform.parent = gridParent.transform;
        line.transform.position = (start + end) / 2;
        
        line.transform.localScale = scale;
    }
}
