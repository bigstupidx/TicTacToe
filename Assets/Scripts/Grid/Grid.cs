using UnityEngine;
using System.Collections.Generic;
using System;
using DG.Tweening;

public class Grid : MonoBehaviour {

    public delegate void SignPlaced(int[] pos, Cell.CellOcc type);
    public event SignPlaced SignWasPlacedEvent;
    public delegate void SignRemoved(int[] pos);
    public event SignRemoved SignWasRemovedEvent;

    // How many need for a win
    public static int WIN_CONDITION = 5;
    protected const float UPDATE_SHOWN_TIME = .3f;
    protected const int SHOW_BORDER = 35; // Border around camera in cells in which we should show cells
    [HideInInspector]
    public string FILE_PATH; // where it should be saved

    // Camera size in world units
    protected int[] cameraHalfSize;
    // The camera's positions: the previous frame and the current frame
    // LB = LeftBottom    TR = TopRight
    protected int[] cameraLastPosLB = new int[2];
    protected int[] cameraLastPosTR = new int[2];
    protected int[] cameraCurrPosLB = new int[2];
    protected int[] cameraCurrPosTR = new int[2];

    // Parent of cell gameobjects
    public static GameObject cellParent;

    protected TTTGameLogic gameLogic;

    // Used for stopping game, it stores where the last sign has been placed
    protected int[] previousGridPos;
    protected int[] secondToPreviousGridPos;
    protected int removeCount = 0;

    // Number of signs in current game
    protected int numberOfSignsInGame = 0;

    protected Dictionary<int[], Partion> partions = new Dictionary<int[], Partion>(new IntegerArrayComparer());
    protected List<int[]> shownPartions = new List<int[]>();

    // Last sign marker
    private LastPlacedMarkerScript lastPlacedMarker;

    protected virtual void Awake() {
        FILE_PATH = Application.persistentDataPath + "/Grid.txt";

        // Set camera size
        cameraHalfSize = new int[2];
        cameraHalfSize[1] = (int) (Camera.main.orthographicSize);
        cameraHalfSize[0] = (int) ((float) Camera.main.pixelWidth / Camera.main.pixelHeight) * cameraHalfSize[1];

        InvokeRepeating("ShowOnlyVisiblePartions", 0.1f, UPDATE_SHOWN_TIME);
        InvokeRepeating("ShowOnlyVisibleBorders", 0.2f, UPDATE_SHOWN_TIME);
        InvokeRepeating("UpdateCameraPos", 0f, 1f / 40f);

        // Parent gameobject of cells
        cellParent = new GameObject("Cells");

        // GameLogic
        gameLogic = FindObjectOfType<TTTGameLogic>();

        // Marker
        lastPlacedMarker = FindObjectOfType<LastPlacedMarkerScript>();
    }

    /// <summary>
    /// How many lines of data one coroutine should process
    /// </summary>
    private int dataProcessCount = 500;
    /// <summary>
    /// Load borders and signs from file
    /// </summary>
    public virtual void LoadFromFile() {
        try {
            string[] lines = System.IO.File.ReadAllLines(FILE_PATH);
            if (lines.Length == 0) return;

            int count = int.Parse(lines[0]);

            int coroutineCount = Mathf.CeilToInt(count / (float) dataProcessCount);
            for (int i = 0; i < coroutineCount; i++) {
                int arrayLength = i == coroutineCount - 1 ? lines.Length - 1 - (coroutineCount - 1) * dataProcessCount : dataProcessCount;

                string[] lineTemp = new string[arrayLength];
                Array.Copy(lines, 1 + i * dataProcessCount, lineTemp, 0, arrayLength);

                StartCoroutine("ProcessLoadedData", lineTemp);
            }
        } catch (Exception e) {
            Debug.LogError(e.Message + " " + e.Source + " " + e.StackTrace);
        }
    }

    /// <summary>
    /// Processes lines of loaded data and the places the signs.
    /// It is called ad a coroutine from LoadFromFile().
    /// </summary>
    private void ProcessLoadedData(string[] lines) {
        string[] line;
        int[] pos = new int[2];
        for (int i = 0; i < lines.Length; i++) {
            line = lines[i].Split(' ');

            pos[0] = int.Parse(line[0]); pos[1] = int.Parse(line[1]);
            PlaceSign(pos, (Cell.CellOcc) Enum.Parse(typeof(Cell.CellOcc), line[2]), true);
        }
    }

