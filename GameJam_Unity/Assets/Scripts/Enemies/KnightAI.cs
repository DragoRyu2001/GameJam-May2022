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
        canAttack = true;
    }

    
    void Update()
    {
        if(Vector3.Distance(this.transform.position, target.transform.position)<=swordRange)
        {
            agent.ResetPath();
            
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
        }
    }

    IEnumerator Attack()
    {
        yield return new WaitForSeconds(coolDown);
        anim.SetTrigger("Attack");
        yield return new WaitForSeconds(attackTime);
        canAttack = true;
    }
}
