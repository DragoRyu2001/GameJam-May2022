using UnityEngine;

public class Rifle : GunGeneral
{
    [SerializeField] bool enemyHit;
    Enemy hitEnemyComponent;

    [SerializeField] GameObject[] decalArray;

    int randIndex;
    private GameObject decal;
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
    }

    private void ReadInput()
    {
        if (Input.GetMouseButton(0) && canShoot)
        {
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
        if (hitSomething)
        {
            enemyHit = hit.transform.TryGetComponent(out hitEnemyComponent);
            if(enemyHit)
            {
                hitEnemyComponent.TakeDamage(Damage, true, playerScript.InBerserk);
            }
            else
            {
                randIndex = Random.Range(0, decalArray.Length);
                decal = Instantiate(decalArray[randIndex], hit.point, Quaternion.identity);
                decal.transform.rotation = Quaternion.FromToRotation(decal.transform.forward, hit.normal);
            }
        }
        currentAmmo -= 1;
        
    }

    // Update is called once per frame
}
