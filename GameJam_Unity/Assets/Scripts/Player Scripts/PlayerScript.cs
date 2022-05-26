using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PlayerScript : BasePlayerClass
{

    [Header("Movement variables")]
    [SerializeField] float moveMult;
    [SerializeField] float sprintMult;
    [SerializeField, Range(0.1f, 0.5f)] float airMoveMult;
    [SerializeField] Transform orientation;
    RaycastHit slopeHit;

    [Header("Jump/Drag variables")]
    [SerializeField] float jumpForce;
    [SerializeField] float groundDrag;
    [SerializeField] float airDrag;
    [SerializeField, Range(0.01f, 0.3f)] float fallingDrag;
    [SerializeField, Range(-0.8f, -0.1f)] float fallThreshold;

    [Header("Misc References")]
    [SerializeField] Transform playerCam;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] Collider coll;
    [SerializeField] Transform playerObj;

    private float moveX;
    private float moveY;
    private float angle;
    private float playerHeight;
    private Collider[] aggroArr;
    private Enemy enemyComponent;
    bool onSlope, gotEnemy;

    Vector3 translateVector;
    Vector3 slopeTranslateDirection;
    [Header("Translate Modifier")]
    [SerializeField, ReadOnly] float translateModifier;
    private Vector3 rotateVector;


    // Start is called before the first frame update
    void Start()
    {
        BaseParametersUpdate();
        recordedData = new List<FrameStats>();
        ControlDrag();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        playerHeight = coll.bounds.size.y;
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
        Debug.Log(CurrentHealth);
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            TakeDamage(20);
        }
        if(IsAlive)
        {
            if (!IsRewinding)
            {
                GroundCheck();
                onSlope = SlopeCheck();
                ControlDrag();
                SprintCheck();
                ReadInput();
                slopeTranslateDirection = Vector3.ProjectOnPlane(translateVector, slopeHit.normal);
                RotateModelToOrientation();
                if(InBerserk)
                {
                    CurrentBT -= Time.deltaTime;
                    if (CurrentBT <= 0)
                        CurrentBT = BerserkTime;
                }
            }
        }

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

        if (Input.GetKeyDown(KeyCode.Q) && CheckMana())
        {
            if (!IsDashing)
            {
                IsDashing = true;
                Reposition();
                Invoke(nameof(SetDashingToFalse), DashReloadTime / Time.timeScale);
            }
        }

        if (Input.GetKeyDown(KeyCode.E) && CheckMana())
        {
            if (!InSlowMo)
            {
                TriggerSlowMo();
                ManaRechargePause = true;
                if (!ManaRecharging)
                    StartCoroutine(StartManaRecharge());

                Invoke(nameof(SetInSlowMoToFalse), SlowReloadTime / Time.timeScale);
            }
            else if (InSlowMo)
            {
                SetInSlowMoToFalse();
            }
        }

        if (Input.GetKeyDown(KeyCode.Z) && CheckMana())
        {
            if (CanCall)
            {
                CanCall = false;
                TriggerAggroCall();
                Invoke(nameof(SetCanCallToTrue), AggroReloadTime);
            }
        }
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

    private void RotateModelToOrientation()
    {
        if (playerObj.forward != orientation.forward)
        {
            angle = Vector3.SignedAngle(playerObj.forward, orientation.forward, Vector3.up);
            playerObj.Rotate(Vector3.up, angle * Time.deltaTime * 5f/Time.timeScale);
        }
        else
        {
            angle = 0;
        }
    }

    private void ControlDrag()
    {
        if (GroundCheck())
        {
            rb.drag = groundDrag;
        }
        else
        {
            if (rb.velocity.normalized.y < fallThreshold)
            {
                rb.drag = fallingDrag;
            }
            else
            {
                rb.drag = airDrag;
            }
        }
    }



    private void Move()
    {
        translateModifier = 1f;
        translateModifier = SprintCheck() ? sprintMult / moveMult : 1f;
        translateModifier = GroundCheck()? translateModifier : airMoveMult;
        if (!onSlope)
        {
            rb.AddForce(translateModifier * moveMult * translateVector.normalized/Time.timeScale, ForceMode.Acceleration);
        }
        else
        {
            rb.AddForce(translateModifier * moveMult * slopeTranslateDirection.normalized/Time.timeScale, ForceMode.Acceleration);
        }
    }

    

    private void TriggerAggroCall()
    {
        aggroArr = Physics.OverlapSphere(transform.position, AggroRange, enemyLayer);
        foreach(Collider col in aggroArr)
        {
            gotEnemy = col.TryGetComponent(out enemyComponent);
            if(gotEnemy)
            {
                enemyComponent.Aggro(gameObject);
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

    private void TriggerSlowMo()
    {
        CurrentMana -= ManaCost;
        InSlowMo = true;
        ManaRechargePause = true;
        if (!ManaRecharging)
            StartCoroutine(StartManaRecharge());
        Time.timeScale *= SlowMult;
        Invoke(nameof(SetInSlowMoToFalse), SlowReloadTime);
    }

    private void SetDashingToFalse()
    {
        IsDashing = false;
    }

    private void Reposition()
    {
        CurrentMana -= ManaCost;
        ManaRechargePause = true;
        if (!ManaRecharging)
            StartCoroutine(StartManaRecharge());
        Vector3 ShootVector = playerCam.forward;
        rb.AddForce(ShootVector * DashForce, ForceMode.Impulse);
    }

    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    #region Physical Checks

    public Vector3 GetVelocity()
    {
        return rb.velocity;
    }
    private bool SlopeCheck()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, (playerHeight / 2) + 1.5f))
        {
            if (slopeHit.normal != Vector3.up)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }
    private bool SprintCheck()
    {
        if (Input.GetKey(KeyCode.LeftShift)&&CheckSprint())
        {
            CurrentSprint -= Time.deltaTime * SprintDecayRate;
            SprintRechargePause = true;
            return true;
            
        }
        else
        {
            if(CurrentSprint<MaxSprint)
            {
                if(!SprintRecharging)
                {
                    StartCoroutine(StartSprintRecharge());
                }    
            }
            return false;
        }
    }

    private bool GroundCheck()
    {
        return Physics.CheckSphere(transform.position - new Vector3(0, playerHeight / 2, 0), 0.4f, groundLayer);
    }
    #endregion

    #region Debugs
    private void OnDrawGizmosSelected()
    {
    }
    #endregion
}
