using UnityEngine;
using System.Collections;

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

    // Holds the previous cell's template
    // Otherwise it's null so be careful!
    private CellTemplate prevCellTemplate;

    private GameObject cellGameObject;
    private Cell cell;
    public Cell Cell {
        get {
            return cell;
        }
    }

    public CellHolder(int[] worldPos) {
        this.worldPos = worldPos;
    }

    public bool IsFull() {
        return cell != null;
    }
    
    /// <summary>
    /// Creates a new cell in this holder of type celltype
    /// </summary>
    /// <param name="cellType"></param>
    /// <param name="animate">Whether to animate the cell or not</param>
    /// <returns>Return null if the cell could not be created</returns>
    public Cell NewCell(Cell.CellOcc cellType, bool animate = true) {
        if (!cell) {
            cellGameObject = CellPooling.GetCell();
            // TODO remove spriterenderer if bluetooth rendering still not working on client side
            SpriteRenderer sprR = cellGameObject.GetComponent<SpriteRenderer>();
            sprR.sortingLayerName = "Signs" + cellType.ToString(); // Set sorting layer for dynamic batching
            if (Grid.cellParent != null) { 
                cellGameObject.transform.parent = Grid.cellParent.transform;
            }
            sprR.enabled = false;

            // Set cell type both for animation and data storage purposes
            cell = cellGameObject.GetComponent<Cell>();
            cell.cellType = cellType;

            // Animation
            if (animate) cell.TriggerDraw();
            else cell.TriggerIdle();

            sprR.enabled = true;

            // Subtract .five because the center is the pivot (because we want to rotate it to give it better look)
            cellGameObject.transform.position = new Vector2(worldPos[0] - 0.5f + Random.Range(0f, 0.08f) - 0.04f, worldPos[1] - 0.5f + Random.Range(0f, 0.08f) - 0.04f);
            cellGameObject.transform.eulerAngles = new Vector3(0, 0, Random.Range(0f, 10f) - 5f);
        } else {
            return null;
        }

        return cell;
    }
    
    /// <summary>
    /// Stores the current cell's template
    /// </summary>
    private void StoreCurrentCellInTemplate() {
        // If we have a cell
        if (cell != null) {
            prevCellTemplate = cell.GetCellTemplate();
        }
    }
    
    /// <summary>
    /// Removes the current cell
    /// </summary>
    public void RemoveCurrentCell() {
        if (cell) {
            StoreCurrentCellInTemplate();

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
        if (prevCellTemplate != null) {
            NewCell(prevCellTemplate.cellOcc, false);
            cell.UpdateAttributes(prevCellTemplate);
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
