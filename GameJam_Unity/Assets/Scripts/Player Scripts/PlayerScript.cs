using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
enum CrossbowTypes
{
    SINGLE = 0,
    BURST = 1,
    AUTO = 2
}


public class PlayerScript : BasePlayerClass
{
    [Header("Sprint")]
    [SerializeField] private float maxSprint;
    [SerializeField, ReadOnly] private float currentSprint;
    [SerializeField] private float sprintRegenRate;
    [SerializeField] private float sprintDecayRate;
    [SerializeField] private bool sprintRecharging;
    [SerializeField] private bool sprintRechargePause;
    [SerializeField, Range(0.5f, 2f)] private float sprintRechargePauseTime;
    [SerializeField] private float sprintMult;

    [Header("Mana")]
    [SerializeField] private float maxMana;
    [SerializeField, ReadOnly] private float currentMana;
    [SerializeField] private float manaRegenRate;
    [SerializeField] private bool manaRecharging;
    [SerializeField] private bool manaRechargePause;
    [SerializeField, Range(0.5f, 2f)] private float manaRechargePauseTime;
    [SerializeField] private float manaCost;

    [SerializeField] float jumpForce;
    [Header("Dash Parameters")]
    [SerializeField, ReadOnly] private bool isDashing;
    [SerializeField] private float dashForce;
    [SerializeField, Range(0.5f, 1.75f)] private float dashReloadTime;

    [Header("Berserk")]
    [SerializeField] private float berserkTime;
    [SerializeField, ReadOnly] private float currentBT;
    [SerializeField] private float berserkHealthMult;

    [Header("Slow Mo Parameters")]
    [SerializeField, ReadOnly] private bool inSlowMo;
    [SerializeField] private float slowTime;
    [SerializeField, Range(0.3f, 0.6f)] private float slowMult;
    [SerializeField, Range(4f, 8f)] private float slowReloadTime;

    [Header("Aggro Parameters")]
    [SerializeField, ReadOnly] private bool canCall;
    [SerializeField] private float aggroRange;
    [SerializeField, Range(4f, 5f)] private float aggroReloadTime;

    [Header("Misc References")]
    [SerializeField] WerewolfScript werewolfScript;
    [Header("Coffin Parameters")]
    [SerializeField] Coffin coffin;
    [SerializeField] float coffinRepairSpeed;

    [Header("UI Tie-ins")]
    [SerializeField] Material healthMat;
    [SerializeField] Material manaMat;
    [SerializeField] Material sprintMat;

    [Header("Gun variables")]
    [SerializeField] GameObject[] gunArray;
    [SerializeField] int currentGun;
    [SerializeField] int previousGun;
    [SerializeField] CrossbowTypes currentType;

    private float moveX;
    private float moveY;
    private Collider[] aggroArr;
    private Enemy enemyComponent;
    bool gotEnemy;
    protected List<FrameStats> recordedData;
    private float desiredWalkAnimSpeed;
    private float currentWalkAnimSpeed;


