using UnityEngine;
using DG.Tweening;
using GooglePlayGames;
using System.Text;

public class GPTTTGameLogic : TTTGameLogic {

    private int xScore = 0;
    public int XScore { get { return xScore; } }
    private int oScore = 0;
    public int OScore { get { return oScore; } }

    private ScoringScript scoring;

    /// <summary>
    /// Which type the server uses
    /// </summary>
    public Cell.CellOcc serverType;

    public override void Start() {
        grid = FindObjectOfType<GPGrid>();

        scoring = FindObjectOfType<ScoringScript>();
        serverType = Cell.CellOcc.X;
        whoseTurn = Cell.CellOcc.X;
    }

    public override void NextTurn(int[] gridPos, out Cell.CellOcc won) {
        Cell.CellOcc didWin;
        base.NextTurn(gridPos, out didWin);

        // Send to client that sign has been placed
        PlayGamesPlatform.Instance.RealTime.SendMessageToAll(true, Encoding.Unicode.GetBytes(
            GPMessageStrings.SIGN_PLACED + "#" + gridPos[0].ToString() + "#" + gridPos[1].ToString() + "#" + SignResourceStorage.GetOppositeOfSign(whoseTurn).ToString()  
        ));

        // also send whose turn it is
        PlayGamesPlatform.Instance.RealTime.SendMessageToAll(true, Encoding.Unicode.GetBytes(
            GPMessageStrings.TURN_OF + "#" + whoseTurn.ToString()
        ));

        if (didWin == Cell.CellOcc.X || didWin == Cell.CellOcc.O) {
            if (didWin == Cell.CellOcc.X) xScore++;
            else oScore++;
            
            scoring.SetScore(xScore, oScore);

            // send score to client
            PlayGamesPlatform.Instance.RealTime.SendMessageToAll(true, Encoding.Unicode.GetBytes(
                GPMessageStrings.SEND_SCORE + "#" + xScore.ToString() + "#" + oScore.ToString()
            ));
        }

        won = didWin;
    }

    public override void AddBorderToGame(int[,] points, float[,] winLinePoints, Cell.CellOcc winType) {
        base.AddBorderToGame(points, winLinePoints, winType);

        string send = string.Join("", new string[] 
        {
            GPMessageStrings.ADD_BORDER,
            "#",
            points.GetLength(0).ToString(),
            "#",
            winLinePoints[0, 0].ToString(),
            "#",
            winLinePoints[0, 1].ToString(),
            "#",
            winLinePoints[1, 0].ToString(),
            "#",
            winLinePoints[1, 1].ToString(),
            "#"
        });

        StringBuilder sb = new StringBuilder(send);

        for (int i = 0; i < points.GetLength(0); i++) {
            for (int k = 0; k < points.GetLength(1); k++) {
                sb.Append(points[i, k]);
                sb.Append("#");
            }
        }

        // No need of # because one is added at the end of the loops
        sb.Append(winType.ToString());

        // Send client the border
        PlayGamesPlatform.Instance.RealTime.SendMessageToAll(true, Encoding.Unicode.GetBytes(sb.ToString()));
    }

    public override void StartNewGame(int[] gridPos) {
        // New game is being started
        if (!GameSarted) {
            // If server is starting new turn
            if (WhoseTurn == serverType) {
                // Send jump command to client
                PlayGamesPlatform.Instance.RealTime.SendMessageToAll(true, Encoding.Unicode.GetBytes(
                        GPMessageStrings.JUMP_TO + "#" + gridPos[0].ToString() + "#" + gridPos[1].ToString()
                    ));

            } else {
                // Client placed so jump server to pos
                JumpCameraTo(gridPos);
            }
        }

        base.StartNewGame(gridPos);
    }

    public bool IsItServersTurn() {
        return WhoseTurn == serverType;
    }

    public void JumpCameraTo(int[] pos) {
        Vector3 jumpTo = new Vector3(pos[0], pos[1], Camera.main.transform.position.z);
        Camera.main.transform.DOMove(jumpTo, Vector2.Distance(Camera.main.transform.position, jumpTo) * BluetoothEventListener.JUMP_TIME_PER_ONE);
    }
}
