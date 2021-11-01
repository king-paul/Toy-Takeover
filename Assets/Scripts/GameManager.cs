using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum GameState { Init, Running, Paused, Win, Loss};

public class GameManager : MonoBehaviour
{
    [Header("Pause and Game Over Text")]
    public GameObject gameOverText;
    public GameObject pauseText;
    
    [Header("Enemy Spawning")]
    public bool spawnEnemies = false;
    [Tooltip("Enemy prefabs to be instantiated in each wave")]
    public GameObject[] enemies;
    [Tooltip("Contains the enemy spawnpoint game objects located in the hierarchy")]
    public Transform[] spawnPoints;
    [Tooltip("Place enemy wave scriptable objects here")]
    public EnemyWave[] waves;

    //public GameObject[] walls;

    GameState state;
    PlayerController player;

    // gui variables
    private float barWidth;
    float MAX_WIDTH;

    private int waveNumber = 1;
    private int enemiesLeft = 0;
    private int enemiesSpawned = 0;
    int enemiesRemaining;

    // public properties and functions
    public GameState State { get => state; set => state = value; }
    public int EnemiesLeft { get => enemiesRemaining; set => enemiesRemaining = value; }
    public void KillEnemy() { enemiesRemaining--;  }
    public void Die()
    {
        gameOverText.gameObject.SetActive(true);
        state = GameState.Loss;
        Time.timeScale = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        state = GameState.Running;

        // ensure that each hasSpawn variable is set to false by default
        foreach (EnemySpawn spawn in waves[waveNumber - 1].enemiesInWave)
            spawn.hasSpawned = false;
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
        if(spawnEnemies)
            UpdateEnemySpawns();
    }

    // handles keyboard input from the user
    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (state == GameState.Running)
            {
                pauseText.SetActive(true);
                Time.timeScale = 0;
                state = GameState.Paused;
            }
            else if (state == GameState.Paused)
            {
                pauseText.SetActive(false);
                Time.timeScale = 1;
                state = GameState.Running;
            }
        }
    }

    // Checks enemy wave data for new enemmis to be spawned at time the current time interval
    // and instantiates them. Also updates the wave number when there are not enemies left
    void UpdateEnemySpawns()
    {
        foreach (EnemySpawn spawn in waves[waveNumber-1].enemiesInWave)
        {
            GameObject enemy = enemies[(int)spawn.enemy];
            Transform spawnPoint = spawnPoints[(int)spawn.spawnPoint];            

            // check that the enemy has not already been spawned and
            // the time has been reached
            if (!spawn.hasSpawned && Time.time >= spawn.timePeriod)
            {
                Instantiate(enemy, spawnPoint.position, spawnPoint.rotation);
                spawn.hasSpawned = true;
                enemiesSpawned++;
                enemiesLeft++;

                Debug.Log("Enemies Spawned: " + enemiesSpawned +
                          ", Enemies in Scene " + enemiesLeft);
            }
        }

        // Check that all enemies have been spawned and there are none left in the wave
        if (enemiesSpawned == waves[waveNumber-1].enemiesInWave.Length && enemiesLeft == 0)
        {
            waveNumber++;
            enemiesSpawned = 0;
        }

    }

}