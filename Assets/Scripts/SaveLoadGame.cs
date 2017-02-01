using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveLoadGame : MonoBehaviour {

    private Grid grid;

	void Start () {
        grid = FindObjectOfType<Grid>();
        
        if (!System.IO.File.Exists(grid.FILE_PATH))
            System.IO.File.Create(grid.FILE_PATH).Dispose();
        if (!System.IO.File.Exists(Border.FILE_PATH))
            System.IO.File.Create(Border.FILE_PATH).Dispose();

        ReadEverything();
	}

    void OnApplicationPause(bool pause) {
        if (pause) {
            WriteEverything();
            if (false) {
                System.IO.File.WriteAllText(grid.FILE_PATH, "0");
                System.IO.File.WriteAllText(Border.FILE_PATH, "0");
            }
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
