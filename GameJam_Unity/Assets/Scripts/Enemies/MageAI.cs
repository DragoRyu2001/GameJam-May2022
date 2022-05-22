using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MageAI : Enemy
{
    ///<summary>
    //This Scripts handles the AI logic of the Mage
    ///</summary>
    
    

    [Header("Shooting")]
    [SerializeField] float shootingRange;
    [SerializeField] float aoeRadius;
    [SerializeField] float castTime;
    [SerializeField] float reloadTime;
    [SerializeField] GameObject AOESplash;
    
    

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
        if(Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(currentHealth, target);
        }
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
        if(Vector3.Distance(this.transform.position, target.transform.position)>shootingRange)
            agent.SetDestination(target.transform.position);
        else
        {
            agent.ResetPath();
            if(canShoot)
            {
                StartCoroutine(AOEShoot(target.transform.position));
                canShoot = false;
                Invoke("ResetShoot", reloadTime);
            }
        }
    }

    IEnumerator AOEShoot(Vector3 targetPos)
    {
        GameObject obj = Instantiate(AOESplash, targetPos, Quaternion.identity);
        yield return new WaitForSeconds(castTime);
        Collider []collider = Physics.OverlapSphere(targetPos, aoeRadius);
        foreach(var hitCollider in collider)
        {
            if(hitCollider.tag=="Player")
            {
                Debug.Log("Player Took Damage");
            }
            else if(hitCollider.tag=="Coffin")
            {
                Debug.Log("Coffin Took Damage");
            }
        }

        Destroy(obj);
        
    }
    void ResetShoot()
    {
        canShoot = true;
    }
    #endregion
   
}
