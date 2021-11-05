using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum GameState { Init, Running, Paused, Win, Loss};

public class GameManager : MonoBehaviour
{    
    [Header("Enemy Spawning")]
    public bool spawnEnemies = false;
    [Tooltip("Enemy prefabs to be instantiated in each wave")]
    public GameObject[] enemies;
    [Tooltip("Contains the enemy spawnpoint game objects located in the hierarchy")]
    public Transform[] spawnPoints;
    [Tooltip("Place enemy wave scriptable objects here")]
    public EnemyWave[] waves;

    GameState state;
    GUIController gui;

    // gui variables
    private float barWidth;
    float MAX_WIDTH;    
    float waveTime = 0f;
    float previousElapsedTime = 0f;

    private int waveNumber = 1;
    private int enemiesLeft = 0;
    private int enemiesSpawned = 0;
    private int enemiesInScene = 0;

    // public properties and functions
    public GameState State { get => state; set => state = value; }
    public int EnemiesLeft { get => enemiesLeft; set => enemiesLeft = value; }
    public int WaveNumber { get => waveNumber; }
    public void KillEnemy() {
        enemiesInScene--;
        enemiesLeft--;  }

    public void Die()
    {
        gui.ShowGameOver();        
        state = GameState.Loss;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0;
    }

    // Start is called before the first frame update
    void Awake()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        gui = GetComponent<GUIController>();
        state = GameState.Running;

        // ensure that each hasSpawn variable is set to false by default
        if (spawnEnemies)
        {
            enemiesLeft = waves[waveNumber - 1].enemiesInWave.Length;
            foreach (EnemySpawn spawn in waves[waveNumber - 1].enemiesInWave)
                spawn.hasSpawned = false;
        }

        waveTime = 0;        
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
        if(spawnEnemies && waveNumber <= waves.Length && state == GameState.Running)
            UpdateEnemySpawns();
    }

    // handles keyboard input from the user
    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (state == GameState.Running)
            {                
                Time.timeScale = 0;
                Cursor.lockState = CursorLockMode.None;
                state = GameState.Paused;
            }
            else if (state == GameState.Paused)
            {
                Time.timeScale = 1;
                Cursor.lockState = CursorLockMode.Locked;
                state = GameState.Running;
            }

            gui.TogglePauseMenu();
        }
    }

    // Checks enemy wave data for new enemmis to be spawned at time the current time interval
    // and instantiates them. Also updates the wave number when there are not enemies left
    void UpdateEnemySpawns()
    {
        waveTime = Time.time - previousElapsedTime;

        // Check that all enemies have been spawned and there are none left in the wave
        if (enemiesSpawned == waves[waveNumber - 1].enemiesInWave.Length && enemiesLeft == 0)
        {
            NextWave();
            return;
        }

        foreach (EnemySpawn spawn in waves[waveNumber-1].enemiesInWave)
        {
            GameObject enemy = enemies[(int)spawn.enemy];
            Transform spawnPoint = spawnPoints[(int)spawn.spawnPoint];            

            // check that the enemy has not already been spawned and
            // the time has been reached
            if (!spawn.hasSpawned && waveTime >= spawn.timePeriod)
            {
                GameObject newEnemy = Instantiate(enemy, spawnPoint.position, spawnPoint.rotation);
                spawn.hasSpawned = true;
                enemiesSpawned++;
                enemiesInScene++;

                PatrolEnemyAI patrolEnemy = newEnemy.GetComponent<PatrolEnemyAI>();
                if(patrolEnemy != null)
                {
                    patrolEnemy.SetSpawnPoint(spawnPoint);
                }

                //Debug.Log("Enemies Spawned: " + enemiesSpawned +
                          //", Enemies in Scene " + enemiesInScene);
            }
        }

    }

    void NextWave()
    {
        waveNumber++;
        enemiesSpawned = 0;        
        previousElapsedTime = Time.time;

        if (waveNumber > waves.Length)
        {
            gui.ShowLevelComplete();            
            state = GameState.Win;
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            enemiesLeft = waves[waveNumber - 1].enemiesInWave.Length;
            // reset hasSpawned values
            foreach (EnemySpawn spawn in waves[waveNumber - 1].enemiesInWave)
                spawn.hasSpawned = false;
        }
    }

}