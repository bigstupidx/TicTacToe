using UnityEngine;
using System.Collections.Generic;

public class CellPooling : MonoBehaviour {

    private const string CELL_PREFAB_PATH = "Prefabs/Cell";
    private static GameObject cellPrefab;

    private static Stack<GameObject> pool = new Stack<GameObject>();

    private static GameObject parentObject;

    void Start() {
        cellPrefab = Resources.Load(CELL_PREFAB_PATH) as GameObject;

        parentObject = gameObject;
    }
    
    /// <summary>
    /// Returns a new cell from the pool
    /// </summary>
    /// <returns></returns>
	public static GameObject GetCell() {
        GameObject spawnedCell;

        // If we have an objet in the pool
        if (pool.Count > 0) {
            spawnedCell = pool.Pop();
        } else { // We don't have any more objects in the pool
            spawnedCell = Instantiate(cellPrefab);
        }

        // Make it not this objet's parent and enabled
        spawnedCell.transform.SetParent(null);
        spawnedCell.SetActive(true);

        return spawnedCell;
    }
	
    /// <summary>
    /// Stores the given object on the pool
    /// </summary>
    /// <param name="cell"></param>
	public static void StoreObject(GameObject cell) {
        if (cell != null) {
            cell.transform.SetParent(parentObject.transform);
            cell.SetActive(false);

            pool.Push(cell);
        }
    }

}
