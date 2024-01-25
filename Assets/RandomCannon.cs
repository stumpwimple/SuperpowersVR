using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomCannon : MonoBehaviour {


    [SerializeField] private GameObject cannonBase;
    [SerializeField] private GameObject trunion;
    [SerializeField] private GameObject barrelEnd;

    public GameObject[] projectiles;
    public float cooldownTime = 3f;
    public float velocityMin = 10f;
    public float velocityMax = 20f;
    public float baseRotationMax = 15f;
    public float trunionRotationMax = 65f;
    public float trunionRotationMin = 25f;
    
    private float nextCooldownTime;
    private Vector3 randBaserotation;
    private Vector3 randTrunionRotation;
    private bool goUp=true;
    private bool turnCW=true;

	// Use this for initialization
	void Start ()
    {


    }
	
	// Update is called once per frame
	void Update () {
        if (turnCW)
        {
            cannonBase.transform.Rotate(Vector3.up, baseRotationMax / 180f);
            //Debug.Log("y-Angle:" + cannonBase.transform.localEulerAngles.y);
            float localY =  cannonBase.transform.localEulerAngles.y;
            if (localY > 180) localY -= 360;
            if (localY  > baseRotationMax)
            {
                turnCW = false;
                //Debug.Log("Turning cCW Now");
            }
        }
        else
        {
            cannonBase.transform.Rotate(Vector3.up, -baseRotationMax / 180f);
            //Debug.Log("y-Angle:"+cannonBase.transform.localEulerAngles.y);
            float localY = cannonBase.transform.localEulerAngles.y;
            if (localY > 180) localY -= 360;
            if (localY < -baseRotationMax )
            {
                turnCW = true;
                //Debug.Log("Turning CW Now");
            }
        }

        if (goUp)
        {
            trunion.transform.Rotate(Vector3.right, baseRotationMax / 180f);
           // Debug.Log("x-Angle:" + trunion.transform.localEulerAngles.x);
            if (trunion.transform.localEulerAngles.x > trunionRotationMax)
            {
                goUp = false;
                //Debug.Log("Turning Up Now");
            }
        }
        else
        {
            trunion.transform.Rotate(Vector3.right, -baseRotationMax / 180f);
            //Debug.Log("x-Angle:" + trunion.transform.localEulerAngles.x);
            if (trunion.transform.localEulerAngles.x < trunionRotationMin)
            {
                goUp = true;
               // Debug.Log("Turning Down Now");
            }

        }

        if (Time.time > nextCooldownTime)
        {
           // Debug.Log("Firing Cannon @ " + Time.time + "s");
            int randProj = Random.Range(0,projectiles.Length);
            GameObject thisEnemyProjectile = Instantiate(projectiles[randProj], barrelEnd.transform);

            float randVelocity = Random.Range(velocityMin,velocityMax);
            thisEnemyProjectile.GetComponent<Rigidbody>().velocity = barrelEnd.transform.forward * randVelocity;
            thisEnemyProjectile.transform.parent = null;
            nextCooldownTime = Time.time + cooldownTime;

            randBaserotation = new Vector3 (0f,Random.Range(-baseRotationMax, baseRotationMax),0f);
            randTrunionRotation = new Vector3 ( Random.Range(25f, 65f),0f,0f);

        }
	}
}
