using UnityEngine;

public class Crossbow : GunGeneral
{
    [SerializeField] GameObject bolt;

    private void Start()
    {
        SetBaseParameters();
    }

    private void Update()
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
        Instantiate(bolt, muzzle.position, muzzle.rotation);
        currentAmmo -= 1;

    }
}
