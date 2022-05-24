using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject Player;

    PlayerScript pScript;
    // Start is called before the first frame update
    void Start()
    {
        pScript = Player.GetComponent<PlayerScript>();
    }

    public Vector3 WhereIsPlayer()
    {
        return Player.transform.position;
    }

    public void DamagePlayer(float damage)
    {
        pScript.TakeDamage(damage);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
