using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public GameObject leftController;
    public GameObject rightController;
    public GameObject headBarrel;
    public GameObject chestBarrel;

    public GameObject headPower;
    public GameObject leftPower;
    public GameObject rightPower;
    public GameObject chestPower;
    public GameObject movePower;

    public GameObject leftTouchPadVisual;
    public GameObject rightTouchPadVisual;

    private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;
    private Valve.VR.EVRButtonId gripButton = Valve.VR.EVRButtonId.k_EButton_Grip;
    private Valve.VR.EVRButtonId touchPad = Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad;
    private Valve.VR.EVRButtonId menuButton = Valve.VR.EVRButtonId.k_EButton_ApplicationMenu;
    private Valve.VR.EVRButtonId systemButton = Valve.VR.EVRButtonId.k_EButton_System;

    private SteamVR_TrackedObject trackedObjectLeftController;
    private SteamVR_TrackedObject trackedObjectRightController;
    private SteamVR_Controller.Device deviceLeftController;
    private SteamVR_Controller.Device deviceRightController;

    private GameObject rightHeldObject;
    private GameObject leftHeldObject;

    Rigidbody rightSimulator;
    Rigidbody leftSimulator;

    void Start()
    {
        rightSimulator = new GameObject().AddComponent<Rigidbody>();
        rightSimulator.name = "RightSimulator";
        rightSimulator.transform.parent = transform.parent;
        leftSimulator = new GameObject().AddComponent<Rigidbody>();
        leftSimulator.name = "LeftSimulator";
        leftSimulator.transform.parent = transform.parent;

        trackedObjectLeftController = leftController.GetComponent<SteamVR_TrackedObject>();
        trackedObjectRightController = rightController.GetComponent<SteamVR_TrackedObject>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        deviceLeftController = SteamVR_Controller.Input((int)trackedObjectLeftController.index);
        deviceRightController = SteamVR_Controller.Input((int)trackedObjectRightController.index);
        
        /////////////////////Touchpad Buttons - Movement Powers
        if (deviceLeftController.GetTouch(touchPad))
        {
            //Debug.Log("TouchPad touched at " + deviceLeftController.GetAxis(touchPad) + "passing GameObject: "+gameObject);
            leftTouchPadVisual.GetComponent<TouchPadVisuals>().UpdateTouchPadVisuals(deviceLeftController.GetAxis(touchPad));
            movePower.GetComponent<MovementPower>().MovePower(deviceLeftController.GetAxis(touchPad), deviceLeftController.GetPress(touchPad));
        }
        if (deviceLeftController.GetTouchUp(touchPad))
        {
            //Debug.Log("TouchPad touched at " + deviceLeftController.GetAxis(touchPad) + "passing GameObject: " + gameObject);
            //leftTouchPadVisual.GetComponent<TouchPadVisuals>().UpdateTouchPadVisuals(Vector3.zero);
            movePower.GetComponent<MovementPower>().StopMovePower();
        }
        if (deviceRightController.GetTouch(touchPad))
        {
            rightTouchPadVisual.GetComponent<TouchPadVisuals>().UpdateTouchPadVisuals(deviceRightController.GetAxis(touchPad));

        }
        if (deviceRightController.GetTouchUp(touchPad))
        {
            //rightTouchPadVisual.GetComponent<TouchPadVisuals>().UpdateTouchPadVisuals(Vector3.zero);

        }

        /////////////////////Trigger Touch Button - Arm Powers
        if (deviceLeftController.GetTouch(triggerButton))
        {
            leftPower.GetComponent<Power>().UsePower(leftController);
        }
        if (deviceLeftController.GetTouchUp(triggerButton))
        {
            leftPower.GetComponent<Power>().StopPower(leftController);
        }

        if (deviceRightController.GetTouch(triggerButton))
        {
            rightPower.GetComponent<Power>().UsePower(rightController);
        }
        if (deviceRightController.GetTouchUp(triggerButton))
        {
            rightPower.GetComponent<Power>().StopPower(rightController);
        }

        ///////// Left Menu Button - Chest Power
        if (deviceLeftController.GetPress(menuButton))
        {
            chestPower.GetComponent<Power>().UsePower(chestBarrel);
        }
        if (deviceLeftController.GetPressUp(menuButton))
        {
            chestPower.GetComponent<Power>().StopPower(chestBarrel);
        }

        ///////// Right Menu Button - Head Power
        if (deviceRightController.GetPress(menuButton))
        {
            headPower.GetComponent<Power>().UsePower(headBarrel);
        }
        if (deviceRightController.GetPressUp(menuButton))
        {
            headPower.GetComponent<Power>().StopPower(headBarrel);
        }


        /////////////Grip Button - Right Hand Holding stuff loop
        if (rightHeldObject)
        {
            rightSimulator.velocity = (rightController.transform.position - rightSimulator.position) * 50f;
            if (deviceRightController.GetPressUp(gripButton))
            {
                rightHeldObject.transform.parent = null;
                rightHeldObject.GetComponent<Rigidbody>().isKinematic = false;
                rightHeldObject.GetComponent<Rigidbody>().velocity = rightSimulator.velocity;
                rightHeldObject.GetComponent<HeldObject>().parent = null;
                rightHeldObject = null;
            }
        }
        else //TODO Check player strength versus object weight
        {
            if (deviceRightController.GetPressDown(gripButton))
            {
                Collider[] cols = Physics.OverlapSphere(rightController.transform.position, 0.1f);

                foreach (Collider col in cols)
                {
                    if (rightHeldObject == null && col.GetComponent<HeldObject>() && col.GetComponent<HeldObject>().parent == null)
                    {
                        rightHeldObject = col.gameObject;
                        rightHeldObject.transform.parent = rightController.transform;
                        rightHeldObject.transform.localPosition = Vector3.zero;
                        rightHeldObject.transform.localRotation = Quaternion.identity;
                        rightHeldObject.GetComponent<Rigidbody>().isKinematic = true;
                        rightHeldObject.GetComponent<HeldObject>().parent = deviceRightController;
                    }
                }
            }
        }

        ////////////////Grip Button - Left Hand Holding stuff loop
        if (leftHeldObject)
        {
            //Debug.Log("Left Holding Stuff");
            leftSimulator.velocity = (leftController.transform.position - leftSimulator.position) * 50f;
            if (deviceLeftController.GetPressUp(gripButton))
            {
                leftHeldObject.transform.parent = null;
                leftHeldObject.GetComponent<Rigidbody>().isKinematic = false;
                leftHeldObject.GetComponent<Rigidbody>().velocity = leftSimulator.velocity;
                leftHeldObject.GetComponent<HeldObject>().parent = null;
                leftHeldObject = null;
            }
        }
        else //TODO Check player strength versus object weight
        {
            //Debug.Log("Left NOT Holding Stuff");
            if (deviceLeftController.GetPressDown(gripButton))
            {
                Collider[] cols = Physics.OverlapSphere(leftController.transform.position, 0.1f);

                foreach (Collider col in cols)
                {
                    if (leftHeldObject == null && col.GetComponent<HeldObject>() && col.GetComponent<HeldObject>().parent == null)
                    {
                        leftHeldObject = col.gameObject;
                        leftHeldObject.transform.parent = leftController.transform;
                        leftHeldObject.transform.localPosition = Vector3.zero;
                        leftHeldObject.transform.localRotation = Quaternion.identity;
                        leftHeldObject.GetComponent<Rigidbody>().isKinematic = true;
                        leftHeldObject.GetComponent<HeldObject>().parent = deviceLeftController;
                    }
                }
            }
        }

        /*//////////////////BELOW THIS LINE IS EXAMPLE BUTTON MAPPINGS//////////////////////////////////
        if (deviceLeftController.GetPressDown(triggerButton)) { Debug.Log("Left Trigger Down"); }
        if (deviceRightController.GetPressDown(triggerButton)) { Debug.Log("Right Trigger Down"); }
        if (deviceLeftController.GetPress(triggerButton)) { Debug.Log("LeftTrigger Held"); }
        if (deviceRightController.GetPress(triggerButton)) { Debug.Log("RightTrigger Held"); }

        //Left Trigger Reports anything above .1 of Trigger Axis
        if (deviceLeftController.GetAxis(triggerButton).x > .1f) { Debug.Log("LeftTriggerAxis: " + deviceLeftController.GetAxis(triggerButton)); }
        //Right Trigger Reports anything above .1 of Trigger Axis
        if (deviceRightController.GetAxis(triggerButton).x > .1f) { Debug.Log("RightTriggerAxis: " + deviceRightController.GetAxis(triggerButton)); }


        if (deviceLeftController.GetPressDown(gripButton)) { Debug.Log("Left grip Down"); }
        if (deviceRightController.GetPressDown(gripButton)) { Debug.Log("Right grip Down"); }

        if (deviceLeftController.GetPressDown(touchPad)) { Debug.Log("Left Touchpad Down @ " + deviceLeftController.GetAxis(touchPad)); }
        if (deviceRightController.GetPressDown(touchPad)) { Debug.Log("Right Touchpad Down @ " + deviceRightController.GetAxis(touchPad)); }

        if (deviceLeftController.GetPressUp(touchPad)) { Debug.Log("Left Touchpad Up @ " + deviceLeftController.GetAxis(touchPad)); }
        if (deviceRightController.GetPressUp(touchPad)) { Debug.Log("Right Touchpad Up @ " + deviceRightController.GetAxis(touchPad)); }

       // if (deviceLeftController.GetAxis(touchPad).sqrMagnitude > 0) { Debug.Log("Left Touchpad touched at coords:" + deviceLeftController.GetAxis(touchPad)); }
       // if (deviceRightController.GetAxis(touchPad).sqrMagnitude > 0) { Debug.Log("Right Touchpad touched at coords:" + deviceRightController.GetAxis(touchPad)); }
       */



    }
}
