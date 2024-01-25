using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConeBuilder : MonoBehaviour {
    public GameObject conePrefab;
    public Vector2 touchpadCoords;
    private float touchpadYaxis;

    private int numConesPerRing=8;
    private int numRings = 1;
    
    private int conecounter=0;
    private GameObject[] coneArray = new GameObject[100];
    
	void Update () {

        SpawnConeArray();
                
        Destroy(gameObject,0.02f);
    }

    private void SpawnConeArray()
    {
        touchpadYaxis = touchpadCoords.y;
        if (coneArray != null)
        {
            foreach (GameObject cone in coneArray)
            {
                Destroy(cone);
            }
        }

        conecounter = 0;
        coneArray[conecounter] = Instantiate(conePrefab, gameObject.transform.position, gameObject.transform.rotation,gameObject.transform);
        coneArray[conecounter].transform.name = "Cone" + conecounter;
        conecounter++;
        for (int j = 0; j < numRings; j++)
        {

            for (int i = 0; i < numConesPerRing; i++)
            {
                coneArray[conecounter] = Instantiate(conePrefab, gameObject.transform.position, gameObject.transform.rotation, gameObject.transform);
                coneArray[conecounter].transform.RotateAround(gameObject.transform.position, transform.right, touchpadYaxis * 70f / numRings * (j + 1));
                coneArray[conecounter].transform.RotateAround(gameObject.transform.position, transform.forward, i * (360f / numConesPerRing));
                coneArray[conecounter].transform.name = "Cone" + conecounter;
                conecounter++;

            }
        }
    }
}
