using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BasePlayerClass : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("UI")]
    Image healthBar;
    [Header("Health")]
    [SerializeField, ReadOnly] private bool critical;
    [SerializeField] private float maxHealth;
    [SerializeField, ReadOnly] private float currentHealth;
    [SerializeField] private float healthRegenRate;
    [SerializeField] private bool healthRecharging;
    [SerializeField] private bool healthRechargePause;
    [SerializeField, Range(2f, 3.5f)] private float healthRechargePauseTime;



    [Header("Jump/Drag variables")]
    [SerializeField] protected float groundDrag;
    [SerializeField] protected float airDrag;
    [SerializeField, Range(0.01f, 0.3f)] protected float fallingDrag;
    [SerializeField, Range(-0.8f, -0.1f)] protected float fallThreshold;

    [Header("Statuses")]
    [SerializeField, ReadOnly] private bool isAlive;
    [SerializeField, ReadOnly] private bool isRewinding;
    [SerializeField, ReadOnly] private bool inBerserk;
    [SerializeField, ReadOnly] private bool canCast;

    [Header("Misc References")]
    [SerializeField] protected Rigidbody rb;
    [SerializeField] protected Transform playerObj;
    [SerializeField] protected Animator anim;
    [SerializeField] protected Transform playerCam;
    [SerializeField] protected LayerMask groundLayer;
    [SerializeField] protected LayerMask enemyLayer;

    [Header("Models")]
    [SerializeField] protected GameObject servantModel;
    [SerializeField] protected GameObject werewolfModel;

    [Header("Collider Gameobjects")]
    [SerializeField] protected Collider coll;
    [SerializeField] protected Collider otherCollider;

    [Header("Movement variables")]
    [SerializeField] protected float moveMult;
    [SerializeField, Range(0.1f, 0.5f)] protected float airMoveMult;

    [SerializeField] protected Transform orientation;

    [Header("Translate Modifier")]
    [SerializeField, ReadOnly] protected float translateModifier;

    Vector3 a, b, newDir;

    protected Vector3 translateVector;
    protected Vector3 slopeTranslateDirection;
    protected RaycastHit slopeHit;
    protected bool onSlope;
    protected float playerHeight;

    #region Setters and Getters
    public float MaxHealth { get => maxHealth; set => maxHealth = value; }
    public float CurrentHealth { get => currentHealth; set => currentHealth = value; }
    public float HealthRegenRate { get => healthRegenRate; set => healthRegenRate = value; }
    public bool HealthRecharging { get => healthRecharging; set => healthRecharging = value; }
    public bool HealthRechargePause { get => healthRechargePause; set => healthRechargePause = value; }
    public float HealthRechargePauseTime { get => healthRechargePauseTime; set => healthRechargePauseTime = value; }
   
    public bool IsAlive { get => isAlive; set => isAlive = value; }
    public bool InBerserk { get => inBerserk; set => inBerserk = value; }
    public bool CanCast { get => canCast; set => canCast = value; }


    public bool Critical { get => critical; set => critical = value; }
    public bool IsRewinding { get => isRewinding; set => isRewinding = value; }
    #endregion

    #region Physical checks
    
    protected bool GroundCheck()
    {
        return Physics.CheckSphere(transform.position + Vector3.down, 0.4f, groundLayer);
    }

    protected bool SlopeCheck()
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

    public bool CheckHealth()
    {
        return IsAlive = CurrentHealth > 0;
    }

    #endregion


    protected void ControlDrag()
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

    protected void Death()
    {
        isAlive = false;
        anim.SetTrigger("Death");
        StopAllCoroutines();
    }

    public void BaseParametersUpdate()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        playerHeight = coll.bounds.size.y;
        CurrentHealth = MaxHealth;
        IsAlive = CheckHealth();
    }

    protected void RotateModelToOrientation()
    {
        a = new Vector3(playerObj.forward.x, 0, playerObj.forward.z);
        b = -new Vector3(orientation.forward.x, 0, orientation.forward.z);
        if (a != b)
        {
            newDir = Vector3.RotateTowards(playerObj.forward, a - b, 50f * Time.deltaTime, 0f);
            playerObj.rotation = Quaternion.LookRotation(newDir);
        }
    }

    public struct FrameStats
    {
        public Vector3 playerPos;
        public Quaternion playerRot;
        public FrameStats(Vector3 playerPos, Quaternion playerRot)
        {
            this.playerPos = playerPos;
            this.playerRot = playerRot;
        }
    }


    #region Resource Recharge Functions



    public IEnumerator StartHealthRecharge()
    {
        HealthRecharging = true;
        while (CurrentHealth < MaxHealth)
        {
            if (HealthRechargePause)
            {
                HealthRechargePause = false;
                yield return new WaitForSecondsRealtime(healthRechargePauseTime);
            }

            CurrentHealth += Time.deltaTime * HealthRegenRate/Time.timeScale;
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
    public void SetHealth()
    {
        healthBar.fillAmount = (currentHealth/maxHealth);
    }

   


    #endregion
}
