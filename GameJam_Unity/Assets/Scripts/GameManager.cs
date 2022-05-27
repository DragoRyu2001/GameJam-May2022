using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject Player;
    [SerializeField] GameObject Archer;
    [SerializeField] GameObject Knight;
    [SerializeField] GameObject Mage;
    
    [Header("Phase and Wave Variables")]
    [SerializeField] int phase;
    [SerializeField, ReadOnly] int currentWave;
    [SerializeField] int wavesThisPhase;
    [SerializeField] int enemiesThisWave;

    [Header("Scaling Parameters")]
    [SerializeField] int enemyWaveDelta;
    [SerializeField] float difficultyScaling;

    [Header("Player Progression")]
    [SerializeField] int survivedWaves;
    [SerializeField] int survivedPhases;
    [SerializeField] int souls;
    [SerializeField] int kills;

    [Header("Weighted Probability")]
    [SerializeField] float archerSpawnRate; 
    [SerializeField] float knightSpawnRate; 
    [SerializeField] float mageSpawnRate;

    public GameObject GiveTarget()
    {
        throw new System.NotImplementedException();
    }

    [SerializeField, ReadOnly] float weightSum;

    [Header("Spawn Variables")]
    [SerializeField] float spawnRate;
    [SerializeField] Transform[] spawnPos;

    PlayerScript pScript;
    public static GameManager instance;

    bool nextWaveReady=true;

    public int RunRandom()
    {
        weightSum = archerSpawnRate + knightSpawnRate + mageSpawnRate;
        float currentWeight = Random.Range(0, weightSum);
        if (currentWeight > archerSpawnRate + knightSpawnRate)
        {
            return 2;
        }
        else if(currentWeight>archerSpawnRate)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }


    public void IncreaseKills()
    {
        kills++;
        if(CheckIfFinalKill())
        {
            Debug.Log("killed all");
        }
    }

    private bool CheckIfFinalKill()
    {
        throw new System.NotImplementedException();
    }

    private void Awake()
    {
        if(instance==null)
        {
            instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    { 
        pScript = Player.GetComponent<PlayerScript>();
        Invoke(nameof(StartFirstPhase), 2f);
    }


    public bool IsPlayerBerserking()
    {
        return pScript.InBerserk;
    }

    public Vector3 WhereIsPlayer()
    {
        return Player.transform.position;
    }

    public void DamagePlayer(float damage)
    {
        pScript.TakeDamage(damage);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void StartFirstPhase()
    {
        phase = 0;
        wavesThisPhase = 2;
        enemiesThisWave = 15;
        StartCoroutine(StartSpawning());
    }

    private IEnumerator StartSpawning()
    {
        for (int i = 0; i < enemiesThisWave; i++)
        {
            yield return new WaitForSeconds(spawnRate);
            switch (RunRandom())
            {
                case 0:
                    {
                        Instantiate(Archer, spawnPos[Random.Range(0, spawnPos.Length)].position, Quaternion.identity);
                        break;
                    }

                case 1:
                    {
                        Instantiate(Knight, spawnPos[Random.Range(0, spawnPos.Length)].position, Quaternion.identity);
                        break;
                    }

                case 2:
                    {
                        Instantiate(Mage, spawnPos[Random.Range(0, spawnPos.Length)].position, Quaternion.identity);
                        break;
                    }
            }
        }
    }

    private void StartPhase()
    {
        currentWave = 0;
        wavesThisPhase = 2 + phase;
        enemiesThisWave = 15 +(enemyWaveDelta*survivedPhases);
        
    }
}
