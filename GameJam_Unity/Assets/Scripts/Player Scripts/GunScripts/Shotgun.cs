using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : GunGeneral
{

    [SerializeField, Range(0.1f, 1f)] float bloom;
    [SerializeField] int pellets;
    [SerializeField] AimScript aim;
    [ReadOnly] public Vector3[] directions;

    Enemy hitEnemyComponent;
    Collider[] enemyColliders;

    [SerializeField, Range(4f, 8f)] float chokeDistance;
    [SerializeField, Range(0.75f, 1.5f)] float chokeDelta;
    [SerializeField] bool choke;

    [SerializeField] float shortRange, medRange;

    [SerializeField] GameObject[] decalArray;

    int randIndex;
    private GameObject decal;
    Vector3 newDir;
    int hitCount;
    float distance;
    private void Start()
    {
        SetBaseParameters();
        directions = new Vector3[pellets];
    }

    private void Update()
    {
        OrientMuzzle();
        CanShootCheck();
        choke = aim.isAiming;
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
        anim.SetTrigger("Attack");
        for (int i = 0; i < pellets; i++)
        {
            newDir = Random.insideUnitCircle * bloom;
            newDir.z = dest.normalized.z - (chokeDistance + (choke?chokeDistance:0));
            newDir = muzzle.TransformDirection(newDir.normalized);
            directions[i] = newDir;
        }
        hitCount = 0;
        foreach (Vector3 dir in directions)
        {
            if (Physics.Raycast(muzzle.position, -dir, out hit, maxDistance, layersToCheck))
            {
                if (hit.transform.TryGetComponent(out hitEnemyComponent))
                {
                    Debug.Log(CalculateDamage(hit.point));
                    hitEnemyComponent.TakeDamage(CalculateDamage(hit.point), true);
                }
                else
                {
                    randIndex = Random.Range(0, decalArray.Length);
                    decal = Instantiate(decalArray[randIndex], hit.point, Quaternion.identity);
                    decal.transform.rotation = Quaternion.FromToRotation(decal.transform.forward, hit.normal);
                }
            }

        }
        enemyColliders = Physics.OverlapBox(muzzle.position + muzzle.forward * 2f, new Vector3(2, 2, 3));
        foreach(Collider col in enemyColliders)
        {
            if (col.TryGetComponent(out hitEnemyComponent))
            {
                Debug.Log("shockwave hit");
                hitEnemyComponent.TakeDamage(300, true);
            }
        }

    }

    protected float CalculateDamage(Vector3 hitPos)
    {
        distance = Vector3.Distance(muzzle.position, hitPos);
        if(distance<shortRange)
        {
            return Damage;
        }
        else if(distance<medRange)
        {
            return Damage - 5;
        }
        else
        {
            return Damage/2;
        }

    }

    void OnDrawGizmos()
    {
        foreach (Vector3 vect in directions)
        {
            Debug.DrawRay(muzzle.position, -vect * maxDistance);
            Debug.DrawRay(muzzle.position, -vect * medRange, Color.yellow);
            Debug.DrawRay(muzzle.position, -vect * shortRange, Color.green);
        }
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(muzzle.position + muzzle.forward * 2f, new Vector3(2, 2, 3));
    }
}
