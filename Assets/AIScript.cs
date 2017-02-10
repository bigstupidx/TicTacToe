using System.Collections.Generic;
using UnityEngine;

public class AIScript : MonoBehaviour {

    public Cell.CellOcc AIType = Cell.CellOcc.X;
    public Cell.CellOcc HumanType = Cell.CellOcc.O;
    public int DIFFICULTY = 10;

    private Grid grid;

    /// <summary>
    /// All the signPos in aiLocalPos in the game
    /// </summary>
    private Stack<IntVector2> pointsInGame;
    /// <summary>
    /// The first point in current game, used for exchanging gridPos to aiLocalPos
    /// This is 0, 0
    /// </summary>
    private IntVector2 firstPointInGame;
    /// <summary>
    /// Stores the current gamefield
    /// if there is no sign there it is NONE
    /// At first it is a 51 * 51 array, if it is exceeded then it will be replaced in a (x + 50) * (x + 50)
    /// 0, 0 is at [length / 2, length / 2]
    /// </summary>
    private Cell.CellOcc[,] gameField;

    /// <summary>
    /// Where the field's botleft is in gameField
    /// </summary>
    private IntVector2 bottomLeftPosOfField;
    /// <summary>
    /// Where the field's topright is in gameField
    /// </summary>
    private IntVector2 topRightPosOfField;

    void Start () {
        grid = GetComponent<Grid>();
        pointsInGame = new Stack<IntVector2>();
        bottomLeftPosOfField = new IntVector2();
        topRightPosOfField = new IntVector2();
        gameField = new Cell.CellOcc[50, 50];

        // subscribe to events
        grid.SignWasPlaced += SignWasAdded;
        grid.SignWasRemoved += SignWasRemoved;
	}

    /// <summary>
    /// Called from grid in event when a sign was removed
    /// </summary>
    private void SignWasRemoved(int[] gridPos) {
        RemoveLastPoint();
    }

    /// <summary>
    /// Called from grid in event when sign was placed
    /// </summary>
    private void SignWasAdded(int[] gridPos, Cell.CellOcc type) {
        IntVector2 pos = GridToLocalAIPos(gridPos);

        AddPoint(pos, type);
    }

    /// <summary>
    /// Adds a point to both the gameField and pointsingame list
    /// </summary>
    /// <param name="pos"></param>
    private void AddPoint(IntVector2 pos, Cell.CellOcc type) {
        // We already have a pos there so just return
        // This shouldn't happen but just in case it does we are ready
        if (gameField[pos.x, pos.y] != Cell.CellOcc.NONE) return;

        gameField[pos.x, pos.y] = type;
        pointsInGame.Push(pos);

        // Set the bound of our game correctly
        if (pos.x < bottomLeftPosOfField.x) bottomLeftPosOfField.x = pos.x;
        if (pos.y < bottomLeftPosOfField.y) bottomLeftPosOfField.y = pos.y;
        if (pos.x > topRightPosOfField.x) topRightPosOfField.x = pos.x;
        if (pos.y > topRightPosOfField.y) topRightPosOfField.y = pos.y;

    }

    /// <summary>
    /// Removes the last point we added
    /// </summary>
    private void RemoveLastPoint() {
        // We use a stack for this reason only, so we can get the same element from the top
        // we simply need to get the element from queue
        IntVector2 pos = pointsInGame.Pop();
        gameField[pos.x, pos.y] = Cell.CellOcc.NONE;
    }

    /// <summary>
    /// If there is no firstPointInGame it will assign the given gridPos as that
    /// </summary>
    /// <param name="gridPos"></param>
    private IntVector2 GridToLocalAIPos(int[] gridPos) {
        if (firstPointInGame == null) {
            firstPointInGame = new IntVector2(gridPos[0], gridPos[1]);
        }

        // Get the point relative to firstpointingame then add the half of the array length in order to have 0, 0 at the middle of array
        return new IntVector2(gridPos[0] - firstPointInGame.x + gameField.GetLength(0) / 2, gridPos[1] - firstPointInGame.y + gameField.GetLength(1) / 2);
    }

