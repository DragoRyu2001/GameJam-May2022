using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoCrossbow : GunGeneral
{
    [SerializeField] GameObject bolt;

    void Start()
    {
        SetBaseParameters();
    }

    // Update is called once per frame
    void Update()
    {
        OrientMuzzle();
        CanShootCheck();
        ReadInput();
    }

    private void ReadInput()
    {
        if (Input.GetMouseButton(0) && canShoot)
        {
            Shoot();
            if (!inFireRateDelay)
            {
                anim.SetTrigger("Attack");
                StartCoroutine(RateOfFireLimiter());
            }
            if(currentAmmo==0&&!reloading)
                StartCoroutine(Reload());
        }
        if(Input.GetKeyDown(KeyCode.R))
        {
            StopAllCoroutines();
            StartCoroutine(Reload());
        }
    }

    private void Shoot()
    {
        Instantiate(bolt, muzzle.position, muzzle.rotation);
        currentAmmo -= 1;
    }
}
