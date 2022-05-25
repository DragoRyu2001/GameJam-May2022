using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TripleCrossbow : GunGeneral
{
    [SerializeField] GameObject bolt;
    [SerializeField, Range(0.2f, 4f)] float yOffset, xOffset;

    Vector3 a, b, c;
    // Start is called before the first frame update
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
        if (Input.GetMouseButtonDown(0) && canShoot)
        {
            Shoot();
            StartCoroutine(Reload());
        }
    }

    private void Shoot()
    {
        a = new Vector3(muzzle.position.x, muzzle.position.y + (yOffset / 2), muzzle.position.z);
        b = new Vector3(muzzle.position.x + (xOffset/2) , muzzle.position.y - (yOffset / 2), muzzle.position.z);
        c = new Vector3(muzzle.position.x - (xOffset/2) , muzzle.position.y - (yOffset / 2), muzzle.position.z);
        Instantiate(bolt, a, muzzle.rotation);
        Instantiate(bolt, b, muzzle.rotation);
        Instantiate(bolt, c, muzzle.rotation);
        currentAmmo -= 1;
    }

}