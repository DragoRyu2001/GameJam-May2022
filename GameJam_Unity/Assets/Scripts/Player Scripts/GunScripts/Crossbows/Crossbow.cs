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
        ReadInput();
    }

    private void ReadInput()
    {
        if (Input.GetMouseButtonDown(0) && canShoot)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        Instantiate(bolt, muzzle.position, muzzle.rotation);
        currentAmmo -= 1;
        StartCoroutine(Reload());
    }
}