    /// <summary>
    /// Write borders and signs to file
    /// </summary>
    public virtual void WriteToFile() {
        List<string> allLines = new List<string>();
        int count = 0;

        foreach (Partion partion in partions.Values) { 
            foreach (CellHolder holder in partion.GetAllCellsWhichHaveSigns()) { 
                if (holder.IsDisabled) { // Only write sign which are in game
                    allLines.Add(holder.WorldPos[0] + " " + holder.WorldPos[1] + " " + holder.CurrentTemplate.cellOcc.ToString());
                    count++;
                }
            }
        }

        // add how many signs there are
        allLines.Insert(0, count.ToString());

        try {
            System.IO.File.WriteAllLines(FILE_PATH, allLines.ToArray());
        } catch (Exception e) {
            Debug.LogError(e.StackTrace);
        }
    }

    /// <summary>
    /// Updates the stored camera position
    /// Updates quite frequently
    /// </summary>
    protected void UpdateCameraPos() {
        // Store camera's curr pos to last pos pos
        cameraLastPosLB[0] = cameraCurrPosLB[0];
        cameraLastPosLB[1] = cameraCurrPosLB[1];
        cameraLastPosTR[0] = cameraCurrPosTR[0];
        cameraLastPosTR[1] = cameraCurrPosTR[1];

        // Get camera's new pos and set curr pos to it
        GetCameraCoords(out cameraCurrPosLB, out cameraCurrPosTR);
    }

    /// <summary>
    /// Is the given gridpos in the camera's sight
    /// </summary>
    public bool IsInCameraSight(int[] gridPos) {
        return gridPos[0] >= cameraCurrPosLB[0] + SHOW_BORDER && gridPos[0] <= cameraCurrPosTR[0] - SHOW_BORDER &&
            gridPos[1] >= cameraCurrPosLB[1] + SHOW_BORDER && gridPos[1] <= cameraCurrPosTR[1] - SHOW_BORDER;
    }

    /// <summary>
    /// This is called from the ui
    /// </summary>
    public void RemoveLastSignUI() {
        RemoveLastSign();
    }

    /// <summary>
    /// Removes the sign we last placed
    /// </summary>
    public virtual bool RemoveLastSign() {
        if (numberOfSignsInGame < 1 || removeCount > 0) return false; // Only remove if we placed two signs already

        // Remove sign
        CellHolder cellHolder = GetCellHolderAtGridPos(previousGridPos);
        if (!cellHolder.IsFull()) return false; // Return if we don't have a sign there

        // At this point we surely are going to remove the sign
        if (SignWasRemovedEvent != null)
            SignWasRemovedEvent(previousGridPos);

        cellHolder.RemoveCurrentCellWithoutStoring();

        // move marker, do this before changin turns because we want the color to be the exact opposite of the sign at secondToPrevious pos
        if (lastPlacedMarker != null)
            lastPlacedMarker.MoveMarkerTo(new Vector2(secondToPreviousGridPos[0], secondToPreviousGridPos[1]), SignResourceStorage.Instance.GetColorRelatedTo(gameLogic.WhoseTurn));

        // Revert back to previous turn in gamelogic
        gameLogic.SetPreviousTurn();

        // The order of these are very crucial
        // We just strated a game so we want to restart it
        if (numberOfSignsInGame == 1) {
            gameLogic.RestartCurrentGame();
            previousGridPos = null;
        } else {
            previousGridPos[0] = secondToPreviousGridPos[0]; previousGridPos[1] = secondToPreviousGridPos[1];
        }
        
        numberOfSignsInGame--;
        removeCount++;
        

        return true;
    }

    /// <summary>
    /// Brings back camera to last placed sign
    /// </summary>
    public virtual void SetCameraToPreviousSign() {
        if (previousGridPos != null)
            Camera.main.transform.DOMove(new Vector3(previousGridPos[0], previousGridPos[1], Camera.main.transform.position.z), 1f, false);
    }

    /// <summary>
    /// which borders should be visible
    /// Updates quite frequently
    /// </summary>
    void ShowOnlyVisibleBorders() {
        if (!HasCameraMovedSinceLastFrame()) return;

        Border.UpdateBordersShown(cameraCurrPosLB, cameraCurrPosTR);
    }

