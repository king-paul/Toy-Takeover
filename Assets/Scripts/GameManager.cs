using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

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

    [Header("Cars and Dolly Carts")]
    [Tooltip("Drag the dolly carts into these to make the cars respawn")]
    public Transform[] dollyCarts;
    [Tooltip("The amount of seconds it takes for a car to respawn after being destroyed")]
    [SerializeField][Range(1, 30)]
    float carRespawnTime = 5f;
    public bool spawnAtOriginalPosition = true;

    [Header("Enemy Waves")]
    [Tooltip("Place enemy wave scriptable objects here")]
    public EnemyWave[] waves;

    [Header("Cameras")]
    public GameObject firstPersonCamera;

    GameState state;
    GUIController gui;
    Transform enemiesTransform;
    AudioSource musicSource, soundSource;
    PlayerSound playerAudio;
    Volume cameraVolume;
    List<CarSpawner> carSpawners;

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

    // Motion blur toggle property
    public bool MotionBlur
    {
        get
        {
            MotionBlur motionBlur;
            cameraVolume.profile.TryGet<MotionBlur>(out motionBlur);
            return motionBlur.active;            
        }

        set
        {
            MotionBlur motionBlur;
            cameraVolume.profile.TryGet<MotionBlur>(out motionBlur);
            motionBlur.active = value;
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

    #region Unity Functions
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

        // set mtion blue setting
        cameraVolume = firstPersonCamera.GetComponents<Volume>()[1];
        MotionBlur motionBlur;
        cameraVolume.profile.TryGet<MotionBlur>(out motionBlur);

        float blurOn = PlayerPrefs.GetInt("MotionBlur", 1);
        if(blurOn == 1)
            motionBlur.active = true;
        if(blurOn == 0)
            motionBlur.active = false;

        // Initialise car spawner list
        carSpawners = new List<CarSpawner>();
        foreach(Transform dollyTransform in dollyCarts)
        {
            var dollyCart = dollyTransform.GetComponent<CinemachineDollyCart>();            
            CarSpawner spawner = new CarSpawner(dollyTransform, dollyCart.m_Position);

            carSpawners.Add(spawner);
        }
    }

    private void Start()
    {        
        SpawnItemPickups();
        if (spawnEnemies)
        {
            StartCoroutine(gui.ShowWaveStartText());
            enemiesLeft = waves[waveNumber - 1].enemiesInWave.Length;

            // spawn wave enemies
            foreach (EnemySpawn spawn in waves[waveNumber - 1].enemiesInWave)
                spawn.hasSpawned = false;

            // spawn cars and pass spawnpoint as a reference
            foreach (CarSpawner spawner in carSpawners)
            {
                var carObject = spawner.dollyCart.GetChild(0);
                    //Instantiate(enemies[2], spawner.spawnPoint);
                var carEnemy = carObject.GetComponent<DollyCartEnemy>();
                //carEnemy.SpawnPoint = spawner.spawnPoint;
                carEnemy.SetSpawner(spawner);
            }

        }

    }

    // Update is called once per frame
    void Update()
    {
        if(spawnEnemies && waveNumber <= waves.Length && state == GameState.Running)
            UpdateEnemySpawns();

        if (Input.GetButtonDown("Cancel"))
        {
            if (!gui.optionsMenu.activeInHierarchy)
                TogglePause();
            else
                gui.ToggleOptionsMenu();
        }            

        // turn music on and off
        if ((state == GameState.Loss || state == GameState.Win) && musicSource.isPlaying)
        {
            musicSource.Stop();
        }
        else if (!musicSource.isPlaying && state == GameState.Running)
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
    #endregion

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

        // Spawn enemies in the wave when the time period has been reached
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

        // Respawn cars in the scene
        foreach(CarSpawner spawner in carSpawners)
        {
            if(spawner.respawning)
            {
                spawner.spawnTimeElapsed += Time.deltaTime;
                //Debug.Log("Respawn Timer: " + spawner.spawnTimeElapsed);

                if(spawner.spawnTimeElapsed >= carRespawnTime)
                {
                    //Debug.Log("Respawning Car...");                    
                    var carObject = Instantiate(enemies[2], spawner.dollyCart);
                    var carEnemy = carObject.GetComponent<DollyCartEnemy>();
                    carEnemy.SpawnPoint = spawner.dollyCart;
                    carEnemy.SetSpawner(spawner);

                    spawner.Reset();

                    var dollyCart = spawner.dollyCart.GetComponent<CinemachineDollyCart>();                    
                    dollyCart.m_Speed = 20;
                }
            }

        }

    }

    public void ResetDollyCart(CarSpawner spawner)
    {
       var dollyCart = spawner.dollyCart.GetComponent<CinemachineDollyCart>();

       dollyCart.m_Position = spawner.position;
       dollyCart.m_Speed = 0;

       spawner.respawning = true; // starts respawn timer
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