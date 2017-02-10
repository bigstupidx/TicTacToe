
public class BluetoothGrid : Grid {

    public int[] LastSignPos {
        get { return previousGridPos; }
    }
    public int[] lastSignRemoved = new int[1];

    private Cell.CellOcc lastSignType;
    public Cell.CellOcc LastSignType {
        get { return lastSignType; }
    }

    protected override void Awake() {
        base.Awake();
        
        gameLogic = FindObjectOfType<BluetoothTTTGameLogic>();
    }
    

    public override void SetCameraToPreviousSign() {
        base.SetCameraToPreviousSign();
    }

    public override void LoadFromFile() { }
    public override void WriteToFile() { }

    public override bool PlaceSign(int[] gridPos, Cell.CellOcc cellType, bool disabled = false) {
        bool ret = base.PlaceSign(gridPos, cellType, disabled);
        if (ret) lastSignType = cellType;

        return ret;
    }

}