    /// <summary>
    /// Calculates which partions should not be shown
    /// Updates quite frequently
    /// </summary>
    void ShowOnlyVisiblePartions() {
        if (!HasCameraMovedSinceLastFrame()) return;

        int[] leftBottomPart = Partion.GetPartionPosOfCell(cameraCurrPosLB);
        int[] topRightPart = Partion.GetPartionPosOfCell(cameraCurrPosTR);

        // Hide partions we can't see anymore
        for (int i = shownPartions.Count - 1; i >= 0; i--) {
            int[] at = shownPartions[i];

            // Out of visibility range
            if ((at[0] < leftBottomPart[0] || at[0] > topRightPart[0]) ||
                (at[1] < leftBottomPart[1] || at[1] > topRightPart[1])) {

                Partion p;
                partions.TryGetValue(at, out p);

                if (p != null) {
                    p.HidePartion();
                    shownPartions.RemoveAt(i);
                }
            }
        }

        // Show partions which are visible
        for (int i = leftBottomPart[0]; i <= topRightPart[0]; i++) {
            for (int k = leftBottomPart[1]; k <= topRightPart[1]; k++) {
                int[] temp = new int[] { i, k };

                Partion p;
                partions.TryGetValue(temp, out p);

                if (p != null && !p.IsShown) {
                    p.ShowPartion();
                    shownPartions.Add(temp);
                }
            }
        }
    }

    /// <summary>
    /// Returns the position of camera's viewport's lefbottom and topright position
    /// </summary>
    /// <param name="leftBottomPart"></param>
    /// <param name="topRightPart"></param>
    protected void GetCameraCoords(out int[] leftBottomPart, out int[] topRightPart) {
        Vector2 cameraPos = Camera.main.transform.position;

        int[] leftBottomPartL = new int[] {
            (int) (cameraPos.x - (cameraHalfSize[0])),
            (int) (cameraPos.y - (cameraHalfSize[1]))
        };
        int[] topRightPartL = new int[] {
            (int) (cameraPos.x + (cameraHalfSize[0])),
            (int) (cameraPos.y + (cameraHalfSize[1]))
        };
        leftBottomPartL[0] -= SHOW_BORDER; leftBottomPartL[1] -= SHOW_BORDER;
        topRightPartL[0] += SHOW_BORDER; topRightPartL[1] += SHOW_BORDER;

        leftBottomPart = leftBottomPartL;
        topRightPart = topRightPartL;
    }

    /// <summary>
    /// Returns whether camera moved since last frame
    /// </summary>
    protected bool HasCameraMovedSinceLastFrame() {
        return cameraCurrPosLB[0] != cameraLastPosLB[0] || cameraCurrPosLB[1] != cameraLastPosLB[1];
    }

    /// <summary>
    /// Places sign at gridpos pos of type cellType
    /// </summary>
    /// <param name="gridPos">Grid pos of click</param>
    /// <param name="cellType"></param>
    /// <returns> whether the sign could be placed or not></returns>
    public virtual bool PlaceSign(int[] gridPos, Cell.CellOcc cellType, bool disabled = false) {
        // The partion's pos
        int[] partionPos = Partion.GetPartionPosOfCell(gridPos);

        // The cell's partion
        Partion currP;

        // We don't have a partion yet for this pos
        if (!partions.ContainsKey(partionPos)) {
            currP = new Partion(partionPos);

            partions.Add(partionPos, currP);
        } else { // we already store the partion in the dictionary
            partions.TryGetValue(partionPos, out currP);
        }

        // At this point we should surely have the partion in p

        // We need to convert the click's world pos to partion local pos
        int[] localPos = Partion.GetLocalPosOfCell(gridPos);

        // We need to place the sign in the partion
        bool couldBePlaced = currP.PlaceSign(localPos, cellType, disabled);
        if (couldBePlaced && !disabled) { // If we could place the sign store it's gridPos
            if (previousGridPos != null) secondToPreviousGridPos = new int[] { previousGridPos[0], previousGridPos[1] };
            previousGridPos = new int[] { gridPos[0], gridPos[1] };

            // Increase amount of cells in game
            numberOfSignsInGame++;
            removeCount = 0; // Reset removecount to be able to remove sign again

            // move marker
            if (lastPlacedMarker != null)
                lastPlacedMarker.MoveMarkerTo(new Vector2(gridPos[0], gridPos[1]), SignResourceStorage.Instance.GetColorRelatedTo(gameLogic.WhoseTurn == Cell.CellOcc.X ? Cell.CellOcc.O : Cell.CellOcc.X));

            if (SignWasPlacedEvent != null)
                SignWasPlacedEvent(gridPos, cellType);
        }
        return couldBePlaced;
    }

