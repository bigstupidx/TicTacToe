using UnityEngine;
using DG.Tweening;

public class BluetoothTTTGameLogic : TTTGameLogic {

    private int xScore = 0;
    public int XScore { get { return xScore; } }
    private int oScore = 0;
    public int OScore { get { return oScore; } }

    private ScoringScript scoring;

    /// <summary>
    /// Which type the server uses
    /// </summary>
    public Cell.CellOcc serverType;

    /// <summary>
    /// Where last sign was placed
    /// </summary>
    public int[] LastSignPlaced {
        get { return ((BluetoothGrid) grid).LastSignPos; }
    }
    public Cell.CellOcc LastType {
        get { return ((BluetoothGrid) grid).LastSignType; }
    }

    private BluetoothEventListener bluetoothEventListener;

    public override void Start() {
        grid = FindObjectOfType<BluetoothGrid>();
        bluetoothEventListener = FindObjectOfType<BluetoothEventListener>();

        scoring = FindObjectOfType<ScoringScript>();
        serverType = Cell.CellOcc.X;
        whoseTurn = Cell.CellOcc.X;
    }

    public override void NextTurn(int[] gridPos, out Cell.CellOcc won) {
        Cell.CellOcc didWin;
        base.NextTurn(gridPos, out didWin);

        if (didWin == Cell.CellOcc.X) {
            xScore++;
            scoring.SetScore(xScore, oScore);
        } else if (didWin == Cell.CellOcc.O) {
            oScore++;
            scoring.SetScore(xScore, oScore);
        }

        won = didWin;
    }

    public override void AddBorderToGame(int[,] points, float[,] winLinePoints, Cell.CellOcc winType) {
        base.AddBorderToGame(points, winLinePoints, winType);

        bluetoothEventListener.SetLastBorder(new Border.BorderStorageLogic(points, winLinePoints, winType));
    }

    public override void StartNewGame(int[] gridPos) {
        // New game is being started
        if (!GameSarted) {
            // If server is starting new turn
            if (WhoseTurn == serverType) {
                // Send jump command to client
                Bluetooth.Instance().Send(BluetoothMessageStrings.JUMP_TO + "#" + gridPos[0] + "#" + gridPos[1]);
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
