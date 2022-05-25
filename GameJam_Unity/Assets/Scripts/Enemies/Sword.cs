using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    [SerializeField] float damage;
    [SerializeField] KnightAI knight;
    
    void OnTriggerEnter(Collider col)
    {
        if(col.transform.tag=="Player" && knight.CanAttack())
        {
            //col.transform.gameObject.GetComponent<BasePlayerClass>().TakeDamage(damage);
            Debug.Log("STRIKE!!!: "+damage);
        }
    }
}
