using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementScript : MonoBehaviour
{

    [Header("Movement variables")]
    [SerializeField] float moveMult;
    [SerializeField] float sprintMult;
    [Range(0.1f, 0.5f)]
    [SerializeField] float airMoveMult;
    [SerializeField] Transform orientation;
    RaycastHit slopeHit;

    [Header("Jump/Drag variables")]
    [SerializeField] float jumpForce;
    [SerializeField] float groundDrag;
    [SerializeField] float airDrag;

    [Header("Misc References")]
    [SerializeField] Rigidbody rb;
    [SerializeField] Transform playerCam;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Collider coll;

    private float moveX;
    private float moveY;
    private float playerHeight;

    bool sprinting, onSlope, onGround;

    Vector3 translateVector;
    Vector3 slopeTranslateDirection;
    float translateModifier;

    // Start is called before the first frame update
    void Start()
    {
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
    }



    private void ControlDrag()
    {
        if (GroundCheck())
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = airDrag;
        }
    }

    private void FixedUpdate()
    {
        Move();
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

        //layermask 6 is currently ground
    }
    #endregion
}