    /// <summary>
    /// Expands gameField by 50 and copies the current elements in the centre so the addition is not disrupted
    /// </summary>
    private void ExpandGameField() {
        int expansionAmount = 50; // Should be even because i didnt think about what would happen if it was odd...
        Cell.CellOcc[,] newField = new Cell.CellOcc[gameField.GetLength(0) + expansionAmount, gameField.GetLength(1) + expansionAmount];

        // Update list and then we can set newField variables to true
        // We do it because it is faster then updating field first
        foreach (IntVector2 vect2 in pointsInGame) {
            Cell.CellOcc type = gameField[vect2.x, vect2.y];
            vect2.x += expansionAmount / 2;
            vect2.y += expansionAmount / 2;

            newField[vect2.x, vect2.y] = type;
        }

        gameField = newField;
    }

    private int[,] checkDirections = new int[,] {
        { 1 , 0 },
        { -1 , 0 },
        { 1 , 1 },
        { 0 , 1 },
        { -1 , 1 }
    };
    private float EvaluateField(Cell.CellOcc[,] field, Cell.CellOcc whoseTurnNext) {
        // Make our own array because we need some extra variables for evaluation
        EvaluationField[,] myField = new EvaluationField[field.GetLength(0), field.GetLength(1)];
        for (int i = 0; i < field.GetLength(0); i++)
            for (int k = 0; k < field.GetLength(1); k++)
                myField[i, k] = new EvaluationField(field[i, k]);

        float AIPoint = 0f;
        float humanPoint = 0f;

        for (int i = 0; i < myField.GetLength(0); i++) {
            for (int k = 0; k < myField.GetLength(1); k++) {
                // Let's check how many signs are next to each other first

                // Only do it of it is a sign
                if (myField[i, k].type != AIType && myField[i, k].type != HumanType) continue;

                // If we go through the array in order there is no way we encounter a sign which is connected to others in the middle of a connection
                // so we don't need to go each way just simply one
                // and we only need to check upwards and sideways
                for (int j = 0; j < checkDirections.GetLength(0); j++) {
                    int inARow = 1;
                    int blocks = 0; // How many blocks are at the end

                    // Do not check if the one the other way is checked because then we already checked this by going through the array in order
                    // Sadly first we have to check these to not have an arrayoutofboundsexception
                    EvaluationField otherWayField = null;
                    if (i - checkDirections[j, 0] >= 0 && i - checkDirections[j, 0] < myField.GetLength(0) && k - checkDirections[j, 1] >= 0 && k - checkDirections[j, 1] < myField.GetLength(1))
                        otherWayField = myField[i - checkDirections[j, 0], k - checkDirections[j, 1]];
                    if (otherWayField != null && otherWayField.nextToEvaluated) continue;
                    if (otherWayField != null && otherWayField.type == HumanType) blocks++;

                    // So now we can start checking in this given direction
                    for (int l = 1; l <= 5; l++) {
                        int checkX = i + checkDirections[j, 0] * l;
                        int checkY = k + checkDirections[j, 1] * l;
                        // We ran out of the array
                        if (checkX < 0 || checkX >= myField.GetLength(0) || checkY < 0 || checkY > myField.GetLength(1))
                            break;

                        // We have one in this row
                        if (myField[checkX, checkY].type == myField[i, k].type)
                            inARow++;
                        else { // Otherwise the row streak was broken so break the counting loop
                            if (myField[checkX, checkY].type == HumanType) // check the other end for blocks
                                blocks++;
                            break;
                        }
                        
                    }
                    
                    // Calculatin points
                    float times = 1;
                    switch (blocks) {
                        case 2: times = 0.5f; break;
                        case 1: times = 1.5f; break;
                        case 0: times = 3f; break;
                    }
                    float score = (Mathf.Pow(inARow, 3) * times);
                    if (myField[i, k].type == AIType) AIPoint += score;
                    else humanPoint += score;
                }
            }
        }

        // TODO defense points that can be done inside loop when we detect blocks

        return AIPoint - humanPoint;
    }
}

class EvaluationField {
    public Cell.CellOcc type;
    public bool nextToEvaluated = false;

    public EvaluationField(Cell.CellOcc type) {
        this.type = type;
    }
}

class IntVector2 {
    public int x;
    public int y;

    public IntVector2(int x, int y) {
        this.x = x;
        this.y = y;
    }

    public IntVector2() {
        x = 0; y = 0;
    }

    public static IntVector2 operator +(IntVector2 first, IntVector2 second) {
        return new IntVector2(first.x + second.x, first.y + second.y);
    }

    public static IntVector2 operator +(IntVector2 vect, int number) {
        return new IntVector2(vect.x + number, vect.y + number);
    }
}