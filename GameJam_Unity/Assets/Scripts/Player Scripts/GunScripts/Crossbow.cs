using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crossbow : GunGeneral
{
    [SerializeField] GameObject bolt;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Shoot()
    {
        firingDelay = rateOfFire;
        Instantiate(bolt ,muzzle.position, Quaternion.Euler(muzzle.forward));
    }

    // Update is called once per frame
    void Update()
    {
        ReadInput();
    }

    private void ReadInput()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }
}
