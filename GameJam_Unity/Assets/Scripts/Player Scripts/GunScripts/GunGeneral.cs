using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunGeneral : MonoBehaviour
{
    [SerializeField] protected Transform muzzle;

    [SerializeField] protected float damage;
    [SerializeField] protected float rateOfFire;
    [SerializeField] protected int maxAmmo;
    [SerializeField, ReadOnly] protected int currentAmmo;
    [SerializeField] protected float maxDistance;

    [SerializeField, ReadOnly] protected float firingDelay;
    [SerializeField] AimScript aimscript;

    [SerializeField] Vector3 shootVector;
    [SerializeField] Vector3 shootQuaternion;

    private bool canFire;

    protected Vector3 GetShootVector()
    {
        shootVector = aimscript.shootQuaternion * transform.forward;
        return shootVector;
    }    

    protected Quaternion GetShootQuaternion()
    {
        return aimscript.shootQuaternion;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
