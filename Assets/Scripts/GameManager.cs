using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public enum GameState { Standby, Running, Paused, Win, Loss};

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(GUIController))]
public class GameManager : MonoBehaviour
{
    [Header("Debug Cheats")]
    public bool enableCheats = false;
    public KeyCode waveSkipKey = KeyCode.F2;

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
    Transform enemiesTransform;
    AudioSource musicSource, soundSource;
    PlayerSound playerAudio;

    // gui variables
    private float barWidth;
    float MAX_WIDTH;    
    float waveTime = 0f;
    float previousElapsedTime = 0f;

    private int waveNumber = 1;
    private int enemiesLeft = 0;
    private int enemiesSpawned = 0;
    private int enemiesInScene = 0;

    #region public properties and functions
    public GameState State { get => state; set => state = value; }
    public int EnemiesLeft { get => enemiesLeft; set => enemiesLeft = value; }
    public int WaveNumber { get => waveNumber; }
    public string WaveTime { 
        get {
            int waveTimeSeconds = (int)waveTime % 60;
            int waveTimeMins = (int)Mathf.Floor(waveTime / 60);

            if (waveTimeSeconds < 10)
                return waveTimeMins + ":0" + waveTimeSeconds;
            else
                return waveTimeMins + ":" + waveTimeSeconds;
        } 
    }
    public string TotalTime {
        get
        {
            int totalTimeSeconds = (int)Time.timeSinceLevelLoad % 60;
            int totalTimeMins = (int)Mathf.Floor(Time.timeSinceLevelLoad / 60);

            if (totalTimeSeconds < 10)
                return totalTimeMins + ":0" + totalTimeSeconds;
            else
                return totalTimeMins + ":" + totalTimeSeconds;
        }
    }

    public void KillEnemy() {
        enemiesInScene--;
        enemiesLeft--;  
    }

    public void Die()
    {
        gui.ShowGameOver();
        soundSource.PlayOneShot(playerAudio.gameOver);
        state = GameState.Loss;
        Cursor.lockState = CursorLockMode.None;
    }

    public void PlaySound(AudioClip clip, float volume)
    {
        soundSource.PlayOneShot(clip, volume);
    }

    public void PlayRandomSound(AudioClip[] clips, float volume)
    {
        int randomNum = Random.Range(0, clips.Length);
        if(randomNum == clips.Length)
        {
            randomNum = clips.Length - 1;
        }

        AudioClip randomSound = clips[randomNum];
        Debug.Log("Playing Enemy Sound: " + randomSound);
        soundSource.PlayOneShot(randomSound, volume);
    }
    #endregion

    // Start is called before the first frame update
    void Awake()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        gui = GetComponent<GUIController>();
        state = GameState.Running;
        enemiesTransform = GameObject.Find("Enemies").transform;
        musicSource = GetComponents<AudioSource>()[0];
        soundSource = GetComponents<AudioSource>()[1];
        playerAudio = GameObject.FindWithTag("Player").GetComponent<PlayerSound>();        
        waveTime = 0;
    }

    private void Start()
    {        
        SpawnItemPickups();
        if (spawnEnemies)
        {
            StartCoroutine(gui.ShowWaveStartText());
            enemiesLeft = waves[waveNumber - 1].enemiesInWave.Length;
            foreach (EnemySpawn spawn in waves[waveNumber - 1].enemiesInWave)
                spawn.hasSpawned = false;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if(spawnEnemies && waveNumber <= waves.Length && state == GameState.Running)
            UpdateEnemySpawns();

        if (Input.GetButtonDown("Cancel"))        
            TogglePause();

        // turn music on and off
        if (state != GameState.Running && musicSource.isPlaying)
        {
            // pause track if game is paused otherwise stop it
            if (state == GameState.Paused)
                musicSource.Pause();
            else if(state == GameState.Loss || state == GameState.Win)
                musicSource.Stop();
        }
        else if (state == GameState.Running && !musicSource.isPlaying)
            musicSource.Play();

        waveTime = Time.timeSinceLevelLoad - previousElapsedTime;

        // wave skipping key
        if(enableCheats && Input.GetKeyDown(waveSkipKey))
        {
            GameObject[] enemySpawns = GameObject.FindGameObjectsWithTag("Enemy");

            foreach(GameObject enemy in enemySpawns)
            {
                GameObject.Destroy(enemy);
                KillEnemy();
            }

            enemiesSpawned = waves[waveNumber - 1].enemiesInWave.Length;
            enemiesLeft = 0;
        }
    }

    public void TogglePause()
    {
        if (state == GameState.Running)
        {
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
            state = GameState.Paused;

            gui.TogglePauseMenu();
        }
        else if (state == GameState.Paused)
        {
            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.Locked;
            state = GameState.Running;

            gui.TogglePauseMenu();
        }

    }

    // Checks enemy wave data for new enemmis to be spawned at time the current time interval
    // and instantiates them. Also updates the wave number when there are not enemies left
    void UpdateEnemySpawns()
    {
        // Check that all enemies have been spawned and there are none left in the wave
        if (state == GameState.Running &&
            enemiesSpawned == waves[waveNumber - 1].enemiesInWave.Length && enemiesLeft == 0)
        {
            playerAudio.StopAllSounds();
            playerAudio.PlaySound(playerAudio.waveEnd);
            StartCoroutine(gui.ShowWaveCompletion());
            state = GameState.Standby;
            
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
                GameObject newEnemy = Instantiate(enemy, spawnPoint.position, spawnPoint.rotation, enemiesTransform);
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

    public void StartNextWave()
    {
        enemiesSpawned = 0;        
        previousElapsedTime = Time.timeSinceLevelLoad;
        state = GameState.Running;

        // WIN GAME CONDITION
        if (waveNumber >= waves.Length)
        {            
            playerAudio.PlaySound(playerAudio.levelComplete);                        
            state = GameState.Win;
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;

            gui.ShowLevelComplete();            
        }
        else
        {
            waveNumber++;

            StartCoroutine(gui.ShowWaveStartText());

            SpawnItemPickups(); // spawn the next set of item pickups
            enemiesLeft = waves[waveNumber - 1].enemiesInWave.Length;
            // reset hasSpawned values
            foreach (EnemySpawn spawn in waves[waveNumber - 1].enemiesInWave)
                spawn.hasSpawned = false;
        }
    }

    void SpawnItemPickups()
    {
        GameObject[] pickupSpawners = GameObject.FindGameObjectsWithTag("PickupSpawner");

        foreach (GameObject spawner in pickupSpawners)
        {
            spawner.GetComponent<ItemPickupSpawner>().SpawnPickup();
        }

    }

}