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

    private bool canFire;

    

    // Update is called once per frame
    void Update()
    {
        
    }
}
