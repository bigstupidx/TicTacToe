using UnityEngine;

public class CellHolder {

    // Can be disabled when the game it took part in ended
    private bool isDisabled = false;
    public bool IsDisabled {
        get { return isDisabled; }
    }
    public void Disable() { isDisabled = true; }

    // The cellholder's world position
    private int[] worldPos;
    public int[] WorldPos {
        get { return worldPos;  }
    }
    /// <summary>
    /// Used for placing the sign. It applies this amount of randomness
    /// </summary>
    public Vector2 GetRandomPosBasedOnWorldPos() {
        return new Vector2(worldPos[0] - 0.5f + Random.Range(0f, 0.08f) - 0.04f, worldPos[1] - 0.5f + Random.Range(0f, 0.08f) - 0.04f);
    }
    /// <summary>
    /// Used for placing the sign. It applies this amount of random angles
    /// </summary>
    public Vector3 GetRandomAngles() {
        return new Vector3(0, 0, Random.Range(0f, 10f) - 5f + possibleRotation[Random.Range(0, possibleRotation.Length)]);
    }

    /// <summary>
    /// Holds the current cell's template. Always has something, if the cell is destroyed and has no body, it will keep the data.
    /// This will surely have the cellType because cell may not have it if this cell has no body(gameobject), just like a BLOCKED cell
    /// </summary>
    private CellTemplate currentTemplate;
    /// <summary>
    /// Holds the current cell's template. Always has something, if the cell is destroyed and has no body, it will keep the data.
    /// This will surely have the cellType because cell may not have it if this cell has no body(gameobject), just like a BLOCKED cell
    /// </summary>
    public CellTemplate CurrentTemplate {
        get { return currentTemplate; }
    }

    private GameObject cellGameObject;
    /// <summary>
    /// May be null if the cell has no body(gameobject)
    /// </summary>
    private Cell cell;
    /// <summary>
    /// May be null if the cell has no body(gameobject)
    /// </summary>
    public Cell Cell {
        get {
            return cell;
        }
    }

    public CellHolder(int[] worldPos) {
        this.worldPos = worldPos;
        currentTemplate = new CellTemplate();
        currentTemplate.cellOcc = Cell.CellOcc.NONE;
    }

    public bool IsFull() {
        return cell != null;
    }

    private float[] possibleRotation = new float[] { 0f, 90f, 180f, 270f };
    /// <summary>
    /// Creates a new cell in this holder of type celltype.
    /// If type is blocked then it's gonna disable cell by default and the returned cell will be null
    /// </summary>
    /// <param name="cellType"></param>
    /// <param name="animate">Whether to animate the cell or not</param>
    /// <returns>Return null if the cell could not be created</returns>
    public Cell NewCell(Cell.CellOcc cellType, bool animate = true) {
        if (!cell) {
            // We have a blocking cell, for example a hole filler
            if (cellType == Cell.CellOcc.BLOCKED) {
                Disable();

                currentTemplate = new CellTemplate();
                currentTemplate.cellOcc = cellType;

                return null;
            }

            cellGameObject = CellPooling.GetCell();

            SpriteRenderer sprR = cellGameObject.GetComponent<SpriteRenderer>();
            sprR.sortingLayerName = "Signs" + cellType.ToString(); // Set sorting layer for dynamic batching
            sprR.color = SignResourceStorage.GetColorRelatedTo(cellType);

            if (Grid.cellParent != null) { 
                cellGameObject.transform.parent = Grid.cellParent.transform;
            }

            // current cell template
            currentTemplate = new CellTemplate();
            currentTemplate.cellOcc = cellType;
            currentTemplate.cellPosition = new Vector2(worldPos[0], worldPos[1]);

            // Set cell type both for animation and data storage purposes
            cell = cellGameObject.GetComponent<Cell>();
            cell.cellType = cellType;

            // Animation
            if (animate) cell.TriggerDraw();
            else cell.TriggerIdle();

            // Subtract .five because the center is the pivot (because we want to rotate it to give it better look)
            cellGameObject.transform.position = GetRandomPosBasedOnWorldPos();
            cellGameObject.transform.eulerAngles = GetRandomAngles();
        } else {
            return null;
        }

        return cell;
    }

    /// <summary>
    /// Both in current and previous cell template
    /// </summary>
    public void StoreTemplate(Cell.CellOcc type, Vector2 position) {
        currentTemplate = new CellTemplate();
        currentTemplate.cellOcc = type;
        currentTemplate.cellPosition = position;
    }
    
    /// <summary>
    /// Removes the current cell
    /// </summary>
    public void RemoveCurrentCell() {
        if (cell) {
            cell.ResetAnimator();
            CellPooling.StoreObject(cellGameObject);
            cell = null;
        }
    }

    /// <summary>
    /// Removes cell at previous pos
    /// </summary>
    public void RemoveCurrentCellWithoutStoring() {
        CellPooling.StoreObject(cellGameObject);
        cell = null;
    }
	
    /// <summary>
    /// Reinits cell based on the previous cell's template
    /// </summary>
    public void ReInitCell() {
        // We have a template
        if (currentTemplate != null) {
            NewCell(currentTemplate.cellOcc, false);
            if (cell != null) { // may not have body
                cell.cellType = currentTemplate.cellOcc;
                
                cellGameObject.transform.position = GetRandomPosBasedOnWorldPos();
                cellGameObject.transform.eulerAngles = GetRandomAngles();
            }
        }
    }

    public override bool Equals(object obj) {
        if (obj is CellHolder) {
            CellHolder ch = (CellHolder) obj;
            return ch.WorldPos[0] == WorldPos[0] && ch.WorldPos[1] == WorldPos[1];
        }

        return base.Equals(obj);
    }

    public override int GetHashCode() {
        return base.GetHashCode();
    }

}
