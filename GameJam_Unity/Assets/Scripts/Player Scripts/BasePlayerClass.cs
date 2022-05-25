using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePlayerClass : MonoBehaviour
{
    // Start is called before the first frame update

    [Header("Health")]
    [SerializeField] private float maxHealth;
    [SerializeField, ReadOnly] private float currentHealth;
    [SerializeField] private float healthRegenRate;
    [SerializeField] private bool healthRecharging;
    [SerializeField] private bool healthRechargePause;
    [SerializeField, Range(0.5f, 2f)] private float healthRechargePauseTime;

    [Header("Mana")]
    [SerializeField] private float maxMana;
    [SerializeField, ReadOnly] private float currentMana;
    [SerializeField] private float manaRegenRate;
    [SerializeField] private bool manaRecharging;
    [SerializeField] private bool manaRechargePause;
    [SerializeField, Range(0.5f, 2f)] private float manaRechargePauseTime;

    [Header("Sprint")]
    [SerializeField] private float maxSprint;
    [SerializeField, ReadOnly] private float currentSprint;
    [SerializeField] private float sprintRegenRate;
    [SerializeField] private float sprintDecayRate;
    [SerializeField] private bool sprintRecharging;
    [SerializeField] private bool sprintRechargePause;
    [SerializeField, Range(0.5f, 2f)] private float sprintRechargePauseTime;

    [Header("Berserk")]
    [SerializeField] private float berserkTime;
    [SerializeField, ReadOnly] private float currentBT;

    [Header("Dash Parameters")]
    [SerializeField] private float dashForce;
    [SerializeField, Range(0.7f, 1.25f)] private float dashReloadTime;

    [Header("Slow Mo Parameters")]
    [SerializeField] private float slowTime;
    [SerializeField, Range(0.3f, 0.5f)] private float slowMult;
    [SerializeField, Range(4f, 5f)] private float slowReloadTime;

    [Header("Aggro Parameters")]
    [SerializeField] private float aggroRange;
    [SerializeField, Range(0.7f, 1.25f)] private float aggroReloadTime;

    [Header("Statuses")]
    [SerializeField, ReadOnly] private bool isAlive;
    [SerializeField, ReadOnly] private bool inBerserk;
    [SerializeField, ReadOnly] private bool canSprint;
    [SerializeField, ReadOnly] private bool canCast;

    [SerializeField] private float manaCost;

    public float MaxHealth { get => maxHealth; set => maxHealth = value; }
    public float CurrentHealth { get => currentHealth; set => currentHealth = value; }
    public float HealthRegenRate { get => healthRegenRate; set => healthRegenRate = value; }
    public bool HealthRecharging { get => healthRecharging; set => healthRecharging = value; }
    public bool HealthRechargePause { get => healthRechargePause; set => healthRechargePause = value; }
    public float HealthRechargePauseTime { get => healthRechargePauseTime; set => healthRechargePauseTime = value; }
    public float MaxMana { get => maxMana; set => maxMana = value; }
    public float CurrentMana { get => currentMana; set => currentMana = value; }
    public float ManaRegenRate { get => manaRegenRate; set => manaRegenRate = value; }
    public bool ManaRecharging { get => manaRecharging; set => manaRecharging = value; }
    public bool ManaRechargePause { get => manaRechargePause; set => manaRechargePause = value; }
    public float ManaRechargePauseTime { get => manaRechargePauseTime; set => manaRechargePauseTime = value; }
    public float MaxSprint { get => maxSprint; set => maxSprint = value; }
    public float CurrentSprint { get => currentSprint; set => currentSprint = value; }
    public float SprintRegenRate { get => sprintRegenRate; set => sprintRegenRate = value; }
    public float SprintDecayRate { get => sprintDecayRate; set => sprintDecayRate = value; }
    public bool SprintRecharging { get => sprintRecharging; set => sprintRecharging = value; }
    public bool SprintRechargePause { get => sprintRechargePause; set => sprintRechargePause = value; }
    public float SprintRechargePauseTime { get => sprintRechargePauseTime; set => sprintRechargePauseTime = value; }
    public float BerserkTime { get => berserkTime; set => berserkTime = value; }
    public float CurrentBT { get => currentBT; set => currentBT = value; }
    public float DashForce { get => dashForce; set => dashForce = value; }
    public float DashReloadTime { get => dashReloadTime; set => dashReloadTime = value; }
    public float SlowTime { get => slowTime; set => slowTime = value; }
    public float SlowMult { get => slowMult; set => slowMult = value; }
    public float SlowReloadTime { get => slowReloadTime; set => slowReloadTime = value; }
    public float AggroRange { get => aggroRange; set => aggroRange = value; }
    public float AggroReloadTime { get => aggroReloadTime; set => aggroReloadTime = value; }
    public bool IsAlive { get => isAlive; set => isAlive = value; }
    public bool InBerserk { get => inBerserk; set => inBerserk = value; }
    public bool CanSprint { get => canSprint; set => canSprint = value; }
    public bool CanCast { get => canCast; set => canCast = value; }
    public float ManaCost { get => manaCost; set => manaCost = value; }

    public void TakeDamage(float damage)
    {
        CurrentHealth -= damage;
        IsAlive = CheckHealth();

        if (CurrentHealth / MaxHealth < 0.15f)
        {
            StartRecording();
        }

        if (!IsAlive && !InBerserk)
        {
            InBerserk = true;
            IsAlive = true;
            Rewind();
        }
        else if (!IsAlive && InBerserk)
        {
            InBerserk = false;
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
        return IsAlive = CurrentHealth >= 0;
    }

    public bool CheckBerserking()
    {
        return InBerserk = CurrentBT >= 0;
    }

    public bool CheckSprint()
    {
        return CanSprint = CurrentSprint >= 0;
    }

    public void BaseParametersUpdate()
    {
        CurrentHealth = MaxHealth;
        CurrentMana = MaxMana;
        CurrentSprint = MaxSprint;
        CurrentBT = BerserkTime;
        CheckHealth();
        CheckSprint();
        CheckMana();
    }

    public bool CheckMana()
    {
        return CanCast = CurrentMana - 30 >= 0f;
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
        ManaRecharging = true;
        while (CurrentMana < MaxMana)
        {
            if (ManaRechargePause)
            {
                ManaRechargePause = false;
                Debug.Log("Pause");
                yield return new WaitForSeconds(ManaRechargePauseTime);
            }
            CurrentMana += Time.deltaTime * ManaRegenRate;
            yield return null;
            if (CurrentMana > MaxMana || Mathf.Approximately(CurrentMana, MaxMana))
            {
                CurrentMana = MaxMana;
                ManaRecharging = false;
                yield break;
            }
        }
        ManaRecharging = false;
    }

    public IEnumerator StartHealthRecharge()
    {
        HealthRecharging = true;
        while (CurrentHealth < MaxHealth)
        {
            if (HealthRechargePause)
            {
                HealthRechargePause = false;
                Debug.Log("health Pause");
                yield return new WaitForSeconds(1.25f);
            }

            CurrentHealth += Time.deltaTime * HealthRegenRate;
            yield return null;

            if (CurrentHealth > MaxHealth || Mathf.Approximately(CurrentHealth, MaxHealth))
            {
                CurrentHealth = MaxHealth;
                HealthRecharging = false;
                yield break;
            }

        }
        HealthRecharging = false;
    }

    public IEnumerator StartSprintRecharge()
    {
        SprintRecharging = true;
        while (CurrentSprint < MaxSprint)
        {
            if (SprintRechargePause)
            {
                SprintRechargePause = false;
                Debug.Log("Sprint Pause");
                yield return new WaitForSeconds(SprintRechargePauseTime);
            }
            CurrentSprint += Time.deltaTime * SprintRegenRate;
            yield return null;
            if (CurrentSprint > MaxSprint || Mathf.Approximately(CurrentSprint, MaxSprint))
            {
                CurrentSprint = MaxSprint;
                SprintRecharging = false;
                yield break;
            }

        }
        SprintRecharging = false;
    }

}
