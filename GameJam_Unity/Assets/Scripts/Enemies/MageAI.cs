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
    [SerializeField]float castPre;
    [SerializeField] float castTime;
    [SerializeField] float reloadTime;
    [SerializeField] GameObject AOESplash;
    [SerializeField] GameObject explode_PS;

    void Start()
    {
        SetTarget();
        SetBaseParameters();
    }
    void Update()
    {
        Move();
        if(currentHealth<=0&&isAlive)
        {
            isAlive = false;
            canAttack = false;
            onDeath();
        }
    }
    #region Shooting
    void Move()
    {
        if(isAlive)
        {
            if(Vector3.Distance(this.transform.position, target.transform.position)>shootingRange)
            {
                anim.SetFloat("Blend", 1, 0.5f, Time.deltaTime);
                agent.SetDestination(target.transform.position);
            }
            else
            {
                agent.ResetPath();
                anim.SetFloat("Blend", 0);
                if(canAttack)
                {
                    StartCoroutine(AOEShoot(target.transform.position));
                    canAttack = false;
                    Invoke("ResetShoot", reloadTime);
                }
            }
        }
    }

    IEnumerator AOEShoot(Vector3 targetPos)
    {
        anim.SetTrigger("Attack");
        yield return new WaitForSeconds(castPre);
        GameObject obj = Instantiate(AOESplash, new Vector3(targetPos.x, (targetPos.y-1f),targetPos.z), AOESplash.transform.rotation);
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
        GameObject explode = Instantiate(explode_PS,  new Vector3(targetPos.x, (targetPos.y-0.75f),targetPos.z), explode_PS.transform.rotation);
        Destroy(explode, 1f);
        
    }
    void ResetShoot()
    {
        canAttack = true;
    }
    #endregion
    
}
