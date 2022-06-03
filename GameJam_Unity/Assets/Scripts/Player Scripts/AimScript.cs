using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimScript : MonoBehaviour
{
    [SerializeField] Transform lookPosition;
    [SerializeField] Camera cam;
    [SerializeField] float rate, fovSpeed, speedfovDesired;
    [SerializeField] PlayerScript player;

    [SerializeField] WerewolfScript wScript;
    float desiredFOV, elapsedTime = 0f;
    bool wasFast = false;

    [Header("Look variables")]
    [SerializeField] float mouseSensX;
    [SerializeField] float mouseSensY;
    [SerializeField] float orbitRadius;
    [SerializeField] Transform orientation;

    float mouseX, mouseY;
    private float yRot;
    private float xRot;
    private float camXPos;
    private float camYPos;
    private float camZPos;
    public bool isAiming;

    void Start()
    {
        desiredFOV = cam.fieldOfView;
    }

    // Update is called once per frame
    void Update()
    {
        ReadMouseInput();
        Look();
        if (wScript.enabled != false)
        {
            return;
        }
        Aim();
        
    }

    private void ReadMouseInput()
    {
        mouseX = Input.GetAxisRaw("Mouse X");
        mouseY = Input.GetAxisRaw("Mouse Y");

        yRot += mouseX * mouseSensX; 
        xRot -= mouseY * mouseSensY;

        xRot = Mathf.Clamp(xRot, -90f, 90f);

        camXPos = Mathf.Sin(yRot) * orbitRadius + lookPosition.position.x;
        camYPos = lookPosition.position.y;
        camZPos = Mathf.Cos(yRot) * orbitRadius + lookPosition.position.z;       
    }
    private void Look()
    {
        transform.position = new Vector3(camXPos, camYPos, camZPos);
        if(Physics.Linecast(lookPosition.position, transform.position, out RaycastHit hit))
        {
            if(hit.transform.tag=="Level")
            {   
                transform.position = hit.point;
            }
        }
        transform.LookAt(new Vector3(lookPosition.position.x, -xRot+lookPosition.position.y, lookPosition.position.z));
        orientation.rotation = Quaternion.Euler(0, yRot*Mathf.Rad2Deg,0);
    }


    private void Aim()
    {
        Debug.Log("Player Velocity: "+player.RB.velocity.magnitude);
        if(player.RB.velocity.magnitude>=fovSpeed && !wasFast)
        {
            wasFast = true;
            desiredFOV = speedfovDesired;
            elapsedTime = 0;
        }
        else if(player.RB.velocity.magnitude<fovSpeed && wasFast)
        {
            wasFast = false;
            desiredFOV = 60;
            elapsedTime = 0;
        }
        if (Input.GetMouseButtonDown(1))
        {
            isAiming = true;
            elapsedTime = 0;
        }
        if (Input.GetMouseButton(1))
        {
            desiredFOV = 40;
        }
        else if (Input.GetMouseButtonUp(1))
        {
            isAiming = false;
            elapsedTime = 0;
            desiredFOV = 60;
        }

        if (desiredFOV != cam.fieldOfView)
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, desiredFOV, elapsedTime / rate);
            elapsedTime += Time.deltaTime/Time.timeScale;
            if (elapsedTime > rate)
            {
                cam.fieldOfView = desiredFOV;
                elapsedTime = 0f;
            }
        }
        else
        {
            elapsedTime = 0;
        }
    }
    public void SetPlayer(Transform pos, Transform ori)
    {
        lookPosition = pos;
        orientation = ori;
    }
}
