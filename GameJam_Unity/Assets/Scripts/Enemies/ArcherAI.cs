using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherAI : Enemy
{
    ///<summary>
    //This Scripts handles the AI logic of the Mage
    ///</summary>

    [Header("Shooting")]
    [SerializeField] float shootingRange;
    [SerializeField] float drawSpeed;
    [SerializeField] GameObject arrow;
    [SerializeField] LayerMask aimMask;
    [SerializeField] Transform arrowPos;
    [SerializeField] GameObject targetLookAt;
    
    

    bool canShoot;

    void Start()
    {
        canShoot = true;
        currentHealth = maxHealth;
        isAlive = true;
    }
    void Update()
    {
        Move();
        if(currentHealth<=0&&isAlive)
        {
            isAlive = false;
            canShoot = false;
            onDeath();
        }
    }
    #region Shooting
    void Move()
    {
        if(isAlive)
        {
            if(Vector3.Distance(this.transform.position, target.transform.position)<=shootingRange && CanShoot())
            {
                agent.ResetPath();
                anim.SetFloat("Blend", 0);
                targetLookAt.transform.position = target.transform.position+(Vector3.up*0.5f);
                transform.LookAt(new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z));
                arrowPos.LookAt(target.transform);
                if(canShoot)
                {
                    canShoot = false;
                    StartCoroutine(Shoot(target.transform.position));
                }
            }
            else
            {
                agent.SetDestination(target.transform.position);
                anim.SetFloat("Blend", 1, 0.5f, Time.deltaTime);
                targetLookAt.transform.position = transform.position+transform.forward*2f;
            }
        }
    }
    bool CanShoot()
    {
        Vector3 startPos = arrowPos.position;
        Vector3 dir = target.transform.position - startPos;
        dir = dir.normalized;
        if(Physics.Raycast(startPos, dir, out RaycastHit hit, shootingRange, aimMask))
        {
            if(hit.transform.tag=="Player")
            {
                return true;
            }
        }
        return false;
    }

    IEnumerator Shoot(Vector3 targetPos)
    {
        
        yield return new WaitForSeconds(drawSpeed);
        anim.SetTrigger("Attack");
        Instantiate(arrow, arrowPos.position, arrowPos.rotation);
        canShoot = true;
    }
    #endregion
   
}
