using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

enum SpawnStates
{
    SPAWNING, WAITING, COUNTINGDOWN, CANSPAWN
}


public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject Player;
    [SerializeField] GameObject Vampire;
    [SerializeField] GameObject[] Enemies;
    [SerializeField] GameObject Coffin;
    [SerializeField] Light sun;
    private Coffin coffinScript;
    [SerializeField] Upgrade upgrade;

    
    [Header("Phase Variables")]
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
    [SerializeField] public int kills;

    [Header("Weighted Probability")]
    [SerializeField] float archerSpawnRate; 
    [SerializeField] float knightSpawnRate;
    [SerializeField] float mageSpawnRate;
    [SerializeField, ReadOnly] float weightSum;

    [Header("Spawn Variables")]
    [SerializeField] float spawnRate;
    [SerializeField] Transform[] spawnPos;
    [SerializeField] SpawnStates spawnState = SpawnStates.COUNTINGDOWN;
    PlayerScript pScript;
    public static GameManager instance;

    [Header("Wave Variables")]
    [SerializeField] int nextWave = 0;
    [SerializeField] float timeBetweenWaves = 5f;
    [SerializeField, ReadOnly] float currentTimeBetweenWave;
    [SerializeField] int waveNumber;
    [SerializeField] int enemyCount;
    [SerializeField] int killsThisWave;
    
    [Header("Swapping References")]
    [SerializeField] Transform servantCamPos;
    [SerializeField] Transform servantOrientation, vampCamPos, vampOrentation;
    [SerializeField] AimScript camHolderAim;
    [SerializeField] Image blackScreen;
    
    [Header("Animations")]
    [SerializeField] Animator servantAnim;

    [Header("UI")]
    [SerializeField] TMP_Text phaseTimer;
    [SerializeField] TMP_Text killsCounter;
    [ReadOnly, SerializeField] float score;
    [ReadOnly, SerializeField] float timeSurvived;
    [ReadOnly, SerializeField] bool game;
    private float crossbowDamage;
    Color black;
    float targetAlpha, targetSpeed;

    void Start()
    {
        crossbowDamage = 20f;
        pScript = Player.GetComponent<PlayerScript>();
        currentTimeBetweenWave = timeBetweenWaves;
        spawnState = SpawnStates.CANSPAWN;
        black = Color.black;
        timeSurvived = 0f;
        game = true;
        coffinScript = Coffin.GetComponent<Coffin>();
        StartPhase();
    }

    void Update()
    {
        if (currentTimeBetweenWave <= 0)
        {
            if (spawnState == SpawnStates.CANSPAWN)
            {
                StartCoroutine(StartSpawning());
                currentTimeBetweenWave = timeBetweenWaves;
            }
        }
        else
        {
            currentTimeBetweenWave -= Time.deltaTime;
        }
        if(game)
            timeSurvived+=Time.deltaTime;
        //Black Screen Fade
        black.a = Mathf.Lerp(black.a, targetAlpha, targetSpeed*Time.deltaTime);
        blackScreen.color = black;
    }

    private void StartPhase()
    {
        
        Player.GetComponent<PlayerScript>().enabled = true;
        servantAnim.SetTrigger("Start");
        Vampire.SetActive(false);
        camHolderAim.SetPlayer(servantCamPos, servantOrientation);
        currentWave = 0;
        wavesThisPhase = 2 + Mathf.RoundToInt(phase/2);
        enemiesThisWave = 5;
        killsThisWave = 0;
        //upgrade.Shop(false);
        targetSpeed = 0.25f;
        targetAlpha = 0f;
        spawnState= SpawnStates.CANSPAWN;
        // text here saying phase x start

    }

    public GameObject GiveTarget(Component component)
    {
        if(component.name.Contains("Archer"))
        {
            return Player;
        }
        else if(component.name.Contains("Knight"))
        {
            return Random.Range(0, 100)>50?Player:Coffin;
        }
        else if(component.name.Contains("Mage"))
        {
            return Coffin;
        }
        else
        {
            return Player;
        }
    }

    public float CrossbowDamageUpdate()
    {
        return crossbowDamage;
    }

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
        killsThisWave ++;
        pScript.HandleWerewolfUI();
        if(killsCounter!=null)
        killsCounter.text = "Kills: " + kills;
    }

    private void Awake()
    {
        if(instance==null)
        {
            instance = this;
        }
    }
    public bool IsPlayerBerserking()
    {
        return pScript.InBerserk;
    }

    public GameObject WhereIsPlayer()
    {
        return Player;
    }

    public void DamagePlayer(float damage)
    {
        pScript.TakeDamage(damage);
    }

    private IEnumerator StartSpawning()
    {
        spawnState = SpawnStates.SPAWNING;
        for (int i = 0; i < enemiesThisWave; i++)
        {
            yield return new WaitForSeconds(spawnRate);
            Instantiate(Enemies[RunRandom()], spawnPos[Random.Range(0, spawnPos.Length)].position, Quaternion.identity);
        }

        spawnState = SpawnStates.WAITING;
        yield return new WaitUntil(() => killsThisWave == enemiesThisWave);
        currentWave++;
        if(currentWave<wavesThisPhase)
        {
            Debug.Log("Wave Complete, moving on to next wave");
            currentTimeBetweenWave = timeBetweenWaves;
            killsThisWave = 0;
            spawnState = SpawnStates.CANSPAWN;
        }
        else
        {
            Debug.Log("Wave And Phase Complete, nighttime");
            EndPhase();
        }
    }

    private void EndPhase()
    {
        sun.intensity = 0;
        souls = kills * 150;
        //coffin.Upgrade(true, souls);
        
        
        StartCoroutine(EndPhaseTimer());
    }
    IEnumerator EndPhaseTimer()
    {
        //Death Animation->Fade to black->Fade back to Vampire->Do Vamp Phase Things
        servantAnim.ResetTrigger("Start");
        servantAnim.SetTrigger("Death");
        targetSpeed = 1.5f;
        targetAlpha = 1f;
        Player.GetComponent<PlayerScript>().enabled = false;
        yield return new WaitForSeconds(2f);
        targetAlpha = 0f;
        Vampire.SetActive(true);
        camHolderAim.SetPlayer(vampCamPos, vampOrentation);

        //phaseTimer.gameObject.SetActive(true);
        int i = 10;//THIS SHOULD BE CHANGED BACK TO 30++++++++++++++++++++================================ 
        while(i>0)
        {
            //phaseTimer.text = "Time remaining: "+i;
            Debug.Log("Time remaining: "+ i);
            yield return new WaitForSeconds(1f);
            i--;
        }
        
        targetAlpha = 1f;
        yield return new WaitForSeconds(1f);
        Vampire.SetActive(false);
        //phaseTimer.gameObject.SetActive(false);
        StartPhase();
    }
    #region PostGame
    public void GameOver()
    {
        Debug.Log("Game Over");
        game = false;
        //
        //End Game Score and High Score Calculation
    }
    void CalcScore()
    {
        score = kills* phase* timeSurvived;
        //scoreText.text = "Score: "+ score;
        //Calculate Score
    }
    
    #endregion

}
