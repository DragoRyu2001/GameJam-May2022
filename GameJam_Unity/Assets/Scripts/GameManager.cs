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
    [SerializeField] GameObject playerDown;
    [SerializeField] AudioManager AM;
    private Coffin coffinScript;
    [SerializeField] Upgrade upgrade;

    private GameObject currentPlayerDown;
    
    [Header("Phase Variables")]
    [SerializeField] int phase;
    [SerializeField] int currentWave;
    [SerializeField] int enemiesThisWave;
    [SerializeField] int startEnemies;


    [Header("Scaling Parameters")]
    [SerializeField] float crossbowBaseDamage;
    [SerializeField] float swordBaseDamage;
    [SerializeField] float mageBaseDamage;
    [SerializeField] float mageReloadTime;
    [SerializeField] float archerReloadTime;
    [SerializeField] float mageHealth;
    [SerializeField] float archerHealth;
    [SerializeField] float knightHealth;
    [SerializeField] float crossbowCurrentBaseDamage;
    [SerializeField] float swordCurrentBaseDamage;
    [SerializeField] float mageCurrentBaseDamage;
    [SerializeField] float mageCurrentReloadTime;
    [SerializeField] float archerCurrentReloadTime;
    [SerializeField] float mageCurrentHealth;
    [SerializeField] float archerCurrentHealth;
    [SerializeField] float knightCurrentHealth;

    [Header("Player Progression")]
    [SerializeField] int survivedWaves;
    [SerializeField] int survivedPhases;
    [SerializeField] int souls;
    [SerializeField] public int kills;

    [Header("Weighted Probability")]
    [SerializeField] float archerSpawnRate; 
    [SerializeField] float knightSpawnRate;
    [SerializeField] float mageSpawnRate;
    [SerializeField] float weightSum;

    [Header("Spawn Variables")]
    [SerializeField] float spawnRate;
    [SerializeField] Transform[] spawnPos;
    [SerializeField] SpawnStates spawnState = SpawnStates.COUNTINGDOWN;
    PlayerScript pScript;
    WerewolfScript wScript;
    public static GameManager instance;

    [Header("Wave Variables")]
    [SerializeField] float timeBetweenWaves = 5f;
    [SerializeField] float currentTimeBetweenWave;
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
    [SerializeField] static bool GameIsPaused = false;
    [SerializeField] TMP_Text phaseTimer1;
    [SerializeField] TMP_Text phaseTimer2;
    [SerializeField] TMP_Text killsCounter;
    [SerializeField] TMP_Text timer;
    [SerializeField] GameObject pauseMenu;

    [Header("Post Game")]
    [SerializeField] GameObject postPanel;
    [SerializeField] TMP_Text scoreText;
    [SerializeField] TMP_Text highScoreText;
    [SerializeField] TMP_Text timeSurvivedText;
    [SerializeField] TMP_Text killsText;
    [SerializeField] TMP_Text soulsEarnedText;
    [SerializeField] int soulsEarned;

    [Header("ReadOnly")]
    [SerializeField] int score, highScore;
    [SerializeField] float timeSurvived;
    [SerializeField] bool game, vamp;

    private float crossbowDamage;
    Color black;
    float targetAlpha, targetSpeed;

    #region Setters and Getters
    public float CrossbowCurrentBaseDamage { get => crossbowCurrentBaseDamage; set => crossbowCurrentBaseDamage = value; }
    public float SwordCurrentBaseDamage { get => swordCurrentBaseDamage; set => swordCurrentBaseDamage = value; }
    public float MageCurrentBaseDamage { get => mageCurrentBaseDamage; set => mageCurrentBaseDamage = value; }
    public float MageCurrentReloadTime { get => mageCurrentReloadTime; set => mageCurrentReloadTime = value; }
    public float ArcherCurrentReloadTime { get => archerCurrentReloadTime; set => archerCurrentReloadTime = value; }
    public float MageCurrentHealth { get => mageCurrentHealth; set => mageCurrentHealth = value; }
    public float ArcherCurrentHealth { get => archerCurrentHealth; set => archerCurrentHealth = value; }
    public float KnightCurrentHealth { get => knightCurrentHealth; set => knightCurrentHealth = value; }

    #endregion

    void Start()
    {
        SetCurrentBaseStats();
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        crossbowDamage = 20f;
        pScript = Player.GetComponent<PlayerScript>();
        wScript = Player.GetComponent<WerewolfScript>();
        currentTimeBetweenWave = timeBetweenWaves;
        spawnState = SpawnStates.CANSPAWN;
        black = Color.black;
        timeSurvived = 0f;
        game = true;
        soulsEarned = 0;
        startEnemies = enemiesThisWave;
        postPanel.SetActive(false);
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
        if (game && !vamp)
        {
            timeSurvived += Time.deltaTime;
            timer.text = "Time: " + (int)timeSurvived;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Toggle();

            }
            else
            {
                Toggle();
            }
        }

        //Black Screen Fade
        black.a = Mathf.Lerp(black.a, targetAlpha, targetSpeed*Time.deltaTime);
        blackScreen.color = black;
    }

    private void StartPhase()
    {
        AM.StopPlaying();
        AM.PlayGameMusic();
        vamp = false;
        pScript.enabled = true;
        Vampire.SetActive(false);
        camHolderAim.SetPlayer(servantCamPos, servantOrientation);
        currentWave = 0;
        killsThisWave = 0;
        upgrade.Shop(false);
        targetSpeed = 0.25f;
        targetAlpha = 0f;
        spawnState= SpawnStates.CANSPAWN;
        upgrade.gameObject.SetActive(false);
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
            return Random.Range(0, 100) > 70 ? Player : Coffin;
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
        pScript.KillsForUlt++;
        pScript.HandleUltUI();
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

    public bool IsPlayerRewinding()
    {
        return pScript.IsRewinding;
    }

    public bool IsPlayerAlive()
    {
        return pScript.IsAlive;
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
        if(currentWave<3)
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
        survivedPhases++;
        sun.intensity = 0;
        souls = kills * 15000;
        soulsEarned += souls;
        upgrade.gameObject.SetActive(true);
        upgrade.UpdateSouls(souls);
        vamp = true;

        StartCoroutine(EndPhaseTimer());
    }

    public void Toggle()
    {
        GameIsPaused = !GameIsPaused;
        Cursor.lockState = GameIsPaused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = GameIsPaused;
        pauseMenu.SetActive(GameIsPaused);
        Time.timeScale = GameIsPaused ? 0f : 1f;
    }

    IEnumerator EndPhaseTimer()
    {
        //Death Animation->Fade to black->Fade back to Vampire->Do Vamp Phase Things
        servantAnim.ResetTrigger("Start");
        AM.StopPlaying();
        AM.PlayVampMusic();
        targetSpeed = 1.5f;
        targetAlpha = 1f;
        servantAnim.SetTrigger("Death");
        Invoke(nameof(SetMCDownAfterAnim), 2.4f);
        yield return new WaitForSeconds(2f);
        targetAlpha = 0f;
        Vampire.SetActive(true);
        camHolderAim.SetPlayer(vampCamPos, vampOrentation);

        phaseTimer1.gameObject.SetActive(true);
        int i = 30;
        while (i > 0)
        {
            phaseTimer1.text = "Time remaining: " + i;
            phaseTimer2.text = i.ToString();
            Debug.Log("Time remaining: \n " + i);
            yield return new WaitForSeconds(1f);
            i--;
        }

        targetAlpha = 1f;
        yield return new WaitForSeconds(1f);
        Vampire.SetActive(false);
        phaseTimer1.gameObject.SetActive(false);
        playerDown.SetActive(false);
        Player.SetActive(true);
        ScaleEnemies();
        StartPhase();
    }

    void SetCurrentBaseStats()
    {
        mageCurrentBaseDamage = mageBaseDamage;
        crossbowCurrentBaseDamage = crossbowBaseDamage;
        swordCurrentBaseDamage = swordBaseDamage;
        mageCurrentReloadTime = mageReloadTime;
        archerCurrentReloadTime = archerReloadTime;
        mageCurrentHealth = mageHealth;
        archerCurrentHealth = archerHealth;
        knightCurrentHealth = knightHealth;
    }

    private void ScaleEnemies()
    {
            mageCurrentBaseDamage += mageBaseDamage * survivedPhases * 0.7f;
        crossbowCurrentBaseDamage += crossbowBaseDamage * survivedPhases * 0.3f;
           swordCurrentBaseDamage += swordBaseDamage * survivedPhases * 0.05f;

        if(mageReloadTime>3f)
        {
            mageCurrentReloadTime -= mageReloadTime * survivedPhases * 0.1f;
        }
        if(archerReloadTime>1f)
        {
            archerCurrentReloadTime -= archerReloadTime * survivedPhases * 0.1f;
        }

          mageCurrentHealth += mageHealth * survivedPhases * 0.2f;
        archerCurrentHealth += archerHealth * survivedPhases * 0.1f;
        knightCurrentHealth += knightHealth * survivedPhases * 0.05f;
        enemiesThisWave = startEnemies + survivedPhases * 2;
        Debug.Log(enemiesThisWave);
    }

    private void SetMCDownAfterAnim()
    {
        Player.SetActive(false);
        playerDown.SetActive(true);
        playerDown.transform.SetPositionAndRotation(Player.transform.position, Player.transform.rotation);
    }
    #region PostGame
    public void GameOver()
    {
        Debug.Log("Game Over");
        game = false;
        CalcScore();
        postPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        //
        //End Game Score and High Score Calculation
    }
    void CalcScore()
    {
        score = kills* survivedPhases* (int)timeSurvived;
        highScore = highScore<score?score:highScore;
        PlayerPrefs.SetInt("HighScore", (int)highScore);
        scoreText.text = ""+ score;
        highScoreText.text = ""+highScore;
        timeSurvivedText.text = (int)(timeSurvived/60)+":"+timeSurvived%60;
        soulsEarnedText.text = ""+soulsEarned;
        killsText.text = ""+kills;
        //Calculate Score
    }
    
    #endregion

}
