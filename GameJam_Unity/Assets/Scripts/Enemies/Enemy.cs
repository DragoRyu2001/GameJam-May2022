using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Enemy : MonoBehaviour
{
    [Header("General")]
    [SerializeField] protected GameObject target;
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] protected float maxHealth;
    [ SerializeField]protected float currentHealth;
    [ SerializeField]protected bool isAlive;
    [SerializeField]protected Animator anim;
    [SerializeField] protected GameObject deathFX;
    [ SerializeField]protected bool canAttack;

    protected void SetBaseParameters()
    {
        isAlive = true;
        currentHealth = maxHealth;
        canAttack = true;
    }
    
    public void Aggro(GameObject obj)
    {
        target = obj;
    }
    #region General
    public void TakeDamage(float dmg, bool isPlayer)
    {
        currentHealth-=(GameManager.instance.IsPlayerBerserking()?(1.5f*dmg):dmg);
        GameManager.instance.HitDetectHandler();
        //if(currentHealth>0)
        //{

        //}
        //else
        //{
        //    Debug.Log("red hitmarker");
        //}

        if(isPlayer)
            target = GameManager.instance.WhereIsPlayer();
    }

    public void SetTarget()
    {
        target = GameManager.instance.GiveTarget(this);
    }

    protected void onDeath()
    {
        GameObject obj = Instantiate(deathFX, transform.position-new Vector3(0, 0.5f, 0), deathFX.transform.rotation);
        Collider[] cols = Physics.OverlapSphere(this.transform.position, 10f);
        GameManager.instance.IncreaseKills();
        Debug.Log("Enemy has Died");
        if(cols.Length>0)
        {
            foreach(Collider col in cols)
            {
                if(col.transform.tag=="Enemy")
                {
                    Enemy enm = col.GetComponent<Enemy>();
                    enm.Aggro(target);
                }
            }
        }
        anim.SetTrigger("Death");
        Destroy(obj, 1f);
        Destroy(this.gameObject, 1f);
    }

    #endregion

}
