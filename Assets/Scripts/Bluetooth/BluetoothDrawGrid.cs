
public class BluetoothDrawGrid : DrawGrid {

    public override void Start() {
        // TODO If bluetooth doesn't work this is the problem just revert it back to how it was
        // override PATH variable outside and copy start from drawgrid

        GRIDMANAGER_PREFAB_PATH = "Prefabs/Bluetooth/GridManager";
        base.Start();
    }

}
