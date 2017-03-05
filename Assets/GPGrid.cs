public class GPGrid : Grid {
    protected override void Awake() {
        base.Awake();

        gameLogic = FindObjectOfType<GPTTTGameLogic>();
    }

    public override void LoadFromFile() { }
    public override void WriteToFile() { }
}
