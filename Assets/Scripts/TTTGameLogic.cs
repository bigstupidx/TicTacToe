using UnityEngine;
using System.Collections.Generic;

public class TTTGameLogic : MonoBehaviour {

    public static bool ResetFiles = false;

    // grid
    protected Grid grid;

    /// <summary>
    /// Stores whether a game is currently going on or not
    /// When it's not going on and a sign is placed it starts
    /// </summary>
    protected bool gameStarted = false;
    public bool GameSarted {
        get { return gameStarted; }
    }

    protected static Cell.CellOcc whoseTurn;
    public Cell.CellOcc WhoseTurn {
        get {
            return whoseTurn;
        }
    }

    public virtual void Start() {
        // It needs to be in start because we create the gridmanager object in start and so
        // in Execution Order i've set it so this comes after that
        grid = FindObjectOfType<Grid>();

        // Set whoseturn to a random one
        whoseTurn = Random.Range(0, 2) == 0 ? Cell.CellOcc.X : Cell.CellOcc.O;
    }

    /// <summary>
    /// Tries placing a sign at clickpos
    /// </summary>
    /// <param name="clickPos"></param>
    public void WantToPlaceAt(Vector2 clickPos) {
        int[] gridPos = Grid.GetCellInGridPos(clickPos);

        // Can be placed at
        if (grid.CanPlaceAt(gridPos)) {
            // It simply returns if it has already started
            StartNewGame(gridPos);

            // Only if the sign could be placed can the new turn come
            if (grid.PlaceSign(gridPos, WhoseTurn)) {
                bool b;
                NextTurn(gridPos, out b);
            }
        }
    }
    
    /// <summary>
    /// Sets the whoseturn correctly and handles winning condition
    /// </summary>
    /// <param name="gridPos">Where the previous sign has been placed</param>
	public virtual void NextTurn(int[] gridPos, out bool won) {
        NextPerson();

        // Check whether someone has won
        GameWonData cellWon = grid.DidWinGame(gridPos);

        // Someone has won
        if (cellWon.gameWon) {
            cellWon = grid.StopCurrentGame(cellWon);
            gameStarted = false;
            
            StartCoroutine("DrawBorderToGame", cellWon);
        }

        won = cellWon.gameWon;
    }

    /// <summary>
    /// Revert back to previous turn
    /// </summary>
    public void SetPreviousTurn() {
        PreviousPerson();
    }

    /// <summary>
    /// Current game of tic tac toe will be restarted
    /// Without reverting the sign because wedon't know what to revert to so just fuck it
    /// </summary>
    public void RestartCurrentGame() {
        gameStarted = false;
    }

    /// <summary>
    /// Sets the previous person (whoseturn)
    /// </summary>
    protected void PreviousPerson() {
        if (whoseTurn == Cell.CellOcc.X) whoseTurn = Cell.CellOcc.O;
        else whoseTurn = Cell.CellOcc.X;
    }

    /// <summary>
    /// Sets the next person (whoseturn)
    /// </summary>
    protected void NextPerson() {
        if (whoseTurn == Cell.CellOcc.X) whoseTurn = Cell.CellOcc.O;
        else whoseTurn = Cell.CellOcc.X;
    }

    protected void DrawBorderToGame(GameWonData cellWon) {
        int[,] points = cellWon.GetShapePoints();
        float[,] winLinePoints = cellWon.GetWinLinePoints();

        AddBorderToGame(points, winLinePoints, cellWon.winType);
    }

    /// <summary>
    /// Add border points in Border class
    /// </summary>
    /// <param name="points">The points</param>
    public virtual void AddBorderToGame(int[,] points, float[,] winLinePoints, Cell.CellOcc winType) {
        Border.AddBorderPoints(points, winLinePoints, winType);
    }

    /// <summary>
    /// Start the game, if it has already started it simply returns
    /// </summary>
    public virtual void StartNewGame(int[] gridPos) {
        // Do not start it if it has started already
        if (GameSarted) return;

        gameStarted = true;
    }

    /// <summary>
    /// Returns the current sprite of whoseturn
    /// </summary>
    /// <returns>May return null in case of apocalypse</returns>
    public Sprite GetCurrentSprite() {
        return SignResourceStorage.GetSpriteRelatedTo(whoseTurn);
    }

