using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("General")]
    [SerializeField] protected GameObject target;
    [SerializeField]protected NavMeshAgent agent;
    [SerializeField]protected float maxHealth;
    [ReadOnly, SerializeField]protected float currentHealth;
    [ReadOnly, SerializeField]protected bool isAlive;
    [SerializeField]protected Animator anim;
    [SerializeField] protected GameObject deathFX;
    
    
    public void Aggro(GameObject obj)
    {
        target = obj;
    }
    #region General
    public void TakeDamage(float dmg, bool isPlayer)
    {
        currentHealth-=dmg;
        if(isPlayer)
        {
            //GameManer info about Player
            //target = player
        }
    }
    protected void onDeath()
    {
        GameObject obj = Instantiate(deathFX, transform.position-new Vector3(0, 0.5f, 0), deathFX.transform.rotation);
        Collider[] cols = Physics.OverlapSphere(this.transform.position, 10f);
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
