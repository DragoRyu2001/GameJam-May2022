using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rifle : GunGeneral
{
    [SerializeField] bool enemyHit;
    Enemy hitEnemyComponent;
    private RaycastHit shotHit;


    // Start is called before the first frame update

    private void Start()
    {
        SetBaseParameters();
    }

    void Update()
    {
        OrientMuzzle();
        CanShootCheck();
        ReadInput();
        Debug.DrawRay(muzzle.position, dest, Color.blue);
    }

    private void ReadInput()
    {
        if (Input.GetMouseButton(0) && canShoot)
        {
            Debug.Log("Shot");
            Shoot();
            if (!inFireRateDelay)
                StartCoroutine(RateOfFireLimiter());
            if (currentAmmo == 0 && !reloading)
                StartCoroutine(Reload());
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            StopAllCoroutines();
            StartCoroutine(Reload());
        }
    }

    private void Shoot()
    {
        enemyHit = hit.transform.TryGetComponent(out hitEnemyComponent);
        if(enemyHit)
        {
            Debug.Log("Shot Enemy");
            hitEnemyComponent.TakeDamage(Damage, true);
        }
        else
        {
            Debug.Log("Shot Wall");
            //Decal stuff
        }
        currentAmmo -= 1;
        
    }

    // Update is called once per frame
}
