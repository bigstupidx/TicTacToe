using System;
using System.Collections.Generic;
using UnityEngine;

public class AIScript : MonoBehaviour {

    public static Cell.CellOcc AIType = Cell.CellOcc.X;
    public static Cell.CellOcc HumanType = Cell.CellOcc.O;
    [HideInInspector]
    private int DIFFICULTY = 2;
    public int Difficulty { get { return DIFFICULTY; } }

    private int DEEP_MAX = 2;
    /// <summary>
    /// What's the chance that the ai simply skips a place where it could place, it just doesn't examine it. 
    /// It will also add some depending on the points in game (heuristic for the game's complexity)
    /// </summary>
    private float leaveOutChance = 0.25f;

    /// <summary>
    /// After how many sign the AI should miss 100%
    /// </summary>
    private int afterSignCountMiss = 200;

    private Grid grid;
    private AITTTGameLogic gameLogic;
    private System.Random rand;

    /// <summary>
    /// All the signPos in aiLocalPos in the game
    /// </summary>
    private List<IntVector2> pointsInGame;
    /// <summary>
    /// The first point in current game, used for exchanging gridPos to aiLocalPos<para />
    /// This is 0, 0
    /// </summary>
    private IntVector2 firstPointInGame;
    /// <summary>
    /// Stores the current gamefield<para />
    /// if there is no sign there it is NONE<para />
    /// At first it is a 51 * 51 array, if it is exceeded then it will be replaced in a (x + 50) * (x + 50)<para />
    /// 0, 0 is at [length / 2, length / 2]<para />
    /// </summary>
    private EvaluationField[,] gameField;

    /// <summary>
    /// Where the field's botleft is in gameField
    /// </summary>
    private IntVector2 bottomLeftPosOfField;
    /// <summary>
    /// Where the field's topright is in gameField
    /// </summary>
    private IntVector2 topRightPosOfField;

    /// <summary>
    /// Top right position of all the signs so far
    /// </summary>
    private IntVector2 topRightAll = new IntVector2();
    /// <summary>
    /// Bottom left position of all the signs so far
    /// </summary>
    private IntVector2 bottomLeftAll = new IntVector2();

    private bool gameInProgress = false;

    void Start() {
        rand = new System.Random();
        grid = GetComponent<Grid>();
        gameLogic = GetComponent<AITTTGameLogic>();
        Reset();

        // subscribe to events
        grid.SignWasPlacedEvent += SignWasAdded;
        grid.SignWasRemovedEvent += SignWasRemoved;
        gameLogic.SomeoneWonGameEvent += SomeoneWonGame;
    }

    /// <summary>
    /// Returns whether the game is currently in progress
    /// </summary>
    /// <returns></returns>
    public bool IsGameInProgress() {
        return pointsInGame.Count > 1 && gameInProgress;
    }

    /// <summary>
    /// Event for when someone has won the game
    /// </summary>
    private void SomeoneWonGame(Cell.CellOcc type) {
        gameInProgress = false;

        if (type == HumanType) {
            PreferencesScript.Instance.PullExpBarThenAdd((int) GetGameWonExp());
        } else {
            PreferencesScript.Instance.PullExpBarThenAdd((int) GetGameLostExp());
        }
    }

    private int[,] playerExpTable = new int[,] {
        { 0, 0, 0 },
        { 0, 0, 0 },
        { 0, 0, 0 },
        { 75, 20, 5 },
        { 100, 45, 15 }
    };
    /// <summary>
    /// How much exp the player gets for winning this game
    /// </summary>
    private float GetGameWonExp() {
        // Add for every sign in the game
        float exp = pointsInGame.Count * 1.7f;

        // Add for base for playing/winning
        exp += 270;

        // Add for every player's signinarow
        for (int i = 0; i < pointsInGame.Count; i++) {
            foreach (SignInARow signInARow in gameField[pointsInGame[i].x, pointsInGame[i].y].signsInARow) {
                int length = signInARow.Length;

                if (signInARow.Type == HumanType) {
                    if (length >= 3 && length <= 4) exp += playerExpTable[length, signInARow.BlockCount()];
                } else {
                    exp += signInARow.BlockCount() * 0.5f * length; // Add for every blocked
                }
            }
        }

        // Difficulty
        return exp * (1f - Mathf.Min(leaveOutChance * 1.6f, 1f));
    }

    /// <summary>
    /// How much exp the player gets for losing this game
    /// </summary>
    /// <returns></returns>
    private float GetGameLostExp() {
        return GetGameWonExp() * 0.3f;
    }

    /// <summary>
    /// 0 - Baby; 1 - Easy; 2 - Normal; 3 - Hard; 4 - Impossible
    /// </summary>
    public void SetDifficulty(int diff) {
        DIFFICULTY = diff;
        switch (diff) {
            case 0:
                leaveOutChance = 0.8f;
                break;
            case 1:
                leaveOutChance = 0.57f;
                break;
            case 2:
                leaveOutChance = 0.25f;
                break;
            case 3:
                leaveOutChance = 0.05f;
                break;
            case 4:
                leaveOutChance = 0f;
                break;
        }
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
        // first point placed in this game
        if (firstPointInGame == null) {
            gameInProgress = true;

            SetLocalGridDisabled(new int[] { gridPos[0] - gameField.GetLength(0) / 2, gridPos[1] - gameField.GetLength(1) / 2 });
        }

        // Set the all bounds if needed
        if (gridPos[0] < bottomLeftAll.x) bottomLeftAll.x = gridPos[0];
        if (gridPos[1] < bottomLeftAll.y) bottomLeftAll.y = gridPos[1];
        if (gridPos[0] > topRightAll.x) topRightAll.x = gridPos[0];
        if (gridPos[1] > topRightAll.y) topRightAll.y = gridPos[1];
 
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
    }

    /// <summary>
    /// Returns the signsinarow points summed up in the gamefield we give it
    /// </summary>
    private float GetPointsFromSignsInARow(EvaluationField[,] field, List<IntVector2> pointsInGame, IntVector2 lastPlaced, out float aiPoint, out float humanPoint) {
        for (int i = lastPlaced.x - Grid.WIN_CONDITION; i <= lastPlaced.x + Grid.WIN_CONDITION; i++) {
            for (int j = lastPlaced.y - Grid.WIN_CONDITION; j <= lastPlaced.y + Grid.WIN_CONDITION; j++) {
                foreach (SignInARow signInARow in field[i, j].signsInARow) {
                    signInARow.UpdatePoints();
                }
            }
        }

        // Update end blocks
        float aiP = 0f, humP = 0f;
        for (int i = 0; i < pointsInGame.Count; i++) {
            foreach (SignInARow signInARow in field[pointsInGame[i].x, pointsInGame[i].y].signsInARow)
                if (signInARow.Type == AIType)
                    aiP += signInARow.PointsWorth;
                else
                    humP += signInARow.PointsWorth;
        }

        aiPoint = aiP; humanPoint = humP;
        
        return aiP + humP;
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
    /// Disables int the local gameField the points, which are disabled in grid<para />
    /// Requires the (0,0) point in gridPos of the gameField
    /// </summary>
    private void SetLocalGridDisabled(int[] zerozero) {
        for (int i = 0; i < gameField.GetLength(0); i++) {
            for (int k = 0; k < gameField.GetLength(1); k++) {
                if (i == gameField.GetLength(0) / 2 && k == gameField.GetLength(1) / 2) continue;

                CellHolder ch = grid.GetCellHolderAtGridPos(new int[] { zerozero[0] + i, zerozero[1] + k });
                if (!(ch == null || ch.CurrentTemplate.cellOcc == Cell.CellOcc.NONE)) {
                    gameField[i, k].type = Cell.CellOcc.BLOCKED; // Because it checks for NONEs in algorithm
                }
            }
        }
    }

    private int[] LocalAIToGridPos(IntVector2 pos) {
        return LocalAIToGridPos(pos.x, pos.y);
    }

    private int[] LocalAIToGridPos(int x, int y) {
        return new int[] {
            x + firstPointInGame.x - gameField.GetLength(0) / 2,
            y + firstPointInGame.y - gameField.GetLength(1) / 2
        };
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
                //if (examineX >= 0 && examineX < field.GetLength(0) && examineY >= 0 && examineY < field.GetLength(1)) {
                // It is the same sign
                if (field[examineX, examineY].type == currentType) {
                    count++;
                    endOne = new IntVector2(examineX, examineY);
                } else {
                    break;
                }
            }

            // Go through the opposite of checkdirection direction
            for (int j = 1; j < Grid.WIN_CONDITION; j++) {
                int examineX = where.x + -checkDirections[i, 0] * j;
                int examineY = where.y + -checkDirections[i, 1] * j;

                // We are in bounds
                //if (examineX >= 0 && examineX < field.GetLength(0) && examineY >= 0 && examineY < field.GetLength(1)) {
                // It is the same sign
                if (field[examineX, examineY].type == currentType) {
                    count++;
                    endTwo = new IntVector2(examineX, examineY);
                } else {
                    break;
                }
            }

            if (count < 2) continue;
            // Now we have the endpoints of this checkdirection in endpoint one and endpoint two
            // We also have if there are blocks at the end if there is not then the block variables are null
            SignInARow signsInARow = new SignInARow(endOne, endTwo, currentType);
            IntVector2 end1 = signsInARow.From - signsInARow.Steepness, end2 = signsInARow.To + signsInARow.Steepness;
            signsInARow.SetEndEvaluationFields(field[end1.x, end1.y], field[end2.x, end2.y]);

            SignInARow removedSignInARow = null;
            field[where.x, where.y].AddSignInARow(signsInARow, out removedSignInARow);

            _placed.Add(new PlaceData(new IntVector2(where), signsInARow));
            if (removedSignInARow != null) _removed.Add(new PlaceData(new IntVector2(where), removedSignInARow));
        }

