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
    [SerializeField] Collider coll;
    [SerializeField] Transform playerObj;

    private float moveX;
    private float moveY;
    private float angle;
    private float playerHeight;

    bool sprinting, onSlope, onGround, isDashing;

    [Header("Reposition Parameters")]
    [SerializeField] float dashForce;

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
        onGround = GroundCheck();
        sprinting = SprintCheck();
        onSlope = SlopeCheck();
        ControlDrag();
        ReadInput();

        slopeTranslateDirection = Vector3.ProjectOnPlane(translateVector, slopeHit.normal);
        if (playerObj.forward != orientation.forward)
        {
            angle = Vector3.SignedAngle(playerObj.forward, orientation.forward, Vector3.up);
            playerObj.Rotate(Vector3.up, angle * Time.deltaTime*5f);
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

    private void LateUpdate()
    {
        
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
        translateModifier = sprinting ? sprintMult / moveMult : 1f;
        translateModifier = onGround ? translateModifier : airMoveMult;
        if (!onSlope)
        {
            rb.AddForce(translateModifier * moveMult * translateVector.normalized, ForceMode.Acceleration);
        }
        else
        {
            rb.AddForce(translateModifier * moveMult * slopeTranslateDirection.normalized, ForceMode.Acceleration);
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
            if (!isDashing)
            {
                rb.useGravity = false;
                isDashing = true;
                Reposition();
                Invoke(nameof(SetDashingToFalse), 0.9f);
            }
        }

        if(Input.GetKeyDown(KeyCode.E)&&CheckMana())
        {

        }
    }

    private void SetDashingToFalse()
    {
        isDashing = false;
        rb.useGravity = true;
    }

    private void Reposition()
    {
        currentMana -= manaCost;
        manaRechargePause = true;
        if (!manaRecharging)
            StartCoroutine(StartManaRecharge());

        Vector3 ShootVector = playerCam.forward;
        rb.AddForce(ShootVector * dashForce, ForceMode.Impulse);
    }

    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void OnDrawGizmos()
    {
     
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
        if (Input.GetKey(KeyCode.LeftShift))
        {
            return true;
        }
        else
            return false;
    }

    private bool GroundCheck()
    {
        return Physics.CheckSphere(transform.position - new Vector3(0, playerHeight / 2, 0), 0.4f, groundLayer);
    }
    #endregion

    #region Debugs
    private void OnDrawGizmosSelected()
    {
        Debug.DrawRay(transform.position, playerObj.forward*20f, Color.green);
        Debug.DrawRay(transform.position, orientation.forward*20f, Color.red);
    }
    #endregion
}
