using System;
using System.Collections.Generic;
using UnityEngine;

public class AIScript : MonoBehaviour {

    public static Cell.CellOcc AIType = Cell.CellOcc.X;
    public static Cell.CellOcc HumanType = Cell.CellOcc.O;
    [HideInInspector]
    private int DIFFICULTY = 2;

    private Grid grid;

    /// <summary>
    /// All the signPos in aiLocalPos in the game
    /// </summary>
    private List<IntVector2> pointsInGame;
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
    private EvaluationField[,] gameField;
    private float pointsOfField = 0;

    /// <summary>
    /// Where the field's botleft is in gameField
    /// </summary>
    private IntVector2 bottomLeftPosOfField;
    /// <summary>
    /// Where the field's topright is in gameField
    /// </summary>
    private IntVector2 topRightPosOfField;

    void Start() {
        grid = GetComponent<Grid>();
        pointsInGame = new List<IntVector2>();
        Reset();

        // subscribe to events
        grid.SignWasPlaced += SignWasAdded;
        grid.SignWasRemoved += SignWasRemoved;
    }

    /// <summary>
    /// Called from grid in event when a sign was removed
    /// </summary>
    private void SignWasRemoved(int[] gridPos) {
        // RemoveLastPoint();
    }

    /// <summary>
    /// Called from grid in event when sign was placed
    /// </summary>
    private void SignWasAdded(int[] gridPos, Cell.CellOcc type) {
        IntVector2 pos = GridToLocalAIPos(gridPos);

        AddPoint(pos, type);
        Debug.Log("AI added sign at " + pos.x + " " + pos.y + " Point of field: " + pointsOfField);
    }

    /// <summary>
    /// Adds a point to both the gameField and pointsingame list
    /// </summary>
    /// <param name="pos"></param>
    private void AddPoint(IntVector2 pos, Cell.CellOcc type) {
        // We already have a pos there so just return
        // This shouldn't happen but just in case it does we are ready
        if (gameField[pos.x, pos.y].type != Cell.CellOcc.NONE) return;

        gameField[pos.x, pos.y].type = type;
        pointsInGame.Add(pos);

        // Set the bound of our game correctly
        if (pos.x < bottomLeftPosOfField.x) bottomLeftPosOfField.x = pos.x;
        if (pos.y < bottomLeftPosOfField.y) bottomLeftPosOfField.y = pos.y;
        if (pos.x > topRightPosOfField.x) topRightPosOfField.x = pos.x;
        if (pos.y > topRightPosOfField.y) topRightPosOfField.y = pos.y;

        List<PlaceData> placeData;
        NewSignPlaced(gameField, pos, out placeData, out placeData);
        pointsOfField = GetPointsFromSignsInARow(gameField, pointsInGame);
    }

