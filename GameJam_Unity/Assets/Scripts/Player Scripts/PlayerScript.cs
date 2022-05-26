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
    [SerializeField] Rigidbody rb;
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
        
        ControlDrag();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        playerHeight = coll.bounds.size.y;
    }

    void Update()
    {
        GroundCheck();
        SprintCheck();
        onSlope = SlopeCheck();
        ControlDrag();
        ReadInput();
        slopeTranslateDirection = Vector3.ProjectOnPlane(translateVector, slopeHit.normal);
        RotateModelToOrientation();

    }


    //don't use this, use event based regen
    private void StatRegen()
    {
        if(CurrentHealth<MaxHealth)
        {
            StartCoroutine(StartHealthRecharge());
        }
        if(CurrentMana<MaxMana)
        {
            StartCoroutine(StartManaRecharge());
        }
        if(CurrentSprint<MaxSprint)
        {
            StartCoroutine(StartSprintRecharge());
        }
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

    private void FixedUpdate()
    {
        Move();
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

        if(Input.GetKeyDown(KeyCode.Q)&&CheckMana())
        {
            if (!IsDashing)
            {
                IsDashing = true;
                Reposition();
                Invoke(nameof(SetDashingToFalse), DashReloadTime/Time.timeScale);
            }
        }

        if(Input.GetKeyDown(KeyCode.E)&&CheckMana())
        {
            if(!InSlowMo)
            {
                TriggerSlowMo();
                ManaRechargePause = true;
                if (!ManaRecharging)
                    StartCoroutine(StartManaRecharge());

                Invoke(nameof(SetInSlowMoToFalse), SlowReloadTime/Time.timeScale);
            }
            else if(InSlowMo)
            {
                SetInSlowMoToFalse();
            }
        }

        if (Input.GetKeyDown(KeyCode.Z) && CheckMana())
        {
            if(CanCall)
            {
                CanCall = false;
                TriggerAggroCall();
                Invoke(nameof(SetCanCallToTrue), AggroReloadTime);
            }
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
