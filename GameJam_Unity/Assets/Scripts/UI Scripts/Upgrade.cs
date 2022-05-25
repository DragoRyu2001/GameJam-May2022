using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Upgrade : MonoBehaviour
{
    [Header("General")]
    [ReadOnly, SerializeField] bool canShop;
    [SerializeField] bool vampPhase;
    [SerializeField] PlayerScript player;

    [Header("UI")]
    [SerializeField] GameObject upgradeMenu;
    [SerializeField] TMP_Text infoText;

    [Header("ReadOnly")]
    [ReadOnly, SerializeField] int manaLevel, healthLevel, sprintLevel;//Base Stats
    [ReadOnly, SerializeField] int aggroLevel, slowLevel, dashLevel;//Ability Stats
    
    //Base Stats
    [ReadOnly, SerializeField] float manaIncrease, manaRegenIncrease, manaCostDecrease;
    [ReadOnly, SerializeField] float healthIncrease, healthRegenIncrease, healthPauseTimeDecrease;
    [ReadOnly, SerializeField] float sprintIncrease, sprintRegenRateIncrease, sprintDecayRateDecrease;
    
    //Ability Stats
    [ReadOnly, SerializeField] float aggroRange, aggroReload;
    [ReadOnly, SerializeField] float dashForce, dashReload;
    [ReadOnly, SerializeField] float slowDuration, slowReload;

    void OnTriggerEnter(Collider col)
    {
        if(col.transform.tag=="Player")
            canShop = true;
    }
    void OnTriggerExit(Collider col)
    {
        if(col.transform.tag == "Player")
            canShop = false;
    }
    void Start()
    {
        Initialise();
        
        
    }
    void Initialise()
    {
        //SetLevels to 0
        manaLevel = 0;
        healthLevel = 0;
        sprintLevel = 0;
        aggroLevel = 0;
        slowLevel = 0;
        dashLevel = 0;
        //Base Stats Initialised=====================================
        //Set Mana values
        manaIncrease = 0.1f*player.MaxMana;
        manaRegenIncrease = 0.1f*player.ManaRegenRate;
        manaCostDecrease = 0.1f*player.ManaCost;
        //Set Health Values
        healthIncrease = 0.1f*player.MaxHealth;
        healthPauseTimeDecrease = 0.1f*player.HealthRechargePauseTime;
        healthRegenIncrease = 0.1f*player.HealthRegenRate;

        //Set Sprint values
        sprintIncrease = 0.1f*player.MaxSprint;
        sprintDecayRateDecrease = 0.1f*player.SprintDecayRate;
        sprintRegenRateIncrease = 0.1f*player.SprintRegenRate;
        //=============================================================
        //Ability Stats Initialised====================================
        //Aggro values
        aggroRange = 0.1f*player.AggroRange;
        aggroReload = 0.1f*player.AggroReloadTime;
        //Dash values
        dashForce = 0.1f*player.DashForce;
        dashReload = 0.1f*player.DashReloadTime;
        //Slow values
        slowDuration = 0.1f*player.SlowTime;
        slowReload = 0.1f*player.SlowReloadTime;
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F) && canShop)
        {
            Debug.Log("Shop Open");
            //Open Shop
            Cursor.lockState = CursorLockMode.None;
            upgradeMenu.SetActive(true);
        }
    }
    #region Base Stats Upgrade
    public void ManaUpgrade()
    {
        manaLevel++;
        switch(manaLevel)
        {
            //Special upgrades
            case 3:
            case 6:
            case 9:
            case 12:
            case 15:
                //Upgrade things for level 5
                player.ManaRegenRate+=manaRegenIncrease;
                player.ManaCost-=manaCostDecrease;
                player.MaxMana+=manaIncrease;
                break;
            default:
                // Increase Base Mana Level
                player.MaxMana+=manaIncrease;
                break;
        }
    }
    public void HealthUpgrade()
    {
        healthLevel++;
        switch(healthLevel)
        {
            case 3:
            case 6:
            case 9:
            case 12:
            case 15:
                //Upgrade things for level 5
                player.MaxHealth+=healthIncrease;
                player.HealthRechargePauseTime -= healthPauseTimeDecrease;
                player.HealthRegenRate += healthRegenIncrease;
                break;
            default:
                // Increase Base Health Level
                player.MaxHealth+=healthIncrease;
                break;
        }
    }
    public void SprintUpgrade()
    {
        sprintLevel++;
        switch(sprintLevel)
        {
            case 3:
            case 6:
            case 9:
            case 12:
            case 15:
                //Upgrade things for level 5
                player.MaxSprint+=sprintIncrease;
                player.SprintDecayRate-=sprintDecayRateDecrease;
                player.SprintRegenRate+=sprintRegenRateIncrease;
                break;
            default:
                // Increase Base Sprint Level
                player.MaxSprint+=sprintIncrease;
                break;
        }
    }

    public void ManaShowUpgrade()
    {
        string str;
        switch(manaLevel+1)
        {
            case 3:
            case 6:
            case 9:
            case 12:
            case 15:
                //Upgrade things for level 5
                str = " Mana: "+player.MaxMana+" + "+manaIncrease+"\n Mana Cost: "+player.ManaCost+" - "+manaCostDecrease + "\n Mana Regen Rate: "+player.ManaRegenRate+ " + " + manaRegenIncrease;
                break;
            default:
                // Increase Base Mana Level           
                str = " Mana: "+player.MaxMana+" + "+manaIncrease;
                break;
        }
        infoText.text = str;
    }
    public void HealthShowUpgrade()
    {
        string str;
        switch(healthLevel+1)
        {
            case 3:
            case 6:
            case 9:
            case 12:
            case 15:
                //Upgrade things for level 5
                str = " Health: "+player.MaxHealth+" + "+healthIncrease+"\n Health Regen Start Time: "+player.HealthRechargePauseTime+" - "+healthPauseTimeDecrease+"\n Health Regen Rate: "+player.HealthRegenRate+" + "+healthRegenIncrease;
                break;
            default:
                // Increase Base Mana Level           
                str = " Health: "+player.MaxHealth+" + "+healthIncrease;
                break;
        }
        infoText.text = str;
    }
    public void SprintShowUpgrade()
    {
        string str;
        switch(healthLevel+1)
        {
            case 3:
            case 6:
            case 9:
            case 12:
            case 15:
                //Upgrade things for level 5
                str = " Sprint: "+player.MaxSprint + " + " + sprintIncrease+"\n Sprint Decay Rate: " + player.SprintDecayRate+" - "+sprintDecayRateDecrease+"\n Sprint Regen Rate: "+ player.SprintRegenRate+" + "+sprintRegenRateIncrease;
                break;
            default:
                // Increase Base Mana Level           
                str = " Sprint: "+player.MaxSprint + " + " + sprintIncrease;
                break;
        }
        infoText.text = str;
    }

    #endregion


    #region Ability Stats Upgrade
    public void SlowUpgrade()
    {
        slowLevel++;
        if(slowLevel<=5)
        {
            player.SlowTime+=slowDuration;
            player.SlowReloadTime-=slowReload;
        }
        else
        {
            Debug.Log("You can't Upgrade this anymore");
        }
    }
    public void DashUpgrade()
    {
        dashLevel++;
        if(dashLevel<=5)
        {
            player.DashForce+=dashForce;
            player.DashReloadTime-=dashReload;
        }
        else
        {
            Debug.Log("You can't Upgrade this anymore");
        }
    }
    public void AggroUpgrade()
    {
        aggroLevel++;
        if(aggroLevel<=5)
        {
            player.AggroRange+=aggroRange;
            player.AggroReloadTime-=aggroReload;
        }
        else
        {
            Debug.Log("You can't Upgrade this anymore");
        }
    }
    public void SlowShowUpgrade()
    {
        string str;
        if((slowLevel+1)<=5)
        {
            str = " Slow Duration: "+player.SlowTime+" + "+slowDuration+"\n Slow Cooldown: "+player.SlowReloadTime+" - "+slowReload;
        }
        else
            str = " No Upgrades Available";
        infoText.text = str;
    }
    public void AggroShowUpgrade()
    {
        string str;
        if((aggroLevel+1)<=5)
        {
            str = " Aggro Range: "+player.AggroRange+" + "+aggroRange+"\n Aggro Cooldown: "+player.AggroReloadTime+" - "+aggroReload;
        }
        else
            str = " No Upgrades Available";
        infoText.text = str;
    }
    public void DashShowUpgrade()
    {
        string str;
        if((dashLevel+1)<=5)
        {
            str = " Dash Force: "+player.DashForce+" + "+dashForce+"\n Dash Cooldown: "+player.DashReloadTime+" - "+dashReload;
        }
        else
            str = " No Upgrades Available";
        infoText.text = str;
    }

    #endregion

    public void ResetInfo()
    {
        infoText.text = "";
    }
    
}
