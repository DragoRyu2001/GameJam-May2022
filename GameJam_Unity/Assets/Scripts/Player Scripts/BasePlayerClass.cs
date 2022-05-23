using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Weapon
{
    RIFLE = 0,
    SHOTGUN = 1,
    CROSSBOW = 2
}


public class BasePlayerClass : MonoBehaviour
{
    // Start is called before the first frame update

    public float maxHealth;
    [ReadOnly] public float currentHealth;
    public float maxMana;
    [ReadOnly] public float currentMana;
    public float sprintResource;
    [ReadOnly] public float currentSResource;
                                                                              
    public float berserkTime;                                                 
    [ReadOnly] public float currentBT;                                        
                                                                              
    [ReadOnly]public bool isAlive;                                            
    [ReadOnly]public bool inBerserk;                                          
    [ReadOnly]public bool canSprint;


    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        isAlive = CheckHealth();

        if(currentHealth/maxHealth<0.15f)
        {
            StartRecording();
        }

        if(!isAlive&&!inBerserk)
        {
            inBerserk = true;
            isAlive = true;
            Rewind();
        }
        else if(!isAlive&&inBerserk)
        {
            inBerserk = false;
            Death();
        }
    }

    private void StartRecording()
    {
        throw new NotImplementedException();
    }

    private void Rewind()
    {
        throw new NotImplementedException();
    }

    private void Death()
    {
        throw new NotImplementedException();
    }

    public bool CheckHealth()
    {

        return isAlive = currentHealth >= 0;
    }

    public bool CheckBerserking()
    {
        return inBerserk = currentBT >= 0;
    }

    public bool CheckSprint()
    {
        return canSprint = currentSResource >= 0;
    }

    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {

    }

    struct FrameStats
    {
        Vector3 playerPos;
        Vector3 playerVelocity;
        float playerHealth;
    }

}
