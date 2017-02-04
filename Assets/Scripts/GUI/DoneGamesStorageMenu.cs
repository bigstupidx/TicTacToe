using UnityEngine;
using System.Collections.Generic;

public class DoneGamesStorageMenu : MonoBehaviour {

    private List<MenuDoneGame> stored;
    
	void Awake () {
        stored = new List<MenuDoneGame>();
	}
	
    /// <summary>
    /// Adds a new donegame to storage
    /// </summary>
    /// <param name="pos">Position in world coords</param>
    /// <param name="id">Position in menucameramovoement array</param>
    public void AddNewDoneGame(Vector2 pos, int id) {
        stored.Add(new MenuDoneGame(pos, id));
    }

    public List<MenuDoneGame> GetStoredDoneGames() {
        return stored; 
    }

    public bool IsThereAnyStored() { return stored.Count > 0; }

    public struct MenuDoneGame {
        public Vector2 position;
        public int menuStorageID;

        public MenuDoneGame(Vector2 position, int menuStorageID) {
            this.position = position;
            this.menuStorageID = menuStorageID;
        }
    }

}