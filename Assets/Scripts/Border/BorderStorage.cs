using UnityEngine;

/// <summary>
/// Used only for storing the data of a border while it is being displayed
/// </summary>
public class BorderStorage : MonoBehaviour {
    public Border.BorderStorageLogic bsl;

    public void SetData(Border.BorderStorageLogic bsl) {
        this.bsl = bsl;
    }
}
