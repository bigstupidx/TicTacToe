using UnityEngine;

public class LoadImage : MonoBehaviour {

    bool loading = false;
	
	void Update () {
		if (loading) {
            transform.Rotate(0, 0, 2f);
        }
	}

    public void ToggleLoading() {
        if (loading) loading = false;
        else loading = true;
    }

    public void StartLoading() {
        loading = true;
    }
    public void StopLoading() {
        loading = false;
    }
}
