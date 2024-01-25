using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Power : MonoBehaviour
{

    private Valve.VR.EVRButtonId touchPad = Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad;
    private SteamVR_TrackedObject trackedObjectController;
    private SteamVR_Controller.Device deviceController;


    public enum PowerType { Projectile, Beam, Cone, Melee, ConeBeam }

    public string powerName;
    public PowerType powerType;
    public GameObject powerModel;
    public Vector3 projectileScale;
    public Vector3 projectileOffset; //Currently only for melee
    public float projectileSpeed;
    public float projectileFireRate;
    public float projectileMaxDist;
    public bool projectileUsesGravity;

    private GameObject thisMelee;

    public bool isPowerOn = false;

    private GameObject projectileBin;

    private float nextFire;

    public void UsePower(GameObject controller)
    {
        trackedObjectController = controller.GetComponent<SteamVR_TrackedObject>();
        deviceController = SteamVR_Controller.Input((int)trackedObjectController.index);

        Transform barrel = controller.transform;
        if (!projectileBin) { projectileBin = GameObject.FindWithTag("Projectiles"); }
        if (powerType == PowerType.Projectile)
        {
            //Debug.Log("Firing Projectile");
            if (nextFire > projectileFireRate + Time.time) { nextFire = Time.time; }
            if (Time.time >= nextFire)
            {
                // Debug.Log("Now: " + Time.time + " Next Fire: " + nextFire);
                GameObject thisProjectile = Instantiate(powerModel, barrel.position, Quaternion.identity);
                Rigidbody rb = thisProjectile.GetComponent<Rigidbody>();
                rb.velocity = barrel.forward * projectileSpeed;
                //Debug.Log("XYZ" + barrel.position);
                thisProjectile.transform.rotation = barrel.transform.rotation;
                thisProjectile.transform.localScale = new Vector3(projectileScale.x, projectileScale.y, projectileScale.z);
                rb.useGravity = projectileUsesGravity;
                thisProjectile.transform.parent = projectileBin.transform;
                nextFire = Time.time + projectileFireRate;
            }
        }

        //Beam Attack    Needs to be fixed for push effects, maybe make the beam grow and shrink, or add an invisible projectile that makes a push on an interval.
        else if (powerType == PowerType.Beam)
        {
            //Debug.Log("Firing Beam");
            GameObject thisBeam;
            int layer_mask = LayerMask.GetMask("Object", "Enemy");
            if (nextFire > projectileFireRate + Time.time) { nextFire = Time.time; }
            if (Time.time >= nextFire)
            {
                RaycastHit hit;
                if (Physics.Raycast(barrel.transform.position, barrel.transform.forward, out hit, projectileMaxDist, layer_mask))
                {
                    //Debug.Log(hit.transform.name + " hit with Laser Beams");
                    float beamLength = ((hit.point - barrel.position).magnitude);
                    thisBeam = Instantiate(powerModel, barrel.position, Quaternion.identity);
                    thisBeam.transform.localScale = new Vector3(projectileScale.x, projectileScale.y, beamLength);
                    thisBeam.transform.rotation = barrel.transform.rotation;
                    thisBeam.transform.parent = projectileBin.transform;
                    thisBeam.name = "BEAM";
                }
                else
                {
                    //Debug.Log("Missing Laser Beams");
                    float beamLength = projectileMaxDist;
                    thisBeam = Instantiate(powerModel, barrel.position, Quaternion.identity);
                    thisBeam.transform.localScale = new Vector3(projectileScale.x, projectileScale.y, beamLength);
                    thisBeam.transform.rotation = barrel.transform.rotation;
                    thisBeam.transform.parent = projectileBin.transform;
                    thisBeam.name = "BEAM";
                }
            }
        }

        else if (powerType == PowerType.Cone)
        {
            GameObject thisCone;
            if (nextFire > projectileFireRate + Time.time) { nextFire = Time.time; }
            if (Time.time >= nextFire)
            {
                //Debug.Log("Firing a Cone");
                thisCone = Instantiate(powerModel, barrel.position, Quaternion.identity);
                thisCone.transform.localScale = new Vector3(projectileScale.x * projectileMaxDist, projectileScale.y * projectileMaxDist, projectileScale.z * projectileMaxDist);
                thisCone.transform.rotation = barrel.transform.rotation;
                thisCone.transform.parent = projectileBin.transform;
                thisCone.name = "CONE";
            }
        }

        else if (powerType == PowerType.Melee)
        {
            //Debug.Log("Attempting to Arm Melee. Is melee Already On?" + isPowerOn);
            if (nextFire > projectileFireRate + Time.time) { nextFire = Time.time; }
            if (Time.time >= nextFire)
            {
                if (!isPowerOn)
                {
                    //Debug.Log("Arming Melee");
                    isPowerOn = true;
                    thisMelee = Instantiate(powerModel, barrel.position, Quaternion.identity); // +projectileOffset
                    thisMelee.transform.localScale = new Vector3(projectileScale.x, projectileScale.y, projectileScale.z);
                    thisMelee.transform.rotation = controller.transform.rotation;
                    thisMelee.transform.parent = controller.transform;
                    thisMelee.transform.localPosition = projectileOffset;
                    thisMelee.name = "MELEE";
                    nextFire = Time.time + projectileFireRate;
                }
            }
        }

        else if (powerType == PowerType.ConeBeam)
        {

            if (nextFire > projectileFireRate + Time.time) { nextFire = Time.time; }
            if (Time.time >= nextFire)
            {
                float touchY = deviceController.GetAxis(touchPad).y;
                float touchX = deviceController.GetAxis(touchPad).x;
                float zScale = (touchX + 1);

                Debug.Log("Firing a ConeBeam");
                if (touchY > 0 && touchY < .7f)
                {
                    SpawnConeArray(controller);
                    nextFire = Time.time + projectileFireRate;
                }
                else if (touchY >= .7f)
                {
                    SpawnConeSphere(controller); 
                    nextFire = Time.time + projectileFireRate;
                }
                else if (touchY <= 0 && touchY > -0.7f)
                {
                    GameObject thisCone = Instantiate(powerModel, barrel.position, Quaternion.identity);
                    thisCone.transform.localScale = new Vector3(projectileScale.x * projectileMaxDist * (1.05f + touchY), projectileScale.y * projectileMaxDist * (1.05f + touchY), projectileScale.z * projectileMaxDist*zScale);
                    thisCone.transform.rotation = barrel.transform.rotation;
                    thisCone.transform.parent = projectileBin.transform;
                    thisCone.name = "CONE";
                    nextFire = Time.time + projectileFireRate;
                }
                else if (touchY <= -.7f)
                {
                    //Add Beam Effects?
                    GameObject thisCone = Instantiate(powerModel, barrel.position, Quaternion.identity);
                    thisCone.transform.localScale = new Vector3(projectileScale.x * projectileMaxDist * (1.05f + touchY), projectileScale.y * projectileMaxDist * (1.05f + touchY), projectileScale.z * projectileMaxDist*zScale);
                    thisCone.transform.rotation = barrel.transform.rotation;
                    thisCone.transform.parent = projectileBin.transform;
                    thisCone.name = "CONE";
                    nextFire = Time.time + projectileFireRate;
                }

            }

        }

    }

    public void StopPower(GameObject controller)
    {
        //Debug.Log("Stopping Power");
        if (thisMelee)
        {
            Destroy(thisMelee);
            isPowerOn = false;
        }
    }
    private void SpawnConeArray(GameObject controller)
    {
        int numConesPerRing = 8;
        int numRings = 1;
        int conecounter = 0;

        float touchY = deviceController.GetAxis(touchPad).y;
        float touchX = deviceController.GetAxis(touchPad).x;
        float zScale = (touchX + 1);

        GameObject[] coneArray = new GameObject[100];

        coneArray[conecounter] = Instantiate(powerModel, controller.transform.position, controller.transform.rotation, controller.transform);
        coneArray[conecounter].transform.localScale = new Vector3(projectileScale.x * projectileMaxDist, projectileScale.y * projectileMaxDist, projectileScale.z * projectileMaxDist)*zScale;
        coneArray[conecounter].transform.name = "Cone" + conecounter;
        coneArray[conecounter].transform.parent = projectileBin.transform;
        conecounter++;
        for (int j = 0; j < numRings; j++)
        {

            for (int i = 0; i < numConesPerRing; i++)
            {
                coneArray[conecounter] = Instantiate(powerModel, controller.transform.position, controller.transform.rotation, controller.transform);
                coneArray[conecounter].transform.RotateAround(controller.transform.position, controller.transform.right, touchY * 70f / numRings * (j + 1));
                coneArray[conecounter].transform.RotateAround(controller.transform.position, controller.transform.forward, i * (360f / numConesPerRing));
                coneArray[conecounter].transform.localScale = new Vector3(projectileScale.x * projectileMaxDist, projectileScale.y * projectileMaxDist, projectileScale.z * projectileMaxDist) * zScale;
                coneArray[conecounter].transform.parent = projectileBin.transform;
                coneArray[conecounter].transform.name = "Cone" + conecounter;
                conecounter++;

            }
        }
    }

    private void SpawnConeSphere(GameObject controller)
    {
        int numConesPerRing = 9;
        int numRings = 4;
        int conecounter = 0;
        float touchX = deviceController.GetAxis(touchPad).x;
        float zScale = (touchX + 1);

        GameObject[] coneArray = new GameObject[100];

        coneArray[conecounter] = Instantiate(powerModel, controller.transform.position, controller.transform.rotation, controller.transform);
        coneArray[conecounter].transform.localScale = new Vector3(projectileScale.x * projectileMaxDist, projectileScale.y * projectileMaxDist, projectileScale.z * projectileMaxDist) * zScale;
        coneArray[conecounter].transform.name = "Cone" + conecounter;
        coneArray[conecounter].transform.parent = projectileBin.transform;
        conecounter++;
        for (int j = 0; j < numRings; j++)
        {

            for (int i = 0; i < numConesPerRing; i++)
            {
                coneArray[conecounter] = Instantiate(powerModel, controller.transform.position, controller.transform.rotation, controller.transform);
                coneArray[conecounter].transform.RotateAround(controller.transform.position, controller.transform.right, 144f / numRings * (j + 1));
                coneArray[conecounter].transform.RotateAround(controller.transform.position, controller.transform.forward, i * (360f / numConesPerRing));
                coneArray[conecounter].transform.localScale = new Vector3(projectileScale.x * projectileMaxDist, projectileScale.y * projectileMaxDist, projectileScale.z * projectileMaxDist) * zScale;
                coneArray[conecounter].transform.parent = projectileBin.transform;
                coneArray[conecounter].transform.name = "Cone" + conecounter;
                conecounter++;

            }
        }
        coneArray[conecounter] = Instantiate(powerModel, controller.transform.position, controller.transform.rotation, controller.transform);
        coneArray[conecounter].transform.localScale = new Vector3(projectileScale.x * projectileMaxDist, projectileScale.y * projectileMaxDist, projectileScale.z * projectileMaxDist) * zScale;
        coneArray[conecounter].transform.RotateAround(controller.transform.position, controller.transform.up, 180);
        coneArray[conecounter].transform.name = "Cone" + conecounter;
        coneArray[conecounter].transform.parent = projectileBin.transform;
    }
}