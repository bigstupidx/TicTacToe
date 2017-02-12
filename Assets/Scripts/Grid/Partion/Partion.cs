using UnityEngine;
using System.Collections.Generic;

public class Partion {

    public static int WIDTH_HEIGHT = 10;

    private int[] partionPos;
    private CellHolder[,] cells;

    private List<int[]> hasSign = new List<int[]>();

    // whether the partion is shown in game
    private bool isShown = false;
    public bool IsShown {
        get { return isShown; }
    }
	
    // ______________________ Constructor __________________________
    // Requieres the partion's world pos
    /// <param name="partionPos">Partion's world pos</param>
    public Partion(int[] partionPos) {
        this.partionPos = partionPos;
        cells = new CellHolder[WIDTH_HEIGHT, WIDTH_HEIGHT];

        for (int i = 0; i < WIDTH_HEIGHT; i++) {
            for (int k = 0; k < WIDTH_HEIGHT; k++) {
                int[] localPos = new int[] { i, k };

                cells[i, k] = new CellHolder(GetWorldCellPos(localPos));
            }
        }
    }

    /// <summary>
    /// Removes cell at localPos position
    /// </summary>
    /// <param name="localPos"></param>
    public void RemoveCellAt(int[] localPos) {
        cells[localPos[0], localPos[1]].RemoveCurrentCellWithoutStoring();
    }
    
    /// <summary>
    /// Places a sign in the cell
    /// </summary>
    /// <param name="pos">localpartionpos of cell</param>
    /// <param name="cellType"></param>
    /// <returns>whether the sign could be placed or not</returns>
    public bool PlaceSign(int[] pos, Cell.CellOcc cellType, bool disabled = false) {
        hasSign.Add(pos);
        // If we want to create it disabled dont make an object at first just store a celltemplate
        if (disabled) {
            cells[pos[0], pos[1]].StoreTemplate(cellType, cells[pos[0], pos[1]].GetRandomPosBasedOnWorldPos());
            cells[pos[0], pos[1]].Disable();
            return true;
        }

        Cell c = cells[pos[0], pos[1]].NewCell(cellType);
        // Couldn't create cell for some reason
        if (c == null) {
            return false;
        }

        // At this point the cell was surely created

        return true;
    }
    
    /// <summary>
    /// Return cellHolder at pos
    /// </summary>
    /// <param name="pos">Localpartionpos of cell</param>
    /// <returns></returns>
    public CellHolder GetCellHolderAt(int[] pos) {
        return cells[pos[0], pos[1]];
    }

    /// <summary>
    /// Returns all cellholder which has signs in this partion
    /// </summary>
    /// <returns></returns>
    public CellHolder[] GetAllCellsWhichHaveSigns() {
        CellHolder[] holder = new CellHolder[hasSign.Count];

        for (int i = 0; i < hasSign.Count; i++)
            holder[i] = cells[hasSign[i][0], hasSign[i][1]];

        return holder;
    }
    
    /// <summary>
    /// Shows the partiotion to the user
    /// </summary>
    public void ShowPartion() {
        if (isShown) return;

        isShown = true;

        // Go through cell which has sign
        foreach (int[] at in hasSign) {
            cells[at[0], at[1]].ReInitCell();
        }
    }
    
    /// <summary>
    /// Destroys gameobjects in this partion
    /// </summary>
    public void HidePartion() {
        if (!isShown) return;

        isShown = false;

        // Go through cell which has signs
        foreach (int[] at in hasSign) {
            cells[at[0], at[1]].RemoveCurrentCell();
        }
    }


    
    /// <summary>
    /// Return the cell's world pos based on it's local pos
    /// </summary>
    /// <param name="localCellPos">Local partion pos of cell</param>
    /// <returns>The world pos of cell</returns>
    public int[] GetWorldCellPos(int[] localCellPos) {
        int[] worldPos = new int[2];
        worldPos[0] = partionPos[0] * 10 - 10 + localCellPos[0];
        worldPos[1] = partionPos[1] * 10 - 10 + localCellPos[1];

        return worldPos;
    }
    
    /// <summary>
    /// which partion the cell is in
    /// </summary>
    /// <param name="cellPos">Gridpos of cell</param>
    /// <returns>The partion's pos</returns>
    public static int[] GetPartionPosOfCell(int[] cellPos) {
        int[] partionPos = new int[2];

        for (int i = 0; i <= 1; i++)
            if (cellPos[i] >= 0) partionPos[i] = cellPos[i] / WIDTH_HEIGHT + 1;
            else if (cellPos[i] >= -WIDTH_HEIGHT) partionPos[i] = 0;
            else partionPos[i] = (cellPos[i] + 1) / WIDTH_HEIGHT;

        return partionPos;
    }
    
    /// <summary>
    /// Converts the worldpos of cell to partion local pos
    /// </summary>
    /// <param name="worldPos">The world pos of the cell</param>
    /// <returns>The partionlocalpos of cell</returns>
    public static int[] GetLocalPosOfCell(int[] worldPos) {
        int[] pos = new int[2];
        pos[0] = worldPos[0] % WIDTH_HEIGHT;
        pos[1] = worldPos[1] % WIDTH_HEIGHT;

        // We need to reverse the pos if it is negative
        if (pos[0] < 0) pos[0] = WIDTH_HEIGHT + pos[0];
        if (pos[1] < 0) pos[1] = WIDTH_HEIGHT + pos[1];

        return pos;
    }

}
