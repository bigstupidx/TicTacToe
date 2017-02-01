public class BluetoothClientBorder : Border {

    public new void Start() {
        base.Start();

        InvokeRepeating("UpdateBordersShown", 0f, 0.3f);
    }

    public new static void WriteBordersToFile() { }
    public new static void ReadBordersFromFile() { }
}
