using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimScript : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] Camera cam;
    [SerializeField] float rate;
    float desiredFOV, elapsedTime = 0f;


    [Header("Look variables")]
    [SerializeField] float mouseSensX;
    [SerializeField] float mouseSensY;
    [SerializeField] float orbitYOffset;
    [SerializeField] float orbitXOffset;
    [SerializeField] float orbitRadius;
    [SerializeField] Transform orientation;

    float mouseX, mouseY;
    private float yRot;
    private float xRot;
    private float camXPos;
    private float camYPos;
    private float camZPos;

    void Start()
    {
        desiredFOV = cam.fieldOfView;
    }

    // Update is called once per frame
    void Update()
    {
        ReadMouseInput();
        Look();
        Aim();

    }

    private void ReadMouseInput()
    {
        mouseX = Input.GetAxisRaw("Mouse X");
        mouseY = Input.GetAxisRaw("Mouse Y");

        yRot += mouseX * mouseSensX; 
        xRot -= mouseY * mouseSensY;

        xRot = Mathf.Clamp(xRot, -90f, 90f);

        camXPos = Mathf.Sin(yRot) * orbitRadius + player.position.x + orbitXOffset;
        camYPos = player.position.y + orbitYOffset;
        camZPos = Mathf.Cos(yRot) * orbitRadius + player.position.z;       
    }
    private void Look()
    {
        cam.transform.SetPositionAndRotation(new Vector3(camXPos, camYPos, camZPos), Quaternion.Euler(xRot, 0, 0));
        cam.transform.LookAt(new Vector3(player.position.x, -xRot, player.position.z));
        orientation.rotation = Quaternion.Euler(0,yRot*Mathf.Rad2Deg,0);
    }


    private void Aim()
    {
        if (Input.GetMouseButtonDown(1))
        {
            elapsedTime = 0;
        }
        if (Input.GetMouseButton(1))
        {
            desiredFOV = 40;
        }
        else if (Input.GetMouseButtonUp(1))
        {
            elapsedTime = 0;
            desiredFOV = 60;
        }

        if (desiredFOV != cam.fieldOfView)
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, desiredFOV, elapsedTime / rate);
            elapsedTime += Time.deltaTime;
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
}
