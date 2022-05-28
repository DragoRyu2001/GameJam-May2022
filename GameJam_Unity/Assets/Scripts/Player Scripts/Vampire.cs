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
        anim.SetFloat("Move", Mathf.Lerp(currentWalkAnimSpeed, desiredWalkAnimSpeed, 500f * Time.deltaTime));
        GroundCheck();
        onSlope = SlopeCheck();
        ControlDrag();
        ReadInput();
        slopeTranslateDirection = Vector3.ProjectOnPlane(translateVector, slopeHit.normal);
        RotateModelToOrientation();
    }
}
