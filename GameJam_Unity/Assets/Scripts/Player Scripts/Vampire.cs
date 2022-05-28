using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vampire : BasePlayerClass
{
    private float moveX;
    private float moveY;
    private float desiredWalkAnimSpeed;
    private float currentWalkAnimSpeed;
    private Vector3 x;
    private Vector3 y;
    private Vector3 newDir1;

    void Start()
    {
        BaseParametersUpdate();
        ControlDrag();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        translateModifier = 1f;
        if (!onSlope)
        {
            rb.AddForce(translateModifier * moveMult * translateVector.normalized / Time.timeScale, ForceMode.Acceleration);
        }
        else
        {
            rb.AddForce(translateModifier * moveMult * slopeTranslateDirection.normalized / Time.timeScale, ForceMode.Acceleration);
        }
    }

    private void ReadInput()
    {
        moveX = Input.GetAxisRaw("Horizontal");
        moveY = Input.GetAxisRaw("Vertical");
        translateVector = -orientation.forward * moveY - orientation.right * moveX;
    }


    void Update()
    {
        desiredWalkAnimSpeed = rb.velocity.magnitude < 2f ? 1f : rb.velocity.magnitude / 5f;

        GroundCheck();
        onSlope = SlopeCheck();
        ControlDrag();
        ReadInput();
        slopeTranslateDirection = Vector3.ProjectOnPlane(translateVector, slopeHit.normal);
        RotateModelToOrientation();
    }

    protected new void RotateModelToOrientation()
    {
        x = new Vector3(playerObj.forward.x, 0, playerObj.forward.z);
        y = -new Vector3(orientation.forward.x, 0, orientation.forward.z);
        if (x != y)
        {
            newDir1 = Vector3.RotateTowards(playerObj.forward, x - y, 20f * Time.deltaTime, 0f);
            playerObj.rotation = Quaternion.LookRotation(newDir1);
        }
    }

}