        placed = _placed; removed = _removed;
    }

    /// <summary>
    /// A recursive minimax algorithm with alpha beta pruning
    /// </summary>
    private EvaluationResult EvaluateField(EvaluationField[,] field, Cell.CellOcc whoseTurn, int deepCount, List<IntVector2> pointsInGame, float alpha, float beta) {
        EvaluationResult result = new EvaluationResult(whoseTurn == HumanType ? int.MaxValue : int.MinValue, new IntVector2());

        List<IntVector2> been = new List<IntVector2>();
        int pointsInGameLength = pointsInGame.Count;

        bool alphaBetaEnd = false;

        // Go through the places where we can place
        // Call NewSignPlaced with field and position where we want to place
        for (int j = pointsInGame.Count - 1; j >= 0; j--) {
            // In each direction
            for (int i = -1; i <= 1 && !alphaBetaEnd; i++) {
                for (int k = -1; k <= 1 && !alphaBetaEnd; k++) {
                    // we just skip a place if we feel like, just to make AI a bit easier
                    if (rand.NextDouble() <= leaveOutChance + pointsInGame.Count / (float) afterSignCountMiss) continue;

                    IntVector2 pos = new IntVector2(pointsInGame[j].x + i, pointsInGame[j].y + k);
                    // Not 0 0 and in bounds
                    if (!(i == 0 && k == 0) /*&& pos.x >= 0 && pos.x < field.GetLength(0) && pos.y >= 0 && pos.y < field.GetLength(1)*/
                        && field[pos.x, pos.y].type == Cell.CellOcc.NONE && !been.Contains(pos)) {  // if we haven't checked this position and the type of cell we are examining is NONE, so empty
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
                        EvaluationResult evalResult;
                        if (deepCount == DEEP_MAX) {
                            float aiPoint, humanPoint;
                            GetPointsFromSignsInARow(field, pointsInGame, pos, out aiPoint, out humanPoint);

                            if (whoseTurn == AIType)
                                evalResult.points = aiPoint + humanPoint * 1.5f;
                            else
                                evalResult.points = aiPoint * 1.5f + humanPoint;
                            evalResult.fieldPos = new IntVector2(pos);
                        } else {
                            evalResult = EvaluateField(field, SignResourceStorage.GetOppositeOfSign(whoseTurn), deepCount + 1, pointsInGame, alpha, beta);
                        }

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
                        for (int l = 0; l < placed.Count; l++)
                            field[placed[l].fieldPos.x, placed[l].fieldPos.y].signsInARow.Remove(placed[l].signInARow);
                        for (int l = 0; l < removed.Count; l++)
                            field[removed[l].fieldPos.x, removed[l].fieldPos.y].signsInARow.Add(removed[l].signInARow);

                        field[pos.x, pos.y].type = Cell.CellOcc.NONE;

                        pointsInGame.RemoveAt(pointsInGame.Count - 1);
                    }
                }
            }
        }

        return result;
    }

    private IntVector2 WhichIsBetter(IntVector2 first, IntVector2 second, Cell.CellOcc placeType) {
        float[] points = new float[2];

        for (int j = 0; j < 2; j++) {
            IntVector2 pos = j == 0 ? first : second;

            // Data so we can revert the field back (because recursive algorithm)
            List<PlaceData> placed;
            List<PlaceData> removed;

            // Set the examined cell to the current's sign
            gameField[pos.x, pos.y].type = placeType;
            NewSignPlaced(gameField, pos, out placed, out removed);
            pointsInGame.Add(new IntVector2(pos));

            float aiPoints, humPoints;
            points[j] = GetPointsFromSignsInARow(gameField, pointsInGame, pos, out aiPoints, out humPoints);

            // Revert the field back 
            for (int l = 0; l < placed.Count; l++)
                gameField[placed[l].fieldPos.x, placed[l].fieldPos.y].signsInARow.Remove(placed[l].signInARow);
            for (int l = 0; l < removed.Count; l++)
                gameField[removed[l].fieldPos.x, removed[l].fieldPos.y].signsInARow.Add(removed[l].signInARow);

            gameField[pos.x, pos.y].type = Cell.CellOcc.NONE;
            pointsInGame.RemoveAt(pointsInGame.Count - 1);
        }

        if (placeType == AIType) // We are maximizing
            return points[0] > points[1] ? first : second;
        else // We are minimizing
            return points[0] > points[1] ? second : first;
    }

    public int[] StartEvaluation() {
        // ________________________________________ FIRST POINT __________________________________________________
        // There is only one placed in the game so just randomize it because otherwise it only places where we examine first
        if (pointsInGame.Count < 2) {
            System.Random rand = new System.Random();
            IntVector2 random;
            do {
                random = new IntVector2(rand.Next(0, 3) - 1, rand.Next(0, 3) - 1);
            } while (random.x == 0 && random.y == 0);

            return LocalAIToGridPos(pointsInGame[0] + random);
        }

        // _____________________________________ DEFENSE EVAL __________________________________________
        // It' only a defending mechanism so only checks for human pointsinrow
        // try places where we surely have to place
        // it has a smaller chance that it skips that position
        SignInARow humThree = null, humFour = null;
        SignInARow aiThree = null, aiFour = null;

        int tillI = pointsInGame.Count;
        float exponentialChance = Mathf.Pow(leaveOutChance, 1.55f);
        for (int i = 0; i < tillI; i++) {
            List<SignInARow> list = gameField[pointsInGame[i].x, pointsInGame[i].y].signsInARow;
            for (int k = 0; k < list.Count; k++) {
                // skip at random
                if (rand.NextDouble() < exponentialChance) continue;
                

                int length = list[k].Length;
                int blockCount = list[k].BlockCount();
                
                if (length == 3 && blockCount == 0) {
                    // Store it so if after this loop we don't find a four (we don't return) place there
                    if (list[k].Type == HumanType)
                        humThree = list[k];
                    else
                        aiThree = list[k];
                } else if (length == 4 && blockCount != 2) {

                    // It prioritizes the fours so if it finds one place there
                    if (list[k].Type == HumanType) {
                        humFour = list[k];
                    } else { // There is a 4 length that is AIType so place there
                        aiFour = list[k];
                    }
                }
            }
        }

        // First if there is a four type ai where we can place win the game
        if (aiFour != null) return LocalAIToGridPos(aiFour.GetUnblockedPos());
        // If there is a four that is type of human we should really place there
        if (humFour != null) return LocalAIToGridPos(humFour.GetUnblockedPos());

        // If there is a signinarow with aitype that is length of three decide which is better and return that
        if (aiThree != null) {
            return LocalAIToGridPos(WhichIsBetter(aiThree.GetBlockField1Pos(), aiThree.GetBlockField2Pos(), AIType));
        }

        // If there is no three with AI type but there is one for the human decide which is better
        if (humThree != null) { // We are going to decide which position is better 
            return LocalAIToGridPos(WhichIsBetter(humThree.GetBlockField1Pos(), humThree.GetBlockField2Pos(), AIType));
        }

        // Come here is everything else fails
        // ___________________ NORMAL EVAL _____________________________-
        EvaluationResult result = new EvaluationResult();
        try {
            result = EvaluateField(gameField, AIType, 1, pointsInGame, int.MinValue, int.MaxValue);
        } catch (Exception e) {
            UnityEngine.Debug.Log(e.Message + "\n" + e.StackTrace);
        }
        return LocalAIToGridPos(result.fieldPos);
    }

    /// <summary>
    /// Places down a random sign in world while taking the already placed signs into consideration
    /// </summary>
    /// <returns></returns>
    public int[] PlaceDownRandom() {
        IntVector2 pos; CellHolder ch;
        do {
            // It allocates a rectangle for the game, so it doesn't place it in another game (thats innerR) but just for good measures we put it in a dowhile
            float r = 20f;

            Vector2 vect = UnityEngine.Random.insideUnitCircle * r;
            pos = topRightAll + new IntVector2((int) vect.x, (int) vect.y);

            ch = grid.GetCellHolderAtGridPos(LocalAIToGridPos(pos.x, pos.y));
        } while (!(ch == null || ch.CurrentTemplate.cellOcc == Cell.CellOcc.NONE));

        return LocalAIToGridPos(pos);
    }

    /// <summary>
    /// It is called from AITTTGL StartNewGame
    /// </summary>
    public void Reset() {
        bottomLeftPosOfField = new IntVector2(int.MaxValue, int.MaxValue);
        topRightPosOfField = new IntVector2(int.MinValue, int.MinValue);
        gameField = new EvaluationField[100, 100];
        pointsInGame = new List<IntVector2>();
        for (int i = 0; i < gameField.GetLength(0); i++)
            for (int k = 0; k < gameField.GetLength(1); k++)
                gameField[i, k] = new EvaluationField(new IntVector2(i, k));
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
    public IntVector2 posInGameField;

    /// <summary>
    /// Stores signs next to each other that star from this sign
    /// </summary>
    public List<SignInARow> signsInARow;

    public EvaluationField(IntVector2 posInGameField) {
        this.posInGameField = posInGameField;
        signsInARow = new List<SignInARow>();
        type = Cell.CellOcc.NONE;
    }

    public EvaluationField(Cell.CellOcc type, IntVector2 posInGameField) : this(posInGameField) {
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

    public override string ToString() {
        return string.Format("X: {0} and Y: {1}", x, y);
    }
}

internal class SignInARow : IEquatable<SignInARow> {
    private IntVector2 from;
    public IntVector2 From { get { return from; } }
    private IntVector2 to;
    public IntVector2 To { get { return to; } }
    private IntVector2 steepness;
    public IntVector2 Steepness { get { return steepness; } }

    private EvaluationField blockField1;
    public IntVector2 GetBlockField1Pos() { return blockField1.posInGameField; }
    private EvaluationField blockField2;
    public IntVector2 GetBlockField2Pos() { return blockField2.posInGameField; }
    /// <summary>
    /// Returns the first unblocked pos it finds. Otherwise just a (-1, -1) vector.
    /// </summary>
    /// <returns></returns>
    public IntVector2 GetUnblockedPos() {
        if (blockField1.type == Cell.CellOcc.NONE) return blockField1.posInGameField;
        else if (blockField2.type == Cell.CellOcc.NONE) return blockField2.posInGameField;

        return new IntVector2(-1, -1);
    }

    private float points;
    public float PointsWorth { get { return points; } }
    public float PointsWorthRecounted {
        get {
            int length = Length;
            points = (type == AIScript.AIType ? pointTable[length > 5 ? 5 : length, BlockCount()] : pointTableHuman[length > 5 ? 5 : length, BlockCount()]);

            return points;
        }
    }
    public int Length { get { return Mathf.Max(Mathf.Abs(to.x - from.x), Mathf.Abs(to.y - from.y)) + 1; } }

    private Cell.CellOcc type = Cell.CellOcc.NONE;
    public Cell.CellOcc Type { get { return type; } }

    private float[,] pointTable = new float[,] {
        { 0, 0, 0 },
        { 0, 0, 0 },
        { 15, 6, 0.5f },
        { 10000, 50, 10 },
        { 50000, 15000, 200 },
        { 999999, 999999, 999999 }
    };

    private float[,] pointTableHuman = new float[,] {
        { 0, 0, 0 },
        { 0, 0, 0 },
        { -15, -6, -0.5f },
        { -25000, -50, -10 },
        { -100000, -50000, -200 },
        { -999999, -999999, -999999 }
    };

    public SignInARow(IntVector2 one, IntVector2 two, Cell.CellOcc type) {
        this.type = type;
        SetPoints(one, two, type);
    }

    public void SetEndEvaluationFields(EvaluationField one, EvaluationField two) {
        this.blockField1 = one;
        this.blockField2 = two;
    }

    public int BlockCount() {
        Cell.CellOcc oppType = SignResourceStorage.GetOppositeOfSign(type);
        bool block1 = blockField1.type == oppType || blockField1.type == Cell.CellOcc.BLOCKED;
        bool block2 = blockField2.type == oppType || blockField2.type == Cell.CellOcc.BLOCKED;

        if (block1 && block2) return 2;
        else if (block1 || block2) return 1;
        else return 0;
    }

    /// <summary>
    /// DYDD<para />
    /// DYXD<para />
    /// DYYD<para />
    /// All ys are smaller than x<para />
    /// Always sets the smaller of the to vectors to be in the from vector<para />
    /// Also sets the steepness as well<para />
    /// Also updates points and length<para />
    /// </summary>
    public void SetPoints(IntVector2 from, IntVector2 to, Cell.CellOcc type) {
        if (from.y == to.y) {
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
    }

    public void UpdatePoints(Cell.CellOcc type) {
        int length = Length;
        points = (type == AIScript.AIType ? pointTable[Mathf.Min(5, length), BlockCount()] : pointTableHuman[Mathf.Min(5, length), BlockCount()]);
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