using UnityEngine;

public class SaveLoadGame : MonoBehaviour {

    private Grid grid;

	void Start () {
        grid = FindObjectOfType<Grid>();
        
        if (!System.IO.File.Exists(grid.FILE_PATH))
            System.IO.File.Create(grid.FILE_PATH).Dispose();
        if (!System.IO.File.Exists(Border.FILE_PATH))
            System.IO.File.Create(Border.FILE_PATH).Dispose();

        StartCoroutine("ReadEverything");
        // ReadEverything();
	}

    void OnApplicationPause(bool pause) {
        if (pause) {
            WriteEverything();
        }
    }

    /// <summary>
    /// Reads everything from files which are needed (Signs, borders)
    /// </summary>
    public void ReadEverything() {
        grid.LoadFromFile();
        Border.ReadBordersFromFile();
    }

    /// <summary>
    /// Writes everything to files (Signs, borders)
    /// </summary>
    public void WriteEverything() {
        grid.WriteToFile();
        Border.WriteBordersToFile();
    }
}
