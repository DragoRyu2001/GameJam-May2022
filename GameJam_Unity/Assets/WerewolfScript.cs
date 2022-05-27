using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WerewolfScript : MonoBehaviour
{
    [Header("Movement variables")]
    [SerializeField] float moveMult;
    [SerializeField, Range(0.1f, 0.5f)] float airMoveMult;
    [SerializeField] Transform orientation;
    RaycastHit slopeHit;

    [Header("Jump/Drag variables")]
    [SerializeField] float groundDrag;
    [SerializeField] float airDrag;
    [SerializeField, Range(0.01f, 0.3f)] float fallingDrag;
    [SerializeField, Range(-0.8f, -0.1f)] float fallThreshold;

    [Header("Misc References")]
    [SerializeField] Transform playerCam;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] Collider coll;
    [SerializeField] Collider wereWolfCollider;
    [SerializeField] Transform playerObj;
    [SerializeField] GameObject servantModel;
    [SerializeField] GameObject werewolfModel;
    [SerializeField] PlayerScript playerScript;
    [SerializeField] Rigidbody rb;

    private float moveX;
    private float moveY;
    private float angle;
    private float playerHeight;
    bool onSlope;
    Vector3 translateVector;
    Vector3 slopeTranslateDirection;
    [Header("Translate Modifier")]
    [SerializeField, ReadOnly] float translateModifier;
    private Vector3 rotateVector;


    void Start()
    {
        ControlDrag();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        playerHeight = wereWolfCollider.bounds.size.y;
    }

    private void FixedUpdate()
    {
        Move();
    }

    void Update()
    {
        GroundCheck();
        onSlope = SlopeCheck();
        ControlDrag();
        ReadInput();
        slopeTranslateDirection = Vector3.ProjectOnPlane(translateVector, slopeHit.normal);
        RotateModelToOrientation();
    }

    private void ReadInput()
    {
        //keyboard input
        moveX = Input.GetAxisRaw("Horizontal");
        moveY = Input.GetAxisRaw("Vertical");
        translateVector = -orientation.forward * moveY - orientation.right * moveX;

        if (Input.GetKeyDown(KeyCode.Z))
        {
            servantModel.SetActive(true);
            werewolfModel.SetActive(false);

            coll.enabled = true;
            wereWolfCollider.enabled = false;

            playerScript.enabled = true;
            enabled = false;
        }
    }

    private void RotateModelToOrientation()
    {
        if (playerObj.forward != orientation.forward)
        {
            angle = Vector3.SignedAngle(playerObj.forward, orientation.forward, Vector3.up);
            playerObj.Rotate(Vector3.up, angle * Time.deltaTime * 5f / Time.timeScale);
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

    private bool GroundCheck()
    {
        return Physics.CheckSphere(transform.position - new Vector3(0, playerHeight / 2, 0), 0.4f, groundLayer);
    }

    // Update is called once per frame

}
