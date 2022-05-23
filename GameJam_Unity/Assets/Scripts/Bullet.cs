using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float damage;
    [SerializeField] float speed;
    [SerializeField] bool isPlayer;
    [SerializeField] GameObject player;
    [SerializeField] float maxDist;
    Vector3 startPos;
    bool canMove;
    
    void Start()
    {
        startPos = this.transform.position;
        canMove = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(transform.position, startPos)>maxDist)
        {
            Destroy(this.gameObject);
        }
        if(canMove)
        {
            transform.Translate(new Vector3(0, 0, 1f) * Time.deltaTime * speed);
        }

    }
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("This is colliding");
        if(isPlayer && other.transform.tag=="Enemy")
        {
            other.transform.gameObject.GetComponent<Enemy>().TakeDamage(damage, isPlayer);
            Destroy(this.gameObject, 5f);
        }
        else if(!isPlayer && other.transform.tag=="Player")
        {
            other.transform.gameObject.GetComponent<BasePlayerClass>().TakeDamage(damage);
            Debug.Log("Player Shot");
            Destroy(this.gameObject);
        }
        else if(other.transform.tag=="Level")
        {
            canMove = false;
            Destroy(this.gameObject, 5f);
        }
        
    }
    public void WhoISPlayer(GameObject obj)
    {
        player = obj;
    }
    
}
