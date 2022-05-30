using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coffin : MonoBehaviour
{
    [SerializeField] GameObject upgradeObj;
    [SerializeField] GameObject coffinOpen;
    [SerializeField] GameObject coffinClose;
    [SerializeField] float maxHealth;
    [ReadOnly, SerializeField] float currentHealth;

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
        if(Input.GetKeyDown(KeyCode.I))
        {
            TakeDamage(25);
        }
        if(Input.GetKeyDown(KeyCode.J))
        {
            HealCoffin(25);
        }
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
    }
    public void HealCoffin(float health)
    {
        
        if(currentHealth<=h1)
        {
            currentHealth+=health;
            currentHealth = Mathf.Clamp(currentHealth, 0, h1);
        }
        else if(currentHealth<=h2)
        {
            currentHealth+=health;
            currentHealth = Mathf.Clamp(currentHealth, h1, h2);
        }
        else
        {
            currentHealth+=health;
            currentHealth = Mathf.Clamp(currentHealth, h2, maxHealth);
        }
        Debug.Log(currentHealth);
    }
    public void Upgrade(bool enable, int souls)
    {
        upgradeObj.SetActive(enable);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player") 
        {
            playerInRepairRange = true;
        }
        
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player") 
        {
            playerInRepairRange = false;
        }
        
    }

}
