using System;
using System.Collections;
using UnityEngine;

public class MageAI : Enemy
{
    ///<summary>
    //This Scripts handles the AI logic of the Mage
    ///</summary>

    [Header("Shooting")]
    [SerializeField] float shootingRange;
    [SerializeField] float aoeRadius;
    [SerializeField] float castPre;
    [SerializeField] float castTime;
    [SerializeField] float reloadTime;
    [SerializeField] float damage;
    [SerializeField] GameObject AOESplash;
    [SerializeField] GameObject explode_PS;

    private Coffin coffinComponent;
    private PlayerScript playerComponent;

    void Start()
    {
        SetTarget();
        PhaseUpdate();
        SetBaseParameters();
    }

    private void PhaseUpdate()
    {
        reloadTime = GameManager.instance.MageCurrentReloadTime;
        damage = GameManager.instance.MageCurrentBaseDamage;
        maxHealth = GameManager.instance.MageCurrentHealth;
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
        Instantiate(AOESplash, new Vector3(targetPos.x, (targetPos.y),targetPos.z), AOESplash.transform.rotation);
        yield return new WaitForSeconds(castTime);
        
        Collider []collider = Physics.OverlapSphere(targetPos, aoeRadius);
        foreach(var hitCollider in collider)
        {
            if(hitCollider.CompareTag("Player"))
            {
                if(hitCollider.transform.root.TryGetComponent<PlayerScript>(out playerComponent))
                {
                    playerComponent.TakeDamage(damage);
                    Debug.Log("Player Took Damage");
                }
            }
            else if(hitCollider.CompareTag("Coffin"))
            {
                if(hitCollider.transform.root.TryGetComponent<Coffin>(out coffinComponent))
                {
                    coffinComponent.TakeDamage(damage/2);
                    Debug.Log("Coffin Took Damage");
                }
            }
        }
        
    }
    void ResetShoot()
    {
        canAttack = true;
    }
    #endregion
    
}