    public class GameWonData {
        public bool gameWon;
        public Cell.CellOcc winType;

        public float[,] winLine = new float[2, 2];

        /// <summary>
        /// The cellholders wich with the player won
        /// </summary>
        protected CellHolder[] holdersWithWon;
        public CellHolder[] HoldersWithWon {
            set {
                float maxDistance = int.MinValue;
                int[] arrayAt = new int[2];

                // Got through each line and search for the max length between them
                // those will be the end and start point of win line
                for (int i = 0; i < value.Length; i++) {
                    for (int k = 0; k < value.Length; k++) {
                        if (i != k) {
                            // Distance between the i and k values
                            float distance = Vector2.Distance(new Vector2(value[i].WorldPos[0], value[i].WorldPos[1]), new Vector2(value[k].WorldPos[0], value[k].WorldPos[1]));

                            if (distance > maxDistance) {
                                maxDistance = distance;
                                arrayAt[0] = i;
                                arrayAt[1] = k;
                            }
                        }
                    }
                }

                // Set winline
                winLine[0, 0] = value[arrayAt[0]].WorldPos[0];
                winLine[0, 1] = value[arrayAt[0]].WorldPos[1];
                winLine[1, 0] = value[arrayAt[1]].WorldPos[0];
                winLine[1, 1] = value[arrayAt[1]].WorldPos[1];

                // If the line is horizontal
                if (Mathf.Abs(winLine[0, 1] - winLine[1, 1]) == 0) {
                    // We need to subtract .5 from y and expand on x -1
                    winLine[0, 1] -= 0.5f; winLine[1, 1] -= 0.5f;

                    if (winLine[0, 0] < winLine[1, 0]) winLine[0, 0]--;
                    else winLine[1, 0]--;
                } else if (Mathf.Abs(winLine[0, 0] - winLine[1, 0]) == 0) { // Line is vertical
                    // We need to subtract .2 from x and expand 1 on y -1
                    winLine[0, 0] -= 0.5f; winLine[1, 0] -= 0.5f;

                    if (winLine[0, 1] < winLine[1, 1]) winLine[0, 1]--;
                    else winLine[1, 1]--;
                } else {
                    // We need to get the bigger y's x coord
                    int biggerYAt = winLine[0, 1] > winLine[1, 1] ? 0 : 1;
                    int smallerYAt = biggerYAt == 0 ? 1 : 0;
                    
                    // If this difference is bigger than 0, then the horizontal goes to right
                    // Otherwise it goes to left
                    float difference = winLine[biggerYAt, 0] - winLine[smallerYAt, 0];

                    // Horizontal goes to right
                    if (difference > 0) {
                        // We need to expand botleft by x -1 and y -1
                        winLine[smallerYAt, 0]--; winLine[smallerYAt, 1]--;
                    } else { // Otherwise horizontal goes to left
                        // Topleft x -1 BotRight y -1
                        winLine[biggerYAt, 0] -= 1;
                        winLine[smallerYAt, 1]--;
                    }
                }

                holdersWithWon = value;
            }

            get { return holdersWithWon;  }
        }

        /// <summary>
        /// Table of cells in the game. True if it has a cell, false if it does not.
        /// </summary>
        public bool[,] table;

        /// <summary>
        /// At what coordinates the game starts in gridPos in the world
        /// </summary>
        public int[] startPos;

        public float[,] GetWinLinePoints() {
            return winLine;
        }

