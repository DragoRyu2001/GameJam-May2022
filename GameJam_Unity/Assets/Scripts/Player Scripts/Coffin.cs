using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Coffin : MonoBehaviour
{
    [SerializeField] GameObject upgradeObj;
    [SerializeField] GameObject coffinOpen;
    [SerializeField] GameObject coffinClose;
    [SerializeField] float maxHealth;
    [ReadOnly, SerializeField] float currentHealth;

    [SerializeField] Image coffinPlayer;
    [SerializeField] Image coffinWerewolf;

    [SerializeField] Image[] coffinNailsP;
    [SerializeField] Image[] coffinNailsW;

    [Header("Read Only")]
    [SerializeField] LayerMask playerLayer;
    [ReadOnly, SerializeField] float h1, h2;
    [SerializeField, ReadOnly] bool playerInRepairRange;
    void Start()
    {
        Init();        
    }
    void Update()
    {
        if(currentHealth<=0)
        {
            GameManager.instance.GameOver();
        }
    }
    public void Init()
    {
        currentHealth = maxHealth;
        h1 = maxHealth/3;
        h2 = 2*h1;
    }

    public bool GetIfPlayerInRepairRange()
    {
        return playerInRepairRange;
    }
    public void TakeDamage(float dmg)
    {
        currentHealth=Mathf.Clamp(currentHealth-dmg, 0, maxHealth);
        if(currentHealth<h2)
        {
            coffinNailsP[0].enabled = false;
            coffinNailsW[0].enabled = false;
        }
        if(currentHealth<h1)
        {
            coffinNailsP[1].enabled = false;
            coffinNailsW[1].enabled = false;
        }
        coffinPlayer.fillAmount = currentHealth / maxHealth;
        coffinWerewolf.fillAmount = currentHealth / maxHealth;
    }
    public void HealCoffin(float health)
    {
        if (currentHealth <= h1)
        {
            currentHealth += health;
            currentHealth = Mathf.Clamp(currentHealth, 0, h1);
        }
        else if (currentHealth <= h2)
        {
            currentHealth += health;
            currentHealth = Mathf.Clamp(currentHealth, h1, h2);
        }
        else
        {
            currentHealth += health;
            currentHealth = Mathf.Clamp(currentHealth, h2, maxHealth);
        }
        coffinPlayer.fillAmount = currentHealth / maxHealth;
        coffinWerewolf.fillAmount = currentHealth / maxHealth;
    }
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) 
        {
            Debug.Log("collision");
            playerInRepairRange = true;
        }
        
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) 
        {
            playerInRepairRange = false;
        }
        
    }

}
