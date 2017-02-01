using UnityEngine;

public class BluetoothGrid : Grid {

    public int[] LastSignPos {
        get { return previousGridPos; }
    }

    private Cell.CellOcc lastSignType;
    public Cell.CellOcc LastSignType {
        get { return lastSignType; }
    }

    private BluetoothEventListener bluetoothScript;

    protected override void Awake() {
        base.Awake();

        bluetoothScript = FindObjectOfType<BluetoothEventListener>();
        gameLogic = FindObjectOfType<BluetoothTTTGameLogic>();
    }

    public override void LoadFromFile() { }
    public override void WriteToFile() { }

    public override bool PlaceSign(int[] gridPos, Cell.CellOcc cellType, bool disabled = false) {
        bool ret = base.PlaceSign(gridPos, cellType, disabled);
        if (ret) lastSignType = cellType;
        
        /* if (ret)
            Bluetooth.Instance().Send(BluetoothMessageStrings.PLACE_SIGN_AT + "#" + gridPos[0].ToString() + "#" + gridPos[1].ToString() + "#" + cellType.ToString()); */

        return ret;
    }

}
