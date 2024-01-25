using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementPower : MonoBehaviour
{

    public string powerName;
    public enum MovementPowerType { Run, Fly, Jump, TeleportBlink }

    public MovementPowerType movementPowerType;
    public float moveSpeed;
    public float moveBoost;
    public float powerCooldown;

    public GameObject targetArrowPrefab;

    private float curMoveBoost = 1;
    private float nextCastTime=0;
    private GameObject player;
    private GameObject tarArrow;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void MovePower(Vector2 touchCoords, bool isPressed)
    {
        if(nextCastTime>Time.time+powerCooldown) { nextCastTime = 0; }
        //Debug.Log("TouchPad touched at " + touchCoords + ", isPressed:" + isPressed);
        player = Camera.main.transform.parent.gameObject;

            if (movementPowerType == MovementPowerType.Run)
            {
                Vector3 rawMoveCoords3d = (Camera.main.transform.forward * touchCoords.y + Camera.main.transform.right * touchCoords.x);
                Vector3 moveCoords3d = new Vector3(rawMoveCoords3d.x, 0f, rawMoveCoords3d.z) * touchCoords.magnitude;

                //Debug.Log("Player is at " + player.transform.position + ", and should move in direction :" + rawMoveCoords3d);

                if (isPressed) { curMoveBoost = moveBoost; }
                else { curMoveBoost = 1; }

                player.transform.position += moveCoords3d * moveSpeed * curMoveBoost * Time.deltaTime;
            }
            else if (movementPowerType == MovementPowerType.Fly)
            {
                Vector3 moveCoords3d = (Camera.main.transform.forward * touchCoords.y + Camera.main.transform.right * touchCoords.x);

                //Debug.Log("Player is at " + player.transform.position + ", and should move in direction :" + rawMoveCoords3d);

                if (isPressed) { curMoveBoost = moveBoost; }
                else { curMoveBoost = 1; }

                player.transform.position += moveCoords3d * moveSpeed * curMoveBoost * Time.deltaTime;
            }
            else if (movementPowerType == MovementPowerType.TeleportBlink)
            {
                Vector3 moveCoords3d = (Camera.main.transform.forward * touchCoords.y + Camera.main.transform.right * touchCoords.x);
                moveCoords3d.y = 0;
                moveCoords3d.Normalize();
                moveCoords3d *= touchCoords.magnitude * moveSpeed;

            if (tarArrow)
            {
                Destroy(tarArrow);
            }
            tarArrow = Instantiate(targetArrowPrefab, player.transform.position + moveCoords3d, Quaternion.identity);


            //Debug.Log("Player is at " + player.transform.position + ", and should move in direction :" + rawMoveCoords3d);

            Debug.Log("nextCastTime: " + nextCastTime + ", Current Time: " + Time.time + ", isPressed?: " + isPressed);
            if (Time.time >= nextCastTime && isPressed)
            {
                player.transform.position = tarArrow.transform.position;
                nextCastTime = Time.time + powerCooldown;
            }


        }
        else if (movementPowerType == MovementPowerType.Jump)
        {

        }
        
    }

    public void StopMovePower()
    {
        if (tarArrow) { Destroy(tarArrow); }
    }
}

