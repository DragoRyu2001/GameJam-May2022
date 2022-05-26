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
    [SerializeField] GunGeneral gun;
    [SerializeField] int souls, costSouls, abilitySoulMult, gunSoulMult;
    [Header("GUNNSSSSSS")]
    [SerializeField] Rifle rifle;
    [SerializeField] Shotgun shotgun;
    [SerializeField] Crossbow crossbow;
    [SerializeField] AutoCrossbow autoCrossbow;
    [SerializeField] TripleCrossbow tripleCrossbow;

    [Header("UI")]
    [SerializeField] GameObject upgradeMenu;
    [SerializeField] GameObject openCloseText;
    [SerializeField] TMP_Text basicInfoText;
    [SerializeField] TMP_Text abilityInfoText;
    [SerializeField] TMP_Text gunInfoText;

    [SerializeField] TMP_Text soulsText;

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

    //Gun Stats
    [ReadOnly, SerializeField] int crossbowLevel, rifleLevel, shotgunLevel;
    [ReadOnly, SerializeField] float crossBowReloadDecrease, rifleReloadDecrease, shotgunReloadDecrease;
    [ReadOnly, SerializeField] float rifleDamageIncrease, shotGunDamageIncrease;


    void OnTriggerEnter(Collider col)
    {
        if(col.transform.tag=="Player")
        {
            canShop = true;
            openCloseText.SetActive(true);
        }
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
        UpdateSouls();//Initialise Souls value
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
        //=============================================================
        //Guns Stats Initialised=======================================
        rifleReloadDecrease = 0.2f*rifle.ReloadTime;
        crossBowReloadDecrease = 0.2f*rifle.ReloadTime;
        rifleDamageIncrease = 0.2f*rifle.Damage;
        shotGunDamageIncrease = 0.2f*shotgun.Damage;
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F) && canShop)
        {
            Debug.Log("Shop Open");
            //Open Shop
            upgradeMenu.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
        }
    }
    void UpdateSouls()
    {
        soulsText.text = souls.ToString();
    }
    #region Base Stats Upgrade
    public void ManaUpgrade()
    {
        manaLevel++;
        if(souls-(costSouls*manaLevel)>0)
        {
            souls-=(costSouls*manaLevel);
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
            ManaShowUpgrade();
            UpdateSouls();
        }
        else
            basicInfoText.text="Don't have enough funds";
        
    }
    public void HealthUpgrade()
    {
        healthLevel++;
        if(souls-(costSouls*healthLevel)>0)
        {
            souls-=(costSouls*healthLevel);
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
            HealthShowUpgrade();
            UpdateSouls();
        }
        else
            basicInfoText.text="Don't have enough funds";
    }
    public void SprintUpgrade()
    {
        sprintLevel++;
        if(souls-(costSouls*sprintLevel)>0)
        {
            souls-=(costSouls*sprintLevel);
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
            SprintShowUpgrade();
            UpdateSouls();
        }
        else
            basicInfoText.text="Don't have enough funds";
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
                str = "Cost: "+ ((manaLevel+1)*costSouls)+"\n Mana: "+player.MaxMana+" + "+manaIncrease+"\n Mana Cost: "+player.ManaCost+" - "+manaCostDecrease + "\n Mana Regen Rate: "+player.ManaRegenRate+ " + " + manaRegenIncrease;
                break;
            default:
                // Increase Base Mana Level           
                str = "Cost: "+ ((manaLevel+1)*costSouls)+"\n Mana: "+player.MaxMana+" + "+manaIncrease;
                break;
        }
        basicInfoText.text = str;
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
                str = "Cost: "+ ((healthLevel+1)*costSouls)+"\n Health: "+player.MaxHealth+" + "+healthIncrease+"\n Health Regen Start Time: "+player.HealthRechargePauseTime+" - "+healthPauseTimeDecrease+"\n Health Regen Rate: "+player.HealthRegenRate+" + "+healthRegenIncrease;
                break;
            default:
                // Increase Base Mana Level           
                str = "Cost: "+ ((healthLevel+1)*costSouls)+"\n Health: "+player.MaxHealth+" + "+healthIncrease;
                break;
        }
        basicInfoText.text = str;
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
                str = "Cost: "+ ((sprintLevel+1)*costSouls)+"\n Sprint: "+player.MaxSprint + " + " + sprintIncrease+"\n Sprint Decay Rate: " + player.SprintDecayRate+" - "+sprintDecayRateDecrease+"\n Sprint Regen Rate: "+ player.SprintRegenRate+" + "+sprintRegenRateIncrease;
                break;
            default:
                // Increase Base Mana Level           
                str = "Cost: "+ ((sprintLevel+1)*costSouls)+"\n Sprint: "+player.MaxSprint + " + " + sprintIncrease;
                break;
        }
        basicInfoText.text = str;
    }

    #endregion


    #region Ability Stats Upgrade
    public void SlowUpgrade()
    {
        slowLevel++;
        if(slowLevel<=5)
        {
            if(souls-(slowLevel*costSouls*abilitySoulMult)>0)
            {
                souls-=(slowLevel*costSouls*abilitySoulMult);
                player.SlowTime+=slowDuration;
                player.SlowReloadTime-=slowReload;    
                UpdateSouls();  
                SlowShowUpgrade();
            }
            else
                abilityInfoText.text = "Don't have enough funds";
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
            if(souls-(dashLevel*costSouls*abilitySoulMult)>0)
            {
                souls-=(dashLevel*costSouls*abilitySoulMult);
                player.DashForce+=dashForce;
                player.DashReloadTime-=dashReload;
                UpdateSouls();
                DashShowUpgrade();
            }
            else
                abilityInfoText.text = "Don't have enough funds";
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
            if(souls-(aggroLevel*costSouls*abilitySoulMult)>0)
            {
                souls-=(aggroLevel*costSouls*abilitySoulMult);
                player.AggroRange+=aggroRange;
                player.AggroReloadTime-=aggroReload;
                AggroShowUpgrade();
                UpdateSouls();
            }
            else
                abilityInfoText.text = "Don't have enough funds";
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
            str = "Cost: "+((slowLevel+1)*costSouls*abilitySoulMult)+"\n Slow Duration: "+player.SlowTime+" + "+slowDuration+"\n Slow Cooldown: "+player.SlowReloadTime+" - "+slowReload;
        }
        else
            str = " No Upgrades Available";
        abilityInfoText.text = str;
    }
    public void AggroShowUpgrade()
    {
        string str;
        if((aggroLevel+1)<=5)
        {
            str = "Cost: "+((aggroLevel+1)*costSouls*abilitySoulMult)+"\n Aggro Range: "+player.AggroRange+" + "+aggroRange+"\n Aggro Cooldown: "+player.AggroReloadTime+" - "+aggroReload;
        }
        else
            str = " No Upgrades Available";
        abilityInfoText.text = str;
    }
    public void DashShowUpgrade()
    {
        string str;
        if((dashLevel+1)<=5)
        {
            str = "Cost: "+((dashLevel+1)*costSouls*abilitySoulMult)+"\n Dash Force: "+player.DashForce+" + "+dashForce+"\n Dash Cooldown: "+player.DashReloadTime+" - "+dashReload;
        }
        else
            str = " No Upgrades Available";
        abilityInfoText.text = str;
        Debug.Log("DASH SLOW UPGRADE!!!");
    }

    #endregion

    #region GUNNSS Stats Upgrade
    //Lvl 1, Lvl2, Lvl 3
    public void UpgradeCrossBow()
    {
        //Base: Damage = 75 DPS->0.7
        //Level 1: Prefab different arrow= Damage = 100 DPS->0.6        (Single)
        //Level 2: Model change, Prefab different arrow= +20%           (Triple)
        //Level 3: Model change, Prefab different arrow= +20% 22 per shot (Auto)
        crossbowLevel++;
        switch(crossbowLevel)
        {
            case 1:
                //Method(int)
                //player.enum = State
                //player.setCrossBow(GameObject)
                break;
            case 2:

                break;
            case 3:
                
                break;
            default:
                //can not upgrade
                break;
        }
    }
    public void UpgradeRifle()
    {
        
        if(rifleLevel<5)
        {
            rifleLevel++;
            int cost = rifleLevel*costSouls*gunSoulMult;
            if(souls-cost>=0)
            {
                souls-=cost;
                
                rifle.ReloadTime-=rifleReloadDecrease;
                rifle.Damage+=rifleDamageIncrease;
                UpdateSouls();
            }
            else
            {
                gunInfoText.text="Don't have enough funds";
            }
            
        }
        else
        {
            gunInfoText.text = "No Upgrades Available";
        }
        
    }
    public void UpgradeShotGun()
    {
        if(shotgunLevel<5)
        {
            shotgunLevel++;
            int cost = shotgunLevel*costSouls*gunSoulMult;
            if(souls-cost>=0)
            {
                souls-=cost;
                shotgun.ReloadTime-=shotgunReloadDecrease;
                shotgun.Damage+=shotGunDamageIncrease;
                UpdateSouls();
            }
            else
            {
                gunInfoText.text="Don't have enough funds";
            }

        }
        else
        {
            gunInfoText.text = "No Upgrades Available";
        }

    }

    #endregion
    
    
    public void ResetInfo()
    {
        basicInfoText.text = "";
    }
    
}
