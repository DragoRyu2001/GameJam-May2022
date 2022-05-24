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

    [Header("Health")]
    [SerializeField] protected float maxHealth;
    [SerializeField, ReadOnly] protected float currentHealth;
    [SerializeField] protected float healthRegenRate;
    [SerializeField] protected bool healthRecharging;
    [SerializeField] protected bool healthRechargePause;
    [SerializeField, Range(0.5f, 2f)] protected float healthRechargePauseTime;

    [Header("Mana")]
    [SerializeField] protected float maxMana;
    [SerializeField, ReadOnly] protected float currentMana;
    [SerializeField] protected float manaRegenRate;
    [SerializeField] protected bool manaRecharging;
    [SerializeField] protected bool manaRechargePause;
    [SerializeField, Range(0.5f, 2f)] protected float manaRechargePauseTime;

    [Header("Sprint")]
    [SerializeField] protected float maxSprint;
    [SerializeField, ReadOnly] protected float currentSprint;
    [SerializeField] protected float sprintRegenRate;
    [SerializeField] protected float sprintDecayRate;
    [SerializeField] protected bool sprintRecharging;
    [SerializeField] protected bool sprintRechargePause;
    [SerializeField, Range(0.5f, 2f)] protected float sprintRechargePauseTime;

    [Header("Berserk")]
    [SerializeField] protected float berserkTime;
    [SerializeField, ReadOnly] protected float currentBT;

    [Header("Dash Parameters")]
    [SerializeField] protected float dashForce;
    [SerializeField, Range(0.7f, 1.25f)] protected float dashReloadTime;

    [Header("Slow Mo Parameters")]
    [SerializeField] protected float slowTime;
    [SerializeField, Range(0.3f, 0.5f)] protected float slowMult;
    [SerializeField, Range(4f, 5f)] protected float slowReloadTime;

    [Header("Aggro Parameters")]
    [SerializeField] protected float aggroRange;
    [SerializeField, Range(0.7f, 1.25f)] protected float aggroReloadTime;

    [Header("Statuses")]
    [SerializeField, ReadOnly] protected bool isAlive;
    [SerializeField, ReadOnly] protected bool inBerserk;
    [SerializeField, ReadOnly] protected bool canSprint;
    [SerializeField, ReadOnly] protected bool canCast;

    [SerializeField] protected float manaCost;

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        isAlive = CheckHealth();

        if (currentHealth / maxHealth < 0.15f)
        {
            StartRecording();
        }

        if (!isAlive && !inBerserk)
        {
            inBerserk = true;
            isAlive = true;
            Rewind();
        }
        else if (!isAlive && inBerserk)
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
        return canSprint = currentSprint >= 0;
    }

    public void BaseParametersUpdate()
    {
        currentHealth = maxHealth;
        currentMana = maxMana;
        currentSprint = maxSprint;
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
        while (currentMana <= maxMana)
        {
            if (manaRechargePause)
            {
                manaRechargePause = false;
                Debug.Log("Pause");
                yield return new WaitForSeconds(manaRechargePauseTime);
            }
            currentMana += Time.deltaTime * manaRegenRate;
            yield return null;
            if (currentMana > maxMana || Mathf.Approximately(currentMana, maxMana))
            {
                currentMana = maxMana;
                yield break;
            }

        }
        manaRecharging = false;
    }

    public IEnumerator StartHealthRecharge()
    {
        healthRecharging = true;
        while (currentHealth <= maxHealth)
        {
            if (healthRechargePause)
            {
                healthRechargePause = false;
                Debug.Log("health Pause");
                yield return new WaitForSeconds(1.25f);
            }

            currentHealth += Time.deltaTime * healthRegenRate;
            yield return null;

            if (currentHealth > maxHealth || Mathf.Approximately(currentHealth, maxHealth))
            {
                currentHealth = maxHealth;
                yield break;
            }

        }
        healthRecharging = false;
    }

    public IEnumerator StartSprintRecharge()
    {
        sprintRecharging = true;
        while (currentSprint <= maxSprint)
        {
            if (sprintRechargePause)
            {
                sprintRechargePause = false;
                Debug.Log("Sprint Pause");
                yield return new WaitForSeconds(sprintRechargePauseTime);
            }
            currentSprint += Time.deltaTime * sprintRegenRate;
            yield return null;
            if (currentSprint > maxSprint || Mathf.Approximately(currentSprint, maxSprint))
            {
                currentSprint = maxSprint;
                yield break;
            }

        }
        sprintRecharging = false;
    }

}
