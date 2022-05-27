using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightAI : Enemy
{
    [Header("Attack")]
    [SerializeField] float swordRange;
    [SerializeField] float coolDown;
    [SerializeField]float attackTime;
    
    bool canAttack;
    void Start()
    {
        SetTarget();
        canAttack = true;
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
    void Move()
    {
        if(isAlive)
        {
            if(Vector3.Distance(this.transform.position, target.transform.position)<=swordRange)
            {
                agent.ResetPath();
                anim.SetFloat("Blend", 0, 0.1f, Time.deltaTime);
                transform.LookAt(new Vector3(target.transform.position.x, this.transform.position.y, target.transform.position.z));
                if(canAttack)
                {
                    canAttack = false;
                    StartCoroutine(Attack());
                }
            }
            else
            {
                agent.SetDestination(target.transform.position);
                anim.SetFloat("Blend", 1, 0.5f, Time.deltaTime);
            }
        }
    }

    IEnumerator Attack()
    {
        yield return new WaitForSeconds(coolDown);
        anim.SetTrigger("Attack");
        yield return new WaitForSeconds(attackTime);
        canAttack = true;
    }
    public bool CanAttack()
    {
        return !canAttack;
    }
}
