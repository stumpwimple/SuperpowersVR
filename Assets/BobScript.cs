using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobScript : MonoBehaviour {
    public float bobHeight;
    public float bobSpeed;

    private bool bobUp=true;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (bobUp)
        {
            gameObject.transform.position += Vector3.up * bobSpeed*Time.deltaTime;
            if (gameObject.transform.position.y >= bobHeight) { bobUp = false; }
        }
        else
        {
            gameObject.transform.position += Vector3.down * bobSpeed*Time.deltaTime;

            if (gameObject.transform.position.y <= 0) { bobUp = true; }
        }
	}
}
