using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwoopAttackTriggerScript : MonoBehaviour
{
    [SerializeField] Collider coll;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] GameObject clawDecal;

    GameObject newDecal;
    Vector3 instantiatePoint, collisionNormal;

    private Enemy enemyComponent;
    bool enemySuccess;
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Hello");
        if (other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("got enemy");
            enemySuccess = other.TryGetComponent<Enemy>(out enemyComponent);
            if(enemySuccess)
                enemyComponent.TakeDamage(2000, true);
        }
        else if(other.gameObject.CompareTag("Ground"))
        {
            Debug.Log("got Ground");
            instantiatePoint = other.ClosestPoint(transform.position);
            collisionNormal = transform.position - instantiatePoint;
            newDecal = Instantiate(clawDecal, instantiatePoint, Quaternion.identity);
            newDecal.transform.rotation = Quaternion.FromToRotation(newDecal.transform.forward, collisionNormal);
            newDecal.transform.Rotate(transform.forward, Random.Range(-180f, 180f));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
