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

    [Header("Base Class Parameters")]
    [SerializeField] protected float maxHealth;
    [SerializeField, ReadOnly] protected float currentHealth;
    [SerializeField] protected float maxMana;
    [SerializeField, ReadOnly] protected float currentMana;
    [SerializeField] protected float sprintResource;
    [SerializeField, ReadOnly] protected float currentSResource;
    [SerializeField] protected float berserkTime;                                                 
    [SerializeField, ReadOnly] protected float currentBT;                                        
                                                                              
    [SerializeField, ReadOnly] protected bool isAlive;                                            
    [SerializeField, ReadOnly] protected bool inBerserk;                                          
    [SerializeField, ReadOnly] protected bool canSprint;
    [SerializeField, ReadOnly] protected bool canCast;

    [SerializeField] protected float manaRegenRate;
    [SerializeField] protected float healthRegenRate;
    [SerializeField] protected float sprintRegenRate;

    [SerializeField] protected bool manaRecharging=false;
    [SerializeField] protected bool healthRecharging;
    [SerializeField] protected bool sprintRecharging;


    [SerializeField] protected bool manaRechargePause;
    [SerializeField] protected bool healthRechargePause;
    [SerializeField] protected bool sprintRechargePause;

    [SerializeField] protected float manaCost;

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

    public void BaseParametersUpdate()
    {
        currentHealth = maxHealth;
        currentMana = maxMana;
        currentSResource = sprintResource;
        currentBT = berserkTime;
        CheckHealth();
        CheckSprint();
        CheckMana();
    }

    public bool CheckMana()
    {
        return canCast = currentMana - 30 >= 0f;
    }

    void Start()
    {
    
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

    public IEnumerator StartManaRecharge()
    {
        manaRecharging = true;
        while(currentMana<=maxMana)
        {
            currentMana += Time.deltaTime*manaRegenRate;
            yield return null;
            if (currentMana > maxMana || Mathf.Approximately(currentMana, maxMana))
            {
                currentMana = maxMana;
                yield break;
            }
            if (manaRechargePause)
            {
                manaRechargePause = false;
                Debug.Log("Pause");
                yield return new WaitForSeconds(1.25f);
            }
        }
        manaRecharging = false;
    }

}
