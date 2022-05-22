using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimScript : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] float rate;
    float desiredFOV, elapsedTime = 0f;
    void Start()
    {
        desiredFOV = cam.fieldOfView;
        //StartCoroutine(nameof(LerpFov));
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(1))
        {
            elapsedTime = 0;
        }
        if (Input.GetMouseButton(1))
        {
            desiredFOV = 40;
        }
        else if(Input.GetMouseButtonUp(1))
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
