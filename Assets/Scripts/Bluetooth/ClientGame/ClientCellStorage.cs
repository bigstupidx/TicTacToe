using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ClientCellStorage : MonoBehaviour {

    public delegate void WinDelegate(Cell.CellOcc type);
    public static event WinDelegate SomeoneWonEvent;

    protected const int SHOW_BORDER = 40; // Border around camera in cells in which we should show cells

    // Camera size in world units
    protected int[] cameraHalfSize;

    private List<CellHolder> visibleCells;
    private List<CellHolder> notVisibleCells;

    public int[] lastRemovedSign = new int[1];
    public int[] lastSign;

    private LastPlacedMarkerScript lastPlaced;

	void Start () {
        visibleCells = new List<CellHolder>();
        notVisibleCells = new List<CellHolder>();
        lastPlaced = FindObjectOfType<LastPlacedMarkerScript>();
        
        // Set camera size
        cameraHalfSize = new int[2];
        cameraHalfSize[1] = (int) (Camera.main.orthographicSize);
        cameraHalfSize[0] = (int) ((float) Camera.main.pixelWidth / Camera.main.pixelHeight) * cameraHalfSize[1];

        InvokeRepeating("ShowOnlyVisibleCells", 0f, 0.3f);
    }

    // Called when someone wins the game
    public void SomeoneWon(Cell.CellOcc type) {
        if (SomeoneWonEvent != null) SomeoneWonEvent(type);

        // Not this device won
        if (type == BluetoothTTTGameLogic.serverType) PreferencesScript.Instance.PullExpBarThenAdd(BluetoothTTTGameLogic.EXP_FOR_LOSING);
        else PreferencesScript.Instance.PullExpBarThenAdd(BluetoothTTTGameLogic.EXP_FOR_WINNING);
    }

    private void ShowOnlyVisibleCells() {
        int[] leftBottomPos, topRightPos;
        GetCameraCoords(out leftBottomPos, out topRightPos);

        // Let's check the visible cells first whether we need to hide them or not
        for (int i = visibleCells.Count - 1; i >= 0; i--) {
            // If we need to hide it then move it to the not shown list and remove gameobject
            if (visibleCells[i].WorldPos[0] <= leftBottomPos[0] || visibleCells[i].WorldPos[0] >= topRightPos[0] ||
                visibleCells[i].WorldPos[1] <= leftBottomPos[1] || visibleCells[i].WorldPos[1] >= topRightPos[1]) {

                visibleCells[i].RemoveCurrentCell();
                notVisibleCells.Add(visibleCells[i]);
                visibleCells.RemoveAt(i);
            }
        }

        // Let's check the not visible cells second whether we need to show them or not
        for (int i = notVisibleCells.Count - 1; i >= 0; i--) {
            // If we need to show it then move it to the shown list and add gameobject
            if (notVisibleCells[i].WorldPos[0] >= leftBottomPos[0] && notVisibleCells[i].WorldPos[0] <= topRightPos[0] &&
                notVisibleCells[i].WorldPos[1] >= leftBottomPos[1] && notVisibleCells[i].WorldPos[1] <= topRightPos[1]) {

                notVisibleCells[i].ReInitCell();
                visibleCells.Add(notVisibleCells[i]);
                notVisibleCells.RemoveAt(i);
            }
        }
    }
	
    /// <summary>
    /// Add type cell at gridPos in world units locally
    /// </summary>
    public void PlaceCellAt(int[] gridPos, Cell.CellOcc type) {
        CellHolder ch = new CellHolder(gridPos);
        ch.NewCell(type);
        visibleCells.Add(ch);
        lastSign = new int[2] { gridPos[0], gridPos[1] };
        lastPlaced.MoveMarkerTo(new Vector2(gridPos[0], gridPos[1]), SignResourceStorage.Instance.GetColorRelatedTo(type == Cell.CellOcc.X ? Cell.CellOcc.O : Cell.CellOcc.X));
    }

    /// <summary>
    /// Moves camera to latest sign
    /// </summary>
    public void GoToPreviousSign() {
        if (lastSign != null)
            Camera.main.transform.DOMove(new Vector3(lastSign[0], lastSign[1], Camera.main.transform.position.z), 1f, false);
    }

    /// <summary>
    /// Returns camera coordinates
    /// </summary>
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

}
