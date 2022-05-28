using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwoopAttackTriggerScript : MonoBehaviour
{
    [SerializeField] Collider coll;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] LayerMask groundLayer;


    private Enemy enemyComponent;
    bool enemySuccess;
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer==enemyLayer)
        {
            Debug.Log("got enemy");
            enemySuccess = other.TryGetComponent<Enemy>(out enemyComponent);
            if(enemySuccess)
                enemyComponent.TakeDamage(2000, true);
        }
        else if(other.gameObject.layer==groundLayer)
        {
            Debug.Log("got Ground");
            //decal things;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
