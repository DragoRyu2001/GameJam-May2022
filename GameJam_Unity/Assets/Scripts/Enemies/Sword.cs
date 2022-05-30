using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    [SerializeField] float damage;
    [SerializeField] KnightAI knight;
    
    void OnTriggerEnter(Collider col)
    {
        if(col.transform.root.CompareTag("Player") && knight.CanAttack())
        {
            col.transform.root.GetComponent<PlayerScript>().TakeDamage(damage);
        }
        else if(col.transform.root.CompareTag("Coffin") && knight.CanAttack())
        {
            col.transform.root.GetComponent<Coffin>().TakeDamage(damage);
        }
    }
}