    /// <summary>
    /// Returns the signsinarow points summed up in the gamefield we give it
    /// </summary>
    private float GetPointsFromSignsInARow(EvaluationField[,] field, List<IntVector2> pointsInGame) {
        float points = 0f;
        for (int i = 0; i < pointsInGame.Count; i++) {
            foreach (SignInARow signInARow in field[pointsInGame[i].x, pointsInGame[i].y].signsInARow) {
                points += signInARow.PointsWorth;
            }
        }

        return points;
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

    private int[] LocalAIToGridPos(IntVector2 pos) {
        return new int[] {
            pos.x + firstPointInGame.x - gameField.GetLength(0) / 2,
            pos.y + firstPointInGame.y - gameField.GetLength(1) / 2
        };
    }

    /// <summary>
    /// Expands gameField by 50 and copies the current elements in the centre so the addition is not disrupted
    /// </summary>
    private void ExpandGameField() {
        int expansionAmount = 50; // Should be even because i didnt think about what would happen if it was odd...
        EvaluationField[,] newField = new EvaluationField[gameField.GetLength(0) + expansionAmount, gameField.GetLength(1) + expansionAmount];

        // Update list and then we can set newField variables to true
        // We do it because it is faster then updating field first
        foreach (IntVector2 vect2 in pointsInGame) {
            Cell.CellOcc type = gameField[vect2.x, vect2.y].type;
            vect2.x += expansionAmount / 2;
            vect2.y += expansionAmount / 2;

            newField[vect2.x, vect2.y].type = type;
        }

        gameField = newField;
    }

    private int[,] checkDirections = new int[,] {
        { -1, 1 },
        { -1, 0 },
        { -1, -1 },
        { 0, -1 }
    };
    /// <summary>
    /// Adds the new signsInARow and removes the ones which overlap
    /// </summary>
    private void NewSignPlaced(EvaluationField[,] field, IntVector2 where, out List<PlaceData> placed, out List<PlaceData> removed) {
        Cell.CellOcc currentType = field[where.x, where.y].type;
        List<PlaceData> _placed = new List<PlaceData>();
        List<PlaceData> _removed = new List<PlaceData>();

        // Check how many signs there are in a row that contains the where sign
        for (int i = 0; i < checkDirections.GetLength(0); i++) {
            int count = 1;
            IntVector2 endOne = new IntVector2(where), endTwo = new IntVector2(where);

            // Go through the checkdirection direction
            for (int j = 1; j < Grid.WIN_CONDITION; j++) {
                int examineX = where.x + checkDirections[i, 0] * j;
                int examineY = where.y + checkDirections[i, 1] * j;

                // We are in bounds
                if (examineX >= 0 && examineX < field.GetLength(0) && examineY >= 0 && examineY < field.GetLength(1)) {
                    // Update blocks
                    if (j == 1)
                        foreach (SignInARow signInARow in field[examineX, examineY].signsInARow)
                            signInARow.UpdatePoints();

                    // It is the same sign
                    if (field[examineX, examineY].type == currentType) {
                        count++;
                        endOne = new IntVector2(examineX, examineY);
                    } else {
                        break;
                    }
                } else {
                    break;
                }
            }

            // Go through the opposite of checkdirection direction
            for (int j = 1; j < Grid.WIN_CONDITION; j++) {
                int examineX = where.x + -checkDirections[i, 0] * j;
                int examineY = where.y + -checkDirections[i, 1] * j;

                // We are in bounds
                if (examineX >= 0 && examineX < field.GetLength(0) && examineY >= 0 && examineY < field.GetLength(1)) {
                    // Update blocks
                    if (j == 1)
                        foreach (SignInARow signInARow in field[examineX, examineY].signsInARow)
                            signInARow.UpdatePoints();

                    // It is the same sign
                    if (field[examineX, examineY].type == currentType) {
                        count++;
                        endTwo = new IntVector2(examineX, examineY);
                    } else {
                        break;
                    }
                }
            }

            if (count < 2) continue;
            // Now we have the endpoints of this checkdirection in endpoint one and endpoint two
            SignInARow signsInARow = new SignInARow(endOne, endTwo, currentType);
            IntVector2 block1 = signsInARow.From - signsInARow.Steepness; IntVector2 block2 = signsInARow.To + signsInARow.Steepness;
            signsInARow.SetEndBlock(field[block1.x, block1.y], field[block2.x, block2.y]);
            signsInARow.UpdatePoints();

            for (int j = signsInARow.From.x, k = signsInARow.From.y; j <= signsInARow.To.x && k <= signsInARow.To.y; j += signsInARow.Steepness.x, k += signsInARow.Steepness.y) {
                SignInARow removedSignInARow = null;
                field[j, k].AddSignInARow(signsInARow, out removedSignInARow);

                _placed.Add(new PlaceData(new IntVector2(j, k), signsInARow));
                if (removedSignInARow != null) _removed.Add(new PlaceData(new IntVector2(j, k), removedSignInARow));
            }
        }

        placed = _placed; removed = _removed;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="field">Pass a copy!</param>
    /// <param name="alpha">Best for maximizer (AI)</param>
    /// <param name="beta">Best form minimizer (HUMAN)</param>
    private EvaluationResult EvaluateField(EvaluationField[,] field, Cell.CellOcc whoseTurn, int deepCount, List<IntVector2> pointsInGame, float alpha, float beta) {
        EvaluationResult result = new EvaluationResult(whoseTurn == HumanType ? int.MaxValue : int.MinValue, new IntVector2());

        if (deepCount == DIFFICULTY) {
            result.points = GetPointsFromSignsInARow(field, pointsInGame);
        } else {
            List<IntVector2> been = new List<IntVector2>();
            int pointsInGameLength = pointsInGame.Count;

            bool alphaBetaEnd = false;

            // Go through the places where we can place
            // Call NewSignPlaced with field and position where we want to place
            for (int j = 0; j < pointsInGameLength && !alphaBetaEnd; j++) {
                // In each direction
                for (int i = -1; i <= 1 && !alphaBetaEnd; i++) {
                    for (int k = -1; k <= 1 && !alphaBetaEnd; k++) {
                        // Not 0 0 and in bounds
                        if (!(i == 0 && k == 0) && pointsInGame[j].x + i >= 0 && pointsInGame[j].x + i < field.GetLength(0) && pointsInGame[j].y + k >= 0 && pointsInGame[j].y + k < field.GetLength(1)) {
                            IntVector2 pos = new IntVector2(pointsInGame[j].x + i, pointsInGame[j].y + k);

                            Debug.Log("Examining with deepnes " + deepCount + "   -   " + pos.x + " " + pos.y);

                            // if we haven't checked this position and the type of cell we are examining is NONE, so empty
                            if (!been.Contains(pos) && field[pos.x, pos.y].type == Cell.CellOcc.NONE) {
                                been.Add(pos);

                                // Data so we can revert the field back (because recursive algorithm)
                                List<PlaceData> placed;
                                List<PlaceData> removed;

                                // Set the examined cell to the current's sign
                                field[pos.x, pos.y].type = whoseTurn;
                                // Place that sign and while that's happening determine signsinarow
                                NewSignPlaced(field, pos, out placed, out removed);

                                pointsInGame.Add(new IntVector2(pos));
                                // Go recursively until DIFFICULTY
                                EvaluationResult evalResult = EvaluateField(field, SignResourceStorage.GetOppositeOfSign(whoseTurn), deepCount + 1, pointsInGame, alpha, beta);

                                // If it is human's turn we search for min value - MINIMIZER
                                if (whoseTurn == HumanType) {
                                    if (result.points > evalResult.points) {
                                        result.points = evalResult.points;
                                        result.fieldPos = new IntVector2(pos);
                                    }

                                    beta = Mathf.Min(beta, result.points);
                                    // If the points here is smaller than the best value for the parent maximizer then we don't have to search further because this minimizer
                                    // potentially has the chance of picking this minimum value, which the parent maximizer will never pick otherwise if this minimizer
                                    // doesn't pick this vaue it's only gonna pick a smaller one which is even worse for the maximizer
                                    // so just stop the search
                                    if (result.points <= alpha) {
                                        alphaBetaEnd = true;
                                    }
                                }
                                // Otherwise if it is AI's turn we search for the max points - MAXIMIZER
                                else if (whoseTurn == AIType) {
                                    if (result.points < evalResult.points) {
                                        result.points = evalResult.points;
                                        result.fieldPos = new IntVector2(pos);
                                    }

                                    alpha = Mathf.Max(alpha, result.points);
                                    // if the point is higher then the minimizer minimum then we don't need to search further because this maximizer
                                    // will surely pick a greater value for the parent minimizer, than it already has
                                    if (result.points >= beta) {
                                        alphaBetaEnd = true;
                                    }
                                }

                                // Revert the field back 
                                foreach (PlaceData data in placed)
                                    field[data.fieldPos.x, data.fieldPos.y].signsInARow.Remove(data.signInARow);
                                foreach (PlaceData data in removed)
                                    field[data.fieldPos.x, data.fieldPos.y].signsInARow.Add(data.signInARow);

                                field[pos.x, pos.y].type = Cell.CellOcc.NONE;

                                pointsInGame.RemoveAt(pointsInGame.Count - 1);
                            }
                        }
                    }
                }
            }
        }

        return result;
    }

    public int[] StartEvaluation() {
        Debug.Log("Started evaluation");
        EvaluationResult result = EvaluateField(gameField, AIType, 1, pointsInGame, int.MinValue, int.MaxValue);
        Debug.Log(result.fieldPos.x + " " + result.fieldPos.y);
        return LocalAIToGridPos(result.fieldPos);
    }

    public void Reset() {
        bottomLeftPosOfField = new IntVector2();
        topRightPosOfField = new IntVector2();
        gameField = new EvaluationField[50, 50];
        for (int i = 0; i < gameField.GetLength(0); i++)
            for (int k = 0; k < gameField.GetLength(1); k++)
                gameField[i, k] = new EvaluationField();
        firstPointInGame = null;
    }
}

internal struct EvaluationResult {
    public float points;
    public IntVector2 fieldPos;

    public EvaluationResult(float points, IntVector2 fieldPos) {
        this.points = points;
        this.fieldPos = fieldPos;
    }
}

/// <summary>
/// Used for storing which signinarows we removed or added so we can revert the field back while going through the field tree
/// </summary>
internal struct PlaceData {
    public IntVector2 fieldPos;
    public SignInARow signInARow;

    public PlaceData(IntVector2 pos, SignInARow signInARow) {
        this.fieldPos = pos;
        this.signInARow = signInARow;
    }
}

internal class EvaluationField {
    public Cell.CellOcc type = Cell.CellOcc.NONE;
    public bool nextToEvaluated = false;

    /// <summary>
    /// Stores signs next to each other that star from this sign
    /// </summary>
    public List<SignInARow> signsInARow;

    public EvaluationField() {
        signsInARow = new List<SignInARow>();
        type = Cell.CellOcc.NONE;
    }

    public EvaluationField(Cell.CellOcc type) : this() {
        this.type = type;
    }

    /// <summary>
    /// If there is a signInARow that has the same steepness then it will remove that first then add this
    /// </summary>
    public void AddSignInARow(SignInARow inARow, out SignInARow removed) {
        removed = null;
        // We remove the one with the same steepness because they overlap and so the longer one should be added (which we think is the new InARow)
        for (int i = signsInARow.Count - 1; i >= 0; i--) {
            // We have a signinarow which has the same steepness so 
            if (signsInARow[i].Steepness == inARow.Steepness) {
                removed = signsInARow[i];
                signsInARow.RemoveAt(i);
                break;
            }
        }

        signsInARow.Add(inARow);
    }

    /// <summary>
    /// Used for when we know which signInARow to remove (the remove parameter)
    /// </summary>
    public void AddSignInARow(SignInARow inARow, SignInARow remove) {
        signsInARow.Remove(remove);
        signsInARow.Add(inARow);
    }
}

internal class IntVector2 : IEquatable<IntVector2> {
    public int x;
    public int y;

    public IntVector2(IntVector2 vector) {
        this.x = vector.x;
        this.y = vector.y;
    }

    public IntVector2(int x, int y) {
        this.x = x;
        this.y = y;
    }

    public IntVector2() {
        x = 0; y = 0;
    }

    //________________________________OPERATORS______________________________
    public static IntVector2 operator +(IntVector2 first, IntVector2 second) {
        return new IntVector2(first.x + second.x, first.y + second.y);
    }

    public static IntVector2 operator +(IntVector2 vect, int number) {
        return new IntVector2(vect.x + number, vect.y + number);
    }

    public static IntVector2 operator -(IntVector2 vect1, IntVector2 vect2) {
        return new IntVector2(vect1.x - vect2.x, vect1.y - vect2.y);
    }

    public static IntVector2 operator /(IntVector2 vect, int number) {
        return new IntVector2(vect.x / number, vect.y / number);
    }

    public static bool operator ==(IntVector2 vect1, IntVector2 vect2) {
        if (object.ReferenceEquals(vect1, null)) {
            return object.ReferenceEquals(vect2, null);
        }
        if (object.ReferenceEquals(vect2, null)) {
            return object.ReferenceEquals(vect1, null);
        }
        return vect1.x == vect2.x && vect1.y == vect2.y;
    }

    public static bool operator !=(IntVector2 vect1, IntVector2 vect2) {
        if (object.ReferenceEquals(vect1, null)) {
            return !object.ReferenceEquals(vect1, null);
        }
        if (object.ReferenceEquals(vect2, null)) {
            return !object.ReferenceEquals(vect2, null);
        }
        return vect1.x != vect2.x || vect1.y != vect2.y;
    }

    /// <summary>
    /// Returns the smaller vector of the two
    /// </summary>
    public static IntVector2 GetSmaller(IntVector2 one, IntVector2 two) {
        if (one.y != two.y) {
            if (one.x < two.x) {
                return one;
            } else {
                return two;
            }
        } else {
            if (one.y < two.y) {
                return one;
            } else {
                return two;
            }
        }
    }

    public bool Equals(IntVector2 other) {
        return other == this;
    }

    public override int GetHashCode() {
        return string.Format("{0}--{1]", x, y).GetHashCode();
    }

    public override bool Equals(object obj) {
        if (obj is IntVector2) {
            return (IntVector2) obj == this;
        }
        return base.Equals(obj);
    }
}

internal class SignInARow : IEquatable<SignInARow> {
    private IntVector2 from;
    public IntVector2 From { get { return from; } }
    private IntVector2 to;
    public IntVector2 To { get { return to; } }
    private IntVector2 steepness;
    public IntVector2 Steepness { get { return steepness; } }

    private EvaluationField blockOne;
    private EvaluationField blockTwo;

    private float points;
    public float PointsWorth { get { return points; } }
    private int length;
    public int Length { get { return length; } }

    private Cell.CellOcc type = Cell.CellOcc.NONE;
    public Cell.CellOcc Type { get { return type; } }

    public SignInARow(IntVector2 one, IntVector2 two, Cell.CellOcc type) {
        this.type = type;
        SetPoints(one, two, type);
    }

    /// <summary>
    /// If you set it a third time it will rewrite the second one and so on
    /// </summary>
    public void SetEndBlock(EvaluationField blockOne, EvaluationField blockTwo) {
        this.blockOne = blockOne;
        this.blockTwo = blockTwo;
    }

    public int BlockCount() {
        Cell.CellOcc oppType = SignResourceStorage.GetOppositeOfSign(type);
        if (blockOne.type == oppType && blockTwo.type == oppType) return 2;
        else if (blockOne.type == oppType || blockTwo.type == oppType) return 1;
        else return 0;
    }

    /// <summary>
    /// DYDD
    /// DYXD
    /// DYYD
    /// All ys are smaller than x
    /// Always sets the smaller of the to vectors to be in the from vector
    /// Also sets the steepness as well
    /// Also updates points and length
    /// </summary>
    public void SetPoints(IntVector2 from, IntVector2 to, Cell.CellOcc type) {
        if (from.y != to.y) {
            if (from.x < to.x) {
                this.from = new IntVector2(from);
                this.to = new IntVector2(to);
            } else {
                this.from = new IntVector2(to);
                this.to = new IntVector2(from);
            }
        } else {
            if (from.y < to.y) {
                this.from = new IntVector2(from);
                this.to = new IntVector2(to);
            } else {
                this.from = new IntVector2(to);
                this.to = new IntVector2(from);
            }
        }

        steepness = new IntVector2((this.to.x - this.from.x), (this.to.y - this.from.y));
        steepness = steepness / Mathf.Max(Mathf.Abs(steepness.x), Mathf.Abs(steepness.y));

        UpdatePoints(type);
    }

    public void UpdatePoints(Cell.CellOcc type) {
        length = Mathf.Max(to.x - from.x, to.y - from.y);

        points = (2 - BlockCount()) * GetLengthMultiplier(length) * Mathf.Pow(length, 3) * (type == AIScript.AIType ? 1 : -1);
    }

    public void UpdatePoints() {
        UpdatePoints(type);
    }

    private float GetLengthMultiplier(int length) {
        switch (length) {
            case 2: return 0.5f;
            case 3: return 1.5f;
            case 4: return 3f;
        }

        return 1f;
    }

    public bool Equals(SignInARow other) {
        return from == other.from && to == other.to;
    }
}