using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crossbow : GunGeneral
{
    [SerializeField] GameObject bolt;
    [SerializeField] GameObject obj;

    // Start is called before the first frame update
    void Start()
    {

    }

    void Shoot()
    {
        firingDelay = rateOfFire;
        Instantiate(bolt, muzzle.position, muzzle.transform.rotation);
    }

    // Update is called once per frame
    void Update()
    {
        OrientMuzzle();
        ReadInput();
        ShowLook();
        Debug.Log(direction);
    }

    private void ShowLook()
    {
        obj.transform.position = dest;
    }

    private void ReadInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, direction);
    }
}
