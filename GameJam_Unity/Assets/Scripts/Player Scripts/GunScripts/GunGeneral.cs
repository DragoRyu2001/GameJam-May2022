using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunGeneral : MonoBehaviour
{
    [SerializeField] protected Transform muzzle;
    [SerializeField] protected Camera cam;
    [SerializeField] private float damage;
    [SerializeField] private float reloadTime;
    [SerializeField] protected float fireRate;
    [SerializeField, ReadOnly] protected float firingDelay;

    [SerializeField] protected int maxAmmo;
    [SerializeField, ReadOnly] protected int currentAmmo;
    [SerializeField, ReadOnly] protected bool canShoot = true;
    [SerializeField, ReadOnly] protected bool reloading;
    [SerializeField, ReadOnly] protected bool fired;
    [SerializeField, ReadOnly] protected bool inFireRateDelay;
    [SerializeField] protected float maxDistance;
    [SerializeField] protected LayerMask layersToCheck;




    protected Ray destRay;
    protected RaycastHit hit;
    protected Vector3 dest;


    public float Damage { get => damage; set => damage = value; }
    public float ReloadTime { get => reloadTime; set => reloadTime = value; }


    protected void SetBaseParameters()
    {
        currentAmmo = maxAmmo;
        canShoot = true;
        firingDelay = 0f;
        reloading = false;
        inFireRateDelay = false;
    }

    public void OrientMuzzle()
    {
        destRay = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        if (Physics.Raycast(destRay, out hit, 100f, layersToCheck))
        {
            dest = hit.point;
        }
        else
        {
            dest = destRay.GetPoint(100);
        }
        muzzle.LookAt(dest);
    }

    protected IEnumerator RateOfFireLimiter()
    {
        inFireRateDelay = true;
        yield return new WaitForSeconds(fireRate);
        inFireRateDelay = false;

    }


    protected IEnumerator Reload()
    {
        reloading = true;
        //animate here
        Debug.Log("Reloading");
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
        reloading = false;
        Debug.Log("Reloaded");

    }

    protected void CanShootCheck()
    {
        canShoot = !inFireRateDelay && !reloading && currentAmmo > 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
