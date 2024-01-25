using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchPadVisuals : MonoBehaviour {

    public GameObject dot;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void UpdateTouchPadVisuals (Vector2 coords)
    {
        //Debug.Log("coords:" + coords);
        dot.transform.localPosition = new Vector3(coords.x, 0f, coords.y);
    }
}
