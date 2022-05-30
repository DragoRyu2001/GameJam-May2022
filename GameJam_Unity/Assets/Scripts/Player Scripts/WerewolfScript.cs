using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WerewolfScript : BasePlayerClass
{
    [Header("Misc References")]
    [SerializeField] PlayerScript playerScript;

    [Header("UI Tie-ins")]
    [SerializeField] GameObject wereWolfUIPanel;
    [SerializeField] Material healthMat;
    [SerializeField] Image swipeOutline;
    [SerializeField] Image poundOutline;
    [SerializeField] Image ultOutline;

    [Header("Ultimate Parameters")]
    [SerializeField, ReadOnly] private bool canUlt;
    [SerializeField, ReadOnly] private bool isUlting;
    [SerializeField] private float ultDuration;
    [SerializeField, ReadOnly] private float currentUltDuration;
    [SerializeField, ReadOnly] private float currentSwipeDuration;
    [SerializeField, ReadOnly] private float currentPoundDuration;
    [SerializeField] Collider leftHandCollider;
    [SerializeField] Collider rightHandCollider;
    [SerializeField] float poundRange;
    [SerializeField] float poundForce;

    private float desiredWalkAnimSpeed, currentWalkAnimSpeed;
    private float moveX;
    private float moveY;

    private Collider[] enemyCol;
    private Enemy enemyComp;

    [SerializeField, ReadOnly] private bool canMove;

    private void OnEnable()
    {
        StartCoroutine(ReturnToHumanForm());
        AssertWerewolfStatus();
    }

    void Start()
    {
        canMove = false;
        Invoke(nameof(SetCanMoveToTrue), 2.45f);
        rb.velocity = Vector3.zero;
        BaseParametersUpdate();
        AssertWerewolfStatus();
        StartCoroutine(ReturnToHumanForm());
        wereWolfUIPanel.SetActive(true);
        currentUltDuration = ultDuration + 2.5f;
        leftHandCollider.enabled = false;
        rightHandCollider.enabled = false;
        currentWalkAnimSpeed = 1f;
        desiredWalkAnimSpeed = 1f;
        ControlDrag();
        playerHeight = otherCollider.bounds.size.y;
        healthMat.SetFloat("_Health", CurrentHealth / MaxHealth);
    }

    private void AssertWerewolfStatus()
    {
        servantModel.SetActive(false);
        werewolfModel.SetActive(true);

        coll.enabled = true;
        otherCollider.enabled = false;

        playerScript.enabled = false;
        enabled = true;
    }

    private IEnumerator AttackAction1(float time)
    {
        anim.SetBool("canAttack", true);
        ToggleHandColliders(true);
        currentSwipeDuration = 0f;
        while(currentSwipeDuration<time)
        {
            yield return null;
            currentSwipeDuration += Time.deltaTime;
            swipeOutline.fillAmount = 1f - currentSwipeDuration / time;
        }
        ToggleHandColliders(false);
        anim.SetBool("canAttack", false);
    } 

    private IEnumerator AttackAction2(float time)
    {
        ToggleHandColliders(true);
        canMove = false;
        currentPoundDuration = 0f;
        while (currentSwipeDuration < time)
        {
            yield return null;
            currentPoundDuration += Time.deltaTime;
            poundOutline.fillAmount = 1f - currentPoundDuration / time;
        }
        ToggleHandColliders(false);

    }

    private void ToggleHandColliders(bool state)
    {
        rightHandCollider.enabled = state;
        leftHandCollider.enabled = state;
    }

    private void FixedUpdate()
    {
        Move();
    }

    void SetCanMoveToTrue()
    {
        canMove = true;
    }

    void Update()
    {
        if (IsAlive)
        {
            currentUltDuration -= Time.deltaTime;
            ultOutline.fillAmount = 1 - currentUltDuration / ultDuration;
            desiredWalkAnimSpeed = rb.velocity.magnitude < 2f ? 1f : rb.velocity.magnitude / 6f;
            anim.SetFloat("Walk", Mathf.Lerp(currentWalkAnimSpeed, desiredWalkAnimSpeed, 500f * Time.deltaTime));
            GroundCheck();
            onSlope = SlopeCheck();
            ControlDrag();
            ReadInput();
            slopeTranslateDirection = Vector3.ProjectOnPlane(translateVector, slopeHit.normal);
            RotateModelToOrientation();
        }
    }

    protected void Move()
    {
        translateModifier = 1f;
        translateModifier = GroundCheck() ? translateModifier : airMoveMult;
        translateModifier = canMove ? translateModifier : 0f;
        if (!onSlope)
        {
            rb.AddForce(translateModifier * moveMult * translateVector.normalized / Time.timeScale, ForceMode.Acceleration);
        }
        else
        {
            rb.AddForce(translateModifier * moveMult * slopeTranslateDirection.normalized / Time.timeScale, ForceMode.Acceleration);
        }
    }

    public void TakeDamage(float damage)
    {
        CurrentHealth -= damage;
        IsAlive = CheckHealth();

        if (!IsAlive)
        {
            anim.SetTrigger("Death");
            rb.velocity = Vector3.zero;
            canMove = false;
            StopCoroutine(ReturnToHumanForm());
            StartCoroutine(TurnToServant());
            Death();
        }

    }

    private void ReadInput()
    {
        //keyboard input
        moveX = Input.GetAxisRaw("Horizontal");
        moveY = Input.GetAxisRaw("Vertical");
        translateVector = -orientation.forward * moveY - orientation.right * moveX;
        if (Input.GetKeyDown(KeyCode.X))
        {
            StartCoroutine(ReturnToHumanForm());
        }

        if (Input.GetMouseButtonDown(0))
        {

            StartCoroutine(AttackAction1(2.46f));
            anim.SetTrigger("Attack1");
            
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            TakeDamage(201);
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (!GroundCheck())
            {
                rb.AddForce(Vector3.down * poundForce, ForceMode.Impulse);
            }

            StartCoroutine(AttackAction2(1.42f));
            enemyCol = Physics.OverlapSphere(transform.position, poundRange, enemyLayer);
            StartCoroutine(AttackGround());
            anim.SetTrigger("Attack2");
        }
    }

    private IEnumerator AttackGround()
    {
        yield return new WaitForSeconds(0.75f);
        foreach (Collider col in enemyCol)
        {
            bool success = col.TryGetComponent<Enemy>(out enemyComp);
            if (success)
            {
                enemyComp.TakeDamage(1000, true);
            }
        }
        canMove = true;
    }

    private IEnumerator ReturnToHumanForm()
    {
        yield return new WaitForSeconds(2.5f);
        canMove = true;
        //start anim wait
        yield return new WaitForSeconds(ultDuration);
        //ult duration wait;
        rb.velocity = Vector3.zero;
        canMove = false;
        yield return StartCoroutine(TurnToServant());

    }

    private IEnumerator TurnToServant()
    {
        anim.StopPlayback();
        anim.SetTrigger(IsAlive ? "Human" : "Death");
        yield return new WaitForSeconds(2.5f);
        rb.AddForce(Vector3.up * 2f, ForceMode.Impulse);
        wereWolfUIPanel.SetActive(false);
        servantModel.SetActive(true);
        werewolfModel.SetActive(false);

        coll.enabled = true;
        otherCollider.enabled = false;

        playerScript.enabled = true;
        enabled = false;
    }
}
