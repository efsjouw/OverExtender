using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Central game script
/// Actively controls spawning/despawning enemies
/// </summary>
public class Game : SingletonBehaviour<Game> {

    [SerializeField] PlayField playField;

    public GameObject playerExplosionPrefab;
    public GameObject enemyExplosionPrefab;

    public readonly string TAG_ENEMY_PICKUP = "EnemyPickup";
    public readonly string LAYER_ENEMY_NAME = "Enemy";

    public Player player;
    private PlayerController playerController;

    public GameObject enemyPrefab;
    public Transform enemySpawnpoints;

    public int maxEnemyObjects = 48;
    public float spawnInterval = 1;

    private int enemyCount = 0;

    [SerializeField] float heightAbovePlane = 1;

    public Text scoreText;
    public Text targetText;

    public Text speedText;
    public Text timeText;

    private int highScore;
    private int score;
    [SerializeField] float targetScore = 150;
    [SerializeField] float targetMultiplier = 1.33f;

    private float gameSpeed = 0;
    [SerializeField] int maxCombo = 0;
    [SerializeField] int charges = 12;

    private System.Random random;

    private int spawnIndex = 0;
    private int lastIndex = 0;

    private int gameTime = 0;
    private bool gameRunning = true;

    private bool spawnSpecial = false;
    private bool spawnQuicken = false;
    private bool spawnExploder = false;
    private bool spawnMultiplier = false;

    PlayField.SpawnDirection[] spawnDirectionsOriginal;
    List<PlayField.SpawnDirection> spawnDirections;

    bool playerAlive;

    void Start () {

        //Screen.SetResolution(1920, 1080, false);
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        spawnDirectionsOriginal = (PlayField.SpawnDirection[])Enum.GetValues(typeof(PlayField.SpawnDirection));
        spawnDirections = spawnDirectionsOriginal.ToList();

        random = new System.Random();
        Screen.orientation = ScreenOrientation.Landscape;
        playerController = player.GetComponent<PlayerController>();

        scoreText.text = score.ToString();
        targetText.text = targetScore.ToString();

        setSpeedText(gameSpeed);

        spawnPlayer();
        StartCoroutine(secondsTimer());
        StartCoroutine(spawnEnemies());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            AndroidLikeDialog.instance.show("Quit", "Are you sure you want to quit?", quitGame);        
    }

    public float getEnemySpeedMultiplier()
    {
        return gameSpeed + 1;
    }

    public void enemyDestroyed()
    {
        addScore(1);
        enemyDespawned();
    }

    /// <summary>
    /// Set speed text with value + 1 for visual feedback
    /// In the calculation this will start witsh 0
    /// </summary>
    /// <param name="value"></param>
    private void setSpeedText(float value)
    {
        speedText.text = Math.Round((value + 1)).ToString();
    }

    public void quicken(float amount = 0.05f)
    {        
        gameSpeed += amount;
        setSpeedText(gameSpeed);
    }
    
    public void addScore(int amount)
    {
        score += amount;
        scoreText.text = score.ToString();
        if(score >= targetScore)
        {
            targetScore *= targetMultiplier;
            player.addCharge(1);
            targetText.text = ((int)targetScore).ToString();
        }
    }

    public void enemySpawned(int count = 1)
    {        
        enemyCount += count;
    }
    
    public void enemyDespawned()
    {
        enemyCount--;
    }

    public IEnumerator spawnEnemies()
    {
        while(gameRunning)
        {
            if (enemyCount < maxEnemyObjects)
            {
                spawnIndex = random.Next(0, enemySpawnpoints.childCount);
                while(spawnIndex == lastIndex)
                {
                    spawnIndex = random.Next(0, enemySpawnpoints.childCount);
                }
                GameObject enemyObject = Instantiate(enemyPrefab, enemySpawnpoints.GetChild(spawnIndex));
                enemyObject.SetActive(true);

                Enemy enemy = enemyObject.GetComponent<Enemy>();
                if(spawnSpecial)
                {
                    if (spawnQuicken) enemy.setPickupEnemy(Pickup.PickupType.Quicken);
                    else if (spawnExploder) enemy.setEnemyType(Enemy.EnemyType.Exploder);
                    else if (spawnMultiplier) enemy.setPickupEnemy(Pickup.PickupType.Multiplier);
                }
                else
                {
                    enemy.setEnemyType(Enemy.EnemyType.DropPickup);
                }

                int randomIndex = (int)UnityEngine.Random.Range(0, spawnDirections.Count - 1);
                PlayField.SpawnDirection spawnDirection = spawnDirections[randomIndex];

                enemy.setMovePosition(playField.getMovePosition(spawnDirection, enemy));

                lastIndex = spawnIndex;
            }
            yield return new WaitForSeconds(spawnInterval - (gameSpeed / 10));
        }
    }    

    public IEnumerator secondsTimer()
    {
        while(gameRunning)
        {
            yield return new WaitForSecondsRealtime(1);
            gameTime++;

            TimeSpan timeSpan = TimeSpan.FromSeconds(gameTime);
            timeText.text = string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
            spawnQuicken = timeSpan.Seconds == 15;
            spawnMultiplier = timeSpan.Seconds == 30;
            spawnExploder = timeSpan.Seconds == 45;
            spawnSpecial = spawnQuicken || spawnMultiplier || spawnExploder;
        }
    }

    public void killPlayer()
    {
        player.gameObject.SetActive(false);
        playerAlive = false;
    }

    public void spawnPlayer(float delay = 0)
    {
        StartCoroutine(spawnPlayerRoutine(delay));
    }

    private IEnumerator spawnPlayerRoutine(float delay = 0)
    {        
        yield return new WaitForSeconds(delay);

        player.gameObject.SetActive(true);
        playerAlive = true;

        Transform cameraTransform = Camera.main.transform;
        Transform playerTransform = player.transform;

        Vector3 playerSpawnPosition = new Vector3(
            cameraTransform.position.x / 2,
            cameraTransform.position.y - playerTransform.localScale.y,
            cameraTransform.position.z / 2);

        player.transform.SetPositionAndRotation(playerSpawnPosition, player.transform.rotation);        
    }  

    private void quitGame()
    {
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }    
}
