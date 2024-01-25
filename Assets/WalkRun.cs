using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkRun : MonoBehaviour
{


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void MovePower(Vector2 touchCoords, bool isPressed)
    {
        float moveBoost = gameObject.GetComponent<MovementPower>().moveBoost;
        float moveSpeed = gameObject.GetComponent<MovementPower>().moveSpeed;
    }
}