        public int[,] GetShapePoints() {
            //if (table == null || startPos == null) return null;

            List<Line> border = new List<Line>();
            BorderedCell[,] cells = new BorderedCell[table.GetLength(0) + 2, table.GetLength(1) + 2];
            for (int i = 0; i < cells.GetLength(0); i++)
                for (int k = 0; k < cells.GetLength(1); k++) {
                    cells[i, k] = new BorderedCell(new int[] { i - 1 + startPos[0], k - 1 + startPos[1] });
                }

            // Calculating where the border should be
            for (int k = 0; k < table.GetLength(1); k++) {
                for (int i = 0; i < table.GetLength(0); i++) {
                    if (!table[i, k]) continue;

                    // Left
                    if (i == 0 || !table[i - 1, k]) {
                        cells[i, k + 1].Right = true;
                    }

                    // Right
                    if (i == table.GetLength(0) - 1 || !table[i + 1, k]) {
                        cells[i + 2, k + 1].Left = true;
                    }

                    // Bottom
                    if (k == 0 || !table[i, k - 1]) {
                        cells[i + 1, k].Top = true;
                    }

                    // Top
                    if (k == table.GetLength(1) - 1 || !table[i, k + 1]) {
                        cells[i + 1, k + 2].Bottom = true;
                    }
                }
            }
            
            for (int i = 0; i < cells.GetLength(0); i++) {
                for (int k = 0; k < cells.GetLength(1); k++) {
                    cells[i, k].RemoveThreeOrFourWalls();
                    cells[i, k].MakeDiagonalIfPossible();
                    
                    foreach (Line l in cells[i, k].GetLines()) {
                        if (l != null) {
                            border.Add(l);
                        }
                    }
                }
            }

            // At this point we have all the borders in tha border list
            // Sort the border list so the borders next to each other on the field are so in the list as well
            //TODO may want to implement some kind of other sorting algorithm
            Line[] sortedBorder = new Line[border.Count];
            sortedBorder[0] = border[0];
            border.RemoveAt(0);

            bool done = false;

            for (int at = 0; at < sortedBorder.Length - 1 && !done; at++) {
                for (int exam = 0; exam < border.Count; exam++) {
                    Line couldBe = border[exam];
                    if (couldBe.StartX == sortedBorder[at].EndX && couldBe.StartY == sortedBorder[at].EndY) {
                        sortedBorder[at + 1] = couldBe;
                        border.RemoveAt(exam);
                        break;
                    }

                    if (couldBe.EndX == sortedBorder[at].EndX && couldBe.EndY == sortedBorder[at].EndY) {
                        couldBe.SwapCoords();
                        sortedBorder[at + 1] = couldBe;
                        border.RemoveAt(exam);
                        break;
                    }
                }

                // We couldn't connect to any more borders (maybe there are more)
                if (sortedBorder[at + 1] == null)
                    done = true; // We are done with this
            }

            // Remove unnecessary points
            List<Line> reworkedSortedBorder = new List<Line>();
            reworkedSortedBorder.Add(sortedBorder[0]);
            for (int i = 1, k = 0; i < sortedBorder.Length && sortedBorder[i] != null; i++) {
                if (!(reworkedSortedBorder[k].OnLine(new int[] { sortedBorder[i].StartX, sortedBorder[i].StartY }) &&
                    reworkedSortedBorder[k].OnLine(new int[] { sortedBorder[i].EndX, sortedBorder[i].EndY}))) {
                    reworkedSortedBorder.Add(sortedBorder[i]);
                    k++;
                }
            }

            int[,] returnArray = new int[reworkedSortedBorder.Count + 1, 2];
            for (int i = 0; i < reworkedSortedBorder.Count; i++) {
                returnArray[i, 0] = reworkedSortedBorder[i].StartX;
                returnArray[i, 1] = reworkedSortedBorder[i].StartY;
            }

            returnArray[returnArray.GetLength(0) - 1, 0] = reworkedSortedBorder[0].StartX;
            returnArray[returnArray.GetLength(0) - 1, 1] = reworkedSortedBorder[0].StartY;

            return returnArray;
        }
    }

    /// <summary>
    /// Used for the calculation of the border
    /// </summary>
    public class BorderedCell {
        public int[] pos;

        protected int wallCount = 0;
        public bool HasWalls() { return wallCount > 0; }

        protected bool left = false;
        public bool Left {
            get { return left; }
            set {
                if (value && !left) wallCount++;
                else if (!value && left) wallCount--;

                this.left = value;
            }
        }
        public Line GetLeftLine() {
            if (!left) return null;
            return new Line(pos[0] - 1, pos[0] - 1, pos[1] - 1, pos[1]);
        }

        protected bool right = false;
        public bool Right {
            get { return right; }
            set {
                if (value && !right) wallCount++;
                else if (!value && right) wallCount--;

                this.right = value;
            }
        }
        public Line GetRightLine() {
            if (!right) return null;
            return new Line(pos[0], pos[0], pos[1] - 1, pos[1]);
        }

        protected bool bottom = false;
        public bool Bottom {
            get { return bottom; }
            set {
                if (value && !bottom) wallCount++;
                else if (!value && bottom) wallCount--;

                this.bottom = value;
            }
        }
        public Line GetBottomLine() {
            if (!bottom) return null;
            return new Line(pos[0] - 1, pos[0], pos[1] - 1, pos[1] - 1);
        }

