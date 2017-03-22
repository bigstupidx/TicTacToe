using System.Collections;
using UnityEngine;
using TMPro;

public class StatePanelScript : MonoBehaviour {

    private TextMeshProUGUI text;
    private int pointCount = 1;

	void Start() {
        text = GetComponentInChildren<TextMeshProUGUI>();

        StartCoroutine(ConnectingTextCoroutine());
	}

    private IEnumerator ConnectingTextCoroutine() {
        while (true) {
            text.text = "Connecting";
            for (int i = 1; i <= pointCount; i++) text.text += ".";

            pointCount++;
            if (pointCount > 3) pointCount = 1;
            yield return new WaitForSeconds(0.5f);
        }
    }
}