    /// <param name="gridPos">Where the placing is examined in gridpos</param>
    /// <returns>Whether the user can place at gridPos</returns>
    public bool CanPlaceAt(int[] gridPos) {
        // Game has not started, explained in GameLogic
        CellHolder ch = GetCellHolderAtGridPos(gridPos);
        if (!gameLogic.GameSarted) {
            return ch == null || !ch.IsDisabled;
        }

        if (ch != null && ch.IsDisabled) return false;

        CellHolder[] holders = GetAllNeighbourCellHolders(gridPos);
        foreach (CellHolder holder in holders) {
            if (holder != null && !holder.IsDisabled && holder.IsFull()) {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// It disables all of the sign which belong to this game
    /// </summary>
    /// <param name="wonData">More data like cellgrid of current game pass in a wonData</param>
    public TTTGameLogic.GameWonData StopCurrentGame(TTTGameLogic.GameWonData wonData) {
        // We need to start a bejaras from the last placed sign's pos, which we happen to store in previousGridPos
        // Then set all of the found sign's cellholder to disabled

        // Disable marker
        if (lastPlacedMarker != null)
            lastPlacedMarker.Disable();

        // Ended game so reset the amount of cells in game
        numberOfSignsInGame = 0;

        // Store all of the cellholders which have signs in game

        List<CellHolder> listOfCells = new List<CellHolder>();
        listOfCells.Add(GetCellHolderAtGridPos(previousGridPos));

        Queue<CellHolder> queue = new Queue<CellHolder>();
        queue.Enqueue(listOfCells[0]);

        // Stores the min and max values of x and y (essentially bottomleft and topright corner points)
        int[] min = new int[] { int.MaxValue, int.MaxValue };
        int[] max = new int[] { int.MinValue, int.MinValue };

        // Go through cells in this game
        while (queue.Count > 0) {
            CellHolder currCH = queue.Dequeue();
            if (currCH.IsDisabled) break;

            // Store the min and max of x and y we examine
            if (currCH.WorldPos[0] < min[0]) min[0] = currCH.WorldPos[0];
            if (currCH.WorldPos[0] > max[0]) max[0] = currCH.WorldPos[0];
            if (currCH.WorldPos[1] < min[1]) min[1] = currCH.WorldPos[1];
            if (currCH.WorldPos[1] > max[1]) max[1] = currCH.WorldPos[1];

            // Disable cell
            currCH.Disable();

            // Get all af nthe neighbours af current cell
            CellHolder[] neighbours = GetAllNeighbourCellHolders(currCH.WorldPos);

            foreach (CellHolder ch in neighbours) {
                if (ch != null && !ch.IsDisabled && ch.IsFull()) { // Found a cell which belongs to this game
                    if (!queue.Contains(ch) && !listOfCells.Contains(ch)) {
                        queue.Enqueue(ch);
                        listOfCells.Add(ch);
                    }
                }
            }
        }

        wonData.table = new bool[Mathf.Abs(max[0] - min[0] + 1), Mathf.Abs(max[1] - min[1] + 1)];

        // Fill the thingy with true where there is a cell. Thingy defined in WonData class
        foreach (CellHolder ch in listOfCells) {
            wonData.table[ch.WorldPos[0] - min[0], ch.WorldPos[1] - min[1]] = true;
        }
        wonData.startPos = min;

        // Now we can fill the holes
        List<int[]> holes = wonData.GetFillableHoles();
        foreach (int[] pos in holes) {
            // Get the partion the cell is in (we surely have this one, because holes are between signs
            int[] partionPos = Partion.GetPartionPosOfCell(pos);
            Partion p;

            // We don't have a partion yet for this pos
            if (!partions.ContainsKey(partionPos)) {
                p = new Partion(partionPos);

                partions.Add(partionPos, p);
            } else { // we already store the partion in the dictionary
                partions.TryGetValue(partionPos, out p);
            }

            // Place new BLOCKED cell in partion at pos
            p.PlaceSign(Partion.GetLocalPosOfCell(pos), Cell.CellOcc.BLOCKED, true);
        }

        return wonData;
    }


    /// <summary>
    /// Returns whether someone has won the game based on the placement of a sign
    /// So it should be called after a sign has been placed
    /// </summary>
    /// <param name="gridPos">Where the sign has been placed</param>
    /// <returns>Returns BLOCKED when no one won yet</returns>
    public TTTGameLogic.GameWonData DidWinGame(int[] gridPos) {
        CellHolder currCellHolder = GetCellHolderAtGridPos(gridPos);
        Cell.CellOcc currCellType = currCellHolder.CurrentTemplate.cellOcc;

        // Used for return data
        TTTGameLogic.GameWonData gameWonData = new TTTGameLogic.GameWonData();
        CellHolder[] winCells = new CellHolder[WIN_CONDITION];
        winCells[0] = currCellHolder;

        // Go through directions
        for (int i = 0; i <= 1; i++) {
            for (int k = -1; k <= 1; k++) {
                if (!(k == 0 && i == 0) && !(i == 0 && k == 1)) { // Dont want 0 0 direction or up dir

                    int count = 1; // Used to determine whether someone has won: if after the loop it is WIN_CONDITION someone has won

                    // Go till we found end in this direction or founf out that someone has won
                    for (int j = 1; j < WIN_CONDITION && count < WIN_CONDITION; j++) {
                        CellHolder ch = GetCellHolderRelativeTo(gridPos, i * j, k * j);

                        // ch is null  
                        // OR  ch is not full  
                        // OR  ch is disabled  
                        // OR  cell type is not the one we have in this cell
                        if (!(ch != null && ch.IsFull() && !ch.IsDisabled && ch.CurrentTemplate.cellOcc == currCellType)) {
                            break;
                        }

                        winCells[count] = ch;
                        count++;
                    }

                    // We need to go in the other direction as well
                    for (int j = 1; j < WIN_CONDITION && count < WIN_CONDITION; j++) {
                        CellHolder ch = GetCellHolderRelativeTo(gridPos, -i * j, -k * j);

                        // ch is null  
                        // OR  ch is not full  
                        // OR  ch is disabled  
                        // OR  cell type is not the one we have in this cell
                        if (!(ch != null && ch.IsFull() && !ch.IsDisabled && ch.CurrentTemplate.cellOcc == currCellType)) {
                            break;
                        }

                        winCells[count] = ch;
                        count++;
                    }

                    if (count >= WIN_CONDITION) {
                        gameWonData.gameWon = true;
                        gameWonData.winType = currCellType;
                        gameWonData.HoldersWithWon = winCells;
                        return gameWonData;
                    }
                }
            }
        }

        gameWonData.gameWon = false;
        return gameWonData;
    }

    /// <summary>
    /// Converts worldclickpos of cell to grid pos
    /// </summary>
    /// <param name="wordClickPos"></param>
    /// <returns></returns>
    public static int[] GetCellInGridPos(Vector2 wordClickPos) {
        int[] gridPos = new int[2];
        gridPos[0] = Mathf.CeilToInt(wordClickPos[0]);
        gridPos[1] = Mathf.CeilToInt(wordClickPos[1]);

        return gridPos;
    }

    /// <summary>
    /// Return cellHolder at gridPos
    /// </summary>
    /// <param name="gridPos">Position of cell in grid</param>
    /// <returns>May return null</returns>
    public CellHolder GetCellHolderAtGridPos(int[] gridPos) {
        int[] partionPos = Partion.GetPartionPosOfCell(gridPos);
        int[] partionLocalPos = Partion.GetLocalPosOfCell(gridPos);

        Partion p;
        partions.TryGetValue(partionPos, out p);
        if (p != null) {
            return p.GetCellHolderAt(partionLocalPos);
        }

        return null;
    }

    /// <summary>
    /// The neighbours start from bottomleft and go first right then up in rows
    /// </summary>
    /// <param name="gridPos">The gridpos of the cell of which we want the neighbours</param>
    /// <returns>An array with a length of 8 in the order seen above</returns>
    public CellHolder[] GetAllNeighbourCellHolders(int[] gridPos) {
        CellHolder[] neighbours = new CellHolder[8];
        int at = 0;
        for (int i = -1; i <= 1; i++) {
            for (int k = -1; k <= 1; k++) {
                if (!(k == 0 && i == 0)) { // We don't want to return the cell itself
                    neighbours[at] = GetCellHolderRelativeTo(gridPos, i, k);
                    at++;
                }
            }
        }

        return neighbours;
    }

    /// <summary>
    /// The neighbours are in this order: Top Right Bottom Left
    /// </summary>
    /// <param name="gridPos">The gridpos of the cell of which we want the neighbours</param>
    /// <returns>An array with a length of 4 in the order seen above</returns>
    public CellHolder[] GetNeighbourCellHolders(int[] gridPos) {
        CellHolder[] holders = new CellHolder[4];
        holders[0] = GetUpwardCellHolder(gridPos);
        holders[1] = GetRightCellHolder(gridPos);
        holders[2] = GetDownwardCellHolder(gridPos);
        holders[3] = GetLeftCellHolder(gridPos);

        return holders;
    }

    /// <summary>
    /// Gets the right cellholder of the given gridpos cell
    /// </summary>
    /// <param name="gridPos"></param>
    /// <returns>Returns null if the cellholder does not exists</returns>
    public CellHolder GetRightCellHolder(int[] gridPos) {
        return GetCellHolderRelativeTo(gridPos, 1, 0);
    }

    /// <summary>
    /// Gets the left cellholder of the given gridpos cell
    /// </summary>
    /// <param name="gridPos"></param>
    /// <returns>Returns null if the cellholder does not exists</returns>
    public CellHolder GetLeftCellHolder(int[] gridPos) {
        return GetCellHolderRelativeTo(gridPos, -1, 0);
    }

    /// <summary>
    /// Gets the upward cellholder of the given gridpos cell
    /// </summary>
    /// <param name="gridPos"></param>
    /// <returns>Returns null if the cellholder does not exists</returns>
    public CellHolder GetUpwardCellHolder(int[] gridPos) {
        return GetCellHolderRelativeTo(gridPos, 0, 1);
    }

    /// <summary>
    /// Gets the downward cellholder of the given gridpos cell
    /// </summary>
    /// <param name="gridPos"></param>
    /// <returns>Returns null if the cellholder does not exists</returns>
    public CellHolder GetDownwardCellHolder(int[] gridPos) {
        return GetCellHolderRelativeTo(gridPos, 0, -1);
    }

    /// <summary>
    /// Gets the cellholder of the cell relative to gridpos with the eltolas of x and y on those axis
    /// Currently only using it for right, left, up and down
    /// In other cases may crash so be aware
    /// </summary>
    /// <param name="gridPos">Grid pos of cell</param>
    /// <param name="x">X axis relative</param>
    /// <param name="y">Y axis relative</param>
    /// <returns>Returns null if can't find cellholder</returns>
    public CellHolder GetCellHolderRelativeTo(int[] gridPos, int x, int y) {
        int[] relativePos = new int[] {
            gridPos[0] + x,
            gridPos[1] + y
        };

        // gets partion and local pos of relative cell
        int[] partionPos = Partion.GetPartionPosOfCell(relativePos);
        int[] partionLocalPos = Partion.GetLocalPosOfCell(relativePos);

        Partion p;
        partions.TryGetValue(partionPos, out p);
        // There is no partion at this coord yet
        if (p == null) return null;

        return p.GetCellHolderAt(partionLocalPos);
    }

    /// <summary>
    /// Used for array if partions
    /// </summary>
    public class IntegerArrayComparer : IEqualityComparer<int[]> {
        public bool Equals(int[] x, int[] y) {
            if (x.Length != y.Length) {
                return false;
            }
            for (int i = 0; i < x.Length; i++) {
                if (x[i] != y[i]) {
                    return false;
                }
            }
            return true;
        }

        public int GetHashCode(int[] obj) {
            int result = 17;
            for (int i = 0; i < obj.Length; i++) {
                unchecked {
                    result = result * 23 + obj[i];
                }
            }
            return result;
        }
    }

    public struct SignStoreInFile {
        public int x;
        public int y;
        public string type;

        public SignStoreInFile(int x, int y, string type) {
            this.x = x;
            this.y = y;
            this.type = type;
        }
    }

}