        protected bool top = false;
        public bool Top {
            get { return top; }
            set {
                if (value && !top) wallCount++;
                else if (!value && top) wallCount--;

                this.top = value;
            }
        }
        public Line GetTopLine() {
            if (!top) return null;
            return new Line(pos[0] - 1, pos[0], pos[1], pos[1]);
        }

        protected bool diagonal = false;
        public bool IsDiagonal {
            get { return diagonal; }
        }
        protected Line diagonalLine = null;

        /// <summary>
        /// May return null in the array Top Right Bottom Left
        /// If it is a diagonal line then it return an array with a length of 1 with the diagonal line
        /// </summary>
        /// <returns></returns>
        public Line[] GetLines() {
            if (!diagonal) { 
                Line[] lines = new Line[4];
                lines[0] = GetTopLine(); lines[1] = GetRightLine(); lines[2] = GetBottomLine(); lines[3] = GetLeftLine();

                return lines;
            } else {
                return new Line[] { diagonalLine };
            }
        }

        public BorderedCell(int[] pos) {
            this.pos = pos;
        }

        /// <summary>
        /// If this cell has 3 or 4 walls then it removes them
        /// Replaces 3 with 1 wall
        /// </summary>
        public void RemoveThreeOrFourWalls() {
            if (wallCount >= 3) {
                Top = !Top;
                Bottom = !Bottom;
                Right = !Right;
                Left = !Left;
            }
        }

        protected void EnableDiagonal() {
            Top = false; Bottom = false; Left = false; Right = false;
            diagonal = true;
        }

        public void MakeDiagonalIfPossible() {
            if (wallCount == 2 && !diagonal && (!(top && bottom) && !(left && right))) {
                Line[] lines = GetLines();

                // index 1 and 2
                if (right && bottom)
                    diagonalLine = new Line(lines[1].EndX, lines[2].StartX, lines[1].EndY, lines[2].StartY);

                // 3 and 2
                if (left && bottom)
                    diagonalLine = new Line(lines[3].EndX, lines[2].EndX, lines[3].EndY, lines[2].EndY);

                // 3 and 0
                if (left && top)
                    diagonalLine = new Line(lines[3].StartX, lines[0].EndX, lines[3].StartY, lines[0].EndY);

                // 1 and 0
                if (right && top)
                    diagonalLine = new Line(lines[1].StartX, lines[0].StartX, lines[1].StartY, lines[0].StartY);

                EnableDiagonal();
            }
        }
    }

    public class Line {
        protected const float width = 0.1f;

        protected int startX;
        public int StartX {
            get { return startX;  }
            set {
                startX = value;
            }
        }
        protected int startY;
        public int StartY {
            get { return startY; }
            set {
                startY = value;
            }
        }
        protected int endX;
        public int EndX {
            get { return endX; }
            set {
                endX = value;
            }
        }
        protected int endY;
        public int EndY {
            get { return endY; }
            set {
                endY = value;
            }
        }

        public Line(int startX, int endX, int startY, int endY) {
            this.startX = startX;
            this.startY = startY;
            this.endX = endX;
            this.endY = endY;
        }

        public bool OnLine(int[] point) {
            return (point[0] - startX) * (endY - startY) - (point[1] - startY) * (endX - startX) == 0;
        }

        public void SwapCoords() {
            int temp = startX, temp1 = startY;
            startX = endX;
            startY = endY;
            endX = temp;
            endY = temp1;
        }

        public override string ToString() {
            return startX + " " + startY + " - " + endX + " " + endY;
        }

        public override bool Equals(object obj) {
            if (obj is Line) {
                Line lineObj = (Line) obj;

                return (startX == lineObj.startX && startY == lineObj.startY) ||
                    (startX == lineObj.endX && startY == lineObj.endY) ||
                    (endX == lineObj.startX && endY == lineObj.startY) ||
                    (endX == lineObj.endX && endY == lineObj.endY);
            }

            return base.Equals(obj);
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }

        public bool CouldBeNextLine(Line other) {
            return (endX == other.endX && endY == other.endY) ||
                (endX == other.startX && endY == other.startY);
        }
    }
}
