using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoneGameScript : MonoBehaviour {

    // Height and width of the whole 
    public int height;
    public int width;

    public Color winColor;
    
	void Start () {
        transform.FindChild("Border").GetComponent<LineRenderer>().material.SetColor("_EmissionColor", winColor);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
