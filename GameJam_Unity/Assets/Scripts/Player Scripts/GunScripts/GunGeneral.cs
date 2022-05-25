using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunGeneral : MonoBehaviour
{
    [SerializeField] protected Transform muzzle;
    [SerializeField] protected Camera cam;
    [SerializeField] private float damage;
    [SerializeField] private float reloadTime;
    [SerializeField] protected float rateOfFire;
    [SerializeField] protected int maxAmmo;
    [SerializeField, ReadOnly] protected int currentAmmo;
    [SerializeField] protected float maxDistance;

    [SerializeField, ReadOnly] protected float firingDelay;

    [SerializeField] Vector3 shootVector;
    [SerializeField] Vector3 shootQuaternion;

    private bool canFire;
    private Ray destRay;
    private RaycastHit hit;
    protected Vector3 dest;

    protected Vector3 direction;

    public float Damage { get => damage; set => damage = value; }
    public float ReloadTime { get => reloadTime; set => reloadTime = value; }

    public void OrientMuzzle()
    {
        destRay = cam.ViewportPointToRay(Input.mousePosition);
        if(Physics.Raycast(destRay, out hit))
        {
            dest = hit.point;
        }
        else
        {
            dest = destRay.GetPoint(100);
        }

        direction = dest - transform.position;
        direction = new Vector3(direction.x, direction.y + 20f, direction.z);
        muzzle.LookAt(direction);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