    #region Setters and Getters
    public float MaxSprint { get => maxSprint; set => maxSprint = value; }
    public float CurrentSprint { get => currentSprint; set => currentSprint = value; }
    public float SprintRegenRate { get => sprintRegenRate; set => sprintRegenRate = value; }
    public float SprintDecayRate { get => sprintDecayRate; set => sprintDecayRate = value; }
    public bool SprintRecharging { get => sprintRecharging; set => sprintRecharging = value; }
    public bool SprintRechargePause { get => sprintRechargePause; set => sprintRechargePause = value; }
    public float SprintRechargePauseTime { get => sprintRechargePauseTime; set => sprintRechargePauseTime = value; }
    public float DashForce { get => dashForce; set => dashForce = value; }
    public float DashReloadTime { get => dashReloadTime; set => dashReloadTime = value; }
    public bool IsDashing { get => isDashing; set => isDashing = value; }
    public float BerserkTime { get => berserkTime; set => berserkTime = value; }
    public float CurrentBT { get => currentBT; set => currentBT = value; }
    public float SlowTime { get => slowTime; set => slowTime = value; }
    public float SlowMult { get => slowMult; set => slowMult = value; }
    public float SlowReloadTime { get => slowReloadTime; set => slowReloadTime = value; }
    public float AggroRange { get => aggroRange; set => aggroRange = value; }
    public float AggroReloadTime { get => aggroReloadTime; set => aggroReloadTime = value; }
    public bool InSlowMo { get => inSlowMo; set => inSlowMo = value; }
    public bool CanCall { get => canCall; set => canCall = value; }
    public float BerserkHealthMult { get => berserkHealthMult; set => berserkHealthMult = value; }
    public float MaxMana { get => maxMana; set => maxMana = value; }
    public float CurrentMana { get => currentMana; set => currentMana = value; }
    public float ManaRegenRate { get => manaRegenRate; set => manaRegenRate = value; }
    public bool ManaRecharging { get => manaRecharging; set => manaRecharging = value; }
    public bool ManaRechargePause { get => manaRechargePause; set => manaRechargePause = value; }
    public float ManaRechargePauseTime { get => manaRechargePauseTime; set => manaRechargePauseTime = value; }
    public float ManaCost { get => manaCost; set => manaCost = value; }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        BaseParametersUpdate();
        ServantSpecificUpdates();
        recordedData = new List<FrameStats>();
        ControlDrag();
        gunArray[currentGun].SetActive(true);
        currentGun = previousGun;
    }

    private void ServantSpecificUpdates()
    {
        CurrentBT = BerserkTime;
        canCall = true;
        CurrentMana = MaxMana;
        CurrentSprint = MaxSprint;
        CheckSprint();
        CheckMana();
    }

    private void FixedUpdate()
    {
        if(!IsRewinding)
        {
            Move();
            if(Critical&&IsAlive)
            {
                StartRecording();
            }
        }
        else
        {
            rb.isKinematic = true;
            Rewind();
        }
    }

    void Update()
    {
        if(IsAlive)
        {
            if (!IsRewinding)
            {
                if(rb.velocity.magnitude>0.2f&&GroundCheck())
                {
                    if(Mathf.Abs(Vector3.SignedAngle(transform.forward, rb.velocity.normalized, Vector3.up))>90f)
                    {
                        anim.SetFloat("Move", 2, 0.1f, Time.deltaTime);
                    }
                    else
                    {
                        anim.SetFloat("Move", 0, 0.1f, Time.deltaTime);
                    }
                }
                else
                {
                    anim.SetFloat("Move", 1, 0.1f, Time.deltaTime);
                }

                GroundCheck();
                onSlope = SlopeCheck();
                ControlDrag();
                SprintCheck();
                ReadInput();
                slopeTranslateDirection = Vector3.ProjectOnPlane(translateVector, slopeHit.normal);
                RotateModelToOrientation();
                UpdateUI();
                if(InBerserk)
                {
                    CurrentBT -= Time.deltaTime;
                    if (CurrentBT <= 0)
                        CurrentBT = BerserkTime;
                }
            }
        }

    }

    private void UpdateUI()
    {
        healthMat.SetFloat("_Health", CurrentHealth / MaxHealth);
        manaMat.SetFloat("_Health", currentMana / maxMana);
        sprintMat.SetFloat("_Health", currentSprint / maxSprint);
    }

    private void ReadInput()
    {
        //keyboard input
        moveX = Input.GetAxisRaw("Horizontal");
        moveY = Input.GetAxisRaw("Vertical");
        translateVector = -orientation.forward * moveY - orientation.right * moveX;

        //jump input
        if (Input.GetKeyDown(KeyCode.Space) && GroundCheck())
        {
            Jump();
        }

        //dash input
        if (Input.GetKeyDown(KeyCode.Q) && CheckMana())
        {
            if (!IsDashing)
            {
                IsDashing = true;
                Reposition();
                Invoke(nameof(SetDashingToFalse), DashReloadTime / Time.timeScale);
            }
        }

        //Slow mo input trigger
        if (Input.GetKeyDown(KeyCode.E) && CheckMana())
        {
            if (!InSlowMo)
            {
                TriggerSlowMo();


                Invoke(nameof(SetInSlowMoToFalse), SlowReloadTime / Time.timeScale);
            }
            else if (InSlowMo)
            {
                SetInSlowMoToFalse();
            }
        }
        //AggroCall input
        if (Input.GetKeyDown(KeyCode.Z) && CheckMana())
        {
            if (CanCall)
            {
                CanCall = false;
                TriggerAggroCall();
                Invoke(nameof(SetCanCallToTrue), AggroReloadTime);
            }
        }

        //Ultimate Input
        if (Input.GetKey(KeyCode.X) && CheckUltMana())
        {
            anim.StopPlayback();
            anim.SetTrigger("Death");
            Invoke(nameof(TurnToWerewolf), 1.2f);
        }


        //Weapon switching 
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentGun = 0;
            anim.SetInteger("Weapon", currentGun);
            anim.SetTrigger("ChangeWeapon");
            if (currentGun != previousGun)
            {
                gunArray[previousGun].SetActive(false);
                gunArray[currentGun].SetActive(true);
                previousGun = currentGun;
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentGun = 1;
            anim.SetInteger("Weapon", currentGun);
            anim.SetTrigger("ChangeWeapon");
            if (currentGun != previousGun)
            {
                gunArray[previousGun].SetActive(false);
                gunArray[currentGun].SetActive(true);
                previousGun = currentGun;
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            currentGun = 2;
            currentGun += (int)currentType;
            if(currentGun==4)
            {
                anim.SetBool("AutoCrossbow",true);
            }
            anim.SetInteger("Weapon", 2);
            anim.SetTrigger("ChangeWeapon");
            if (currentGun != previousGun)
            {
                gunArray[previousGun].SetActive(false);
                gunArray[currentGun].SetActive(true);
                previousGun = currentGun;
            }
        }

        if(Input.GetKey(KeyCode.C)&&coffin.GetIfPlayerInRepairRange())
        {
            coffin.HealCoffin(Time.deltaTime*coffinRepairSpeed);
        }
    }

    private void TurnToWerewolf()
    {
        //rb.AddForce(Vector3.up * 2f, ForceMode.Impulse);
        servantModel.SetActive(false);
        werewolfModel.SetActive(true);

        coll.enabled = false;
        otherCollider.enabled = true;

        werewolfScript.enabled = true;
        enabled = false;

        gunArray[currentGun].SetActive(false);
    }

    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }


    public void TakeDamage(float damage)
    {
        if(!IsRewinding)
        {
            CurrentHealth -= damage;
            IsAlive = CheckHealth();
            HealthRechargePause = true;
            if (!HealthRecharging)
                StartCoroutine(StartHealthRecharge());

            if (CurrentHealth / MaxHealth < 0.25f)
            {
                Critical = true;
            }

            if (!IsAlive && !InBerserk)
            {
                InBerserk = true;
                IsAlive = true;
                StopAllCoroutines();
                Rewind();
            }
            else if (!IsAlive && InBerserk)
            {
                InBerserk = false;
                Death();
            }
        }
    }

    public void StartRecording()
    {
        recordedData.Insert(0, new FrameStats(transform.position, transform.rotation));
        if(recordedData.Count > Mathf.Round(5f/Time.fixedDeltaTime))
        {
            recordedData.RemoveAt(recordedData.Count - 1);
        }
    }

    public void Rewind()
    {
        if(recordedData.Count>0)
        {
            transform.position = recordedData[0].playerPos;
            playerObj.rotation = recordedData[0].playerRot;
            recordedData.RemoveAt(0);
            IsRewinding = true;
        }
        else
        {
            InBerserk = true;
            IsRewinding = false;
            Time.timeScale = 1f;
            rb.isKinematic = false;
            CurrentHealth = BerserkHealthMult * MaxHealth;
            Invoke(nameof(SetIsBerserkToFalse), BerserkTime);
        }
    }

    void SetIsBerserkToFalse()
    {
        InBerserk = false;
        StartCoroutine(StartHealthRecharge());
        StartCoroutine(StartManaRecharge());
        StartCoroutine(StartSprintRecharge());
    }

    private void TriggerAggroCall()
    {
        SubtractMana();

        aggroArr = Physics.OverlapSphere(transform.position, AggroRange, enemyLayer);
        foreach(Collider col in aggroArr)
        {
            gotEnemy = col.TryGetComponent(out enemyComponent);
            if(gotEnemy)
            {
                enemyComponent.Aggro(gameObject);
                enemyComponent.TakeDamage(40, true);
            }
        }

    }

    private void SetCanCallToTrue()
    {
        CanCall = true;
    }

    private void SetInSlowMoToFalse()
    {
        InSlowMo = false;
        Time.timeScale = 1f;
    }

    public int GetCrossbowType()
    {
        return (int)currentType;
    }

    public void SetCrossbowType(int set)
    {
        currentType = (CrossbowTypes)set;
    }

    private void TriggerSlowMo()
    {
        SubtractMana();
        InSlowMo = true;
        ManaRechargePause = true;
        if (!ManaRecharging)
            StartCoroutine(StartManaRecharge());
        Time.timeScale *= SlowMult;
        Invoke(nameof(SetInSlowMoToFalse), SlowReloadTime);
    }

    private void SubtractMana()
    {
        CurrentMana -= ManaCost;
        ManaRechargePause = true;
        if (!ManaRecharging)
            StartCoroutine(StartManaRecharge());
    }

    private void SetDashingToFalse()
    {
        IsDashing = false;
    }

    private void Reposition()
    {
        SubtractMana();

        Vector3 ShootVector = playerCam.forward;
        rb.AddForce(ShootVector * DashForce, ForceMode.Impulse);
    }

    protected void Move()
    {
        translateModifier = 1f;
        translateModifier = SprintCheck() ? sprintMult / moveMult : 1f;
        translateModifier = GroundCheck() ? translateModifier : airMoveMult;
        if (!onSlope)
        {
            rb.AddForce(translateModifier * moveMult * translateVector.normalized / Time.timeScale, ForceMode.Acceleration);
        }
        else
        {
            rb.AddForce(translateModifier * moveMult * slopeTranslateDirection.normalized / Time.timeScale, ForceMode.Acceleration);
        }
    }



    #region Player resource checks

    protected bool SprintCheck()
    {
        if (Input.GetKey(KeyCode.LeftShift) && CheckSprint() && Input.GetKey(KeyCode.W))
        {
            anim.SetBool("Sprint", true);
            CurrentSprint -= Time.deltaTime * SprintDecayRate/Time.timeScale;
            SprintRechargePause = true;
            return true;
        }
        else
        {
            if (CurrentSprint < MaxSprint)
            {
                if (!SprintRecharging)
                {
                    StartCoroutine(StartSprintRecharge());
                }
            }
            
            anim.SetBool("Sprint", false);
            return false;
        }
    }

    public bool CheckMana()
    {
        return CanCast = CurrentMana - 30 >= 0f;
    }
    public bool CheckUltMana()
    {
        return currentMana >= maxMana;
    }
    public bool CheckSprint()
    {
        return CurrentSprint >= 0;
    }

    #endregion

    #region Player specific resource recharges
    public IEnumerator StartSprintRecharge()
    {
        SprintRecharging = true;
        while (CurrentSprint < MaxSprint)
        {
            if (SprintRechargePause)
            {
                SprintRechargePause = false;
                yield return new WaitForSecondsRealtime(SprintRechargePauseTime);
            }
            CurrentSprint += Time.deltaTime * SprintRegenRate/Time.timeScale;
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

    public IEnumerator StartManaRecharge()
    {
        ManaRecharging = true;
        while (CurrentMana < MaxMana)
        {
            if (ManaRechargePause)
            {
                ManaRechargePause = false;
                yield return new WaitForSecondsRealtime(ManaRechargePauseTime);
            }
            CurrentMana += Time.deltaTime * ManaRegenRate/Time.timeScale;
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

   
    
    #endregion

    #region Debugs
    private void OnDrawGizmosSelected()
    {

    }
    #endregion
}
