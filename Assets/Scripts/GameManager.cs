﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public enum GameState { Init, Running, Paused, Win, Loss};

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(GUIController))]
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

    [Header("Enemy Waypoints")]
    [Tooltip("Waypoints on bottom of the level that all enemies can travel to")]
    public Transform[] groundWaypoints;
    [Tooltip("Waypoints above the ground which enemis that can use ramps travel to")]
    public Transform[] platformWaypoints;
    //[Tooltip("Waypoints that flying enemies can move between")]
    //public Transform[] skyWaypoints;
    [Tooltip("Temporary waypoints for the cars to drive towards")]
    public Transform[] roadCheckpoints;

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
    private bool highlightQuitButton;

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
        soundSource.PlayOneShot(playerAudio.gameOver);
        state = GameState.Loss;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0;
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
    
    // Instantiates a game object and destroys it after a specified time
    public IEnumerator LoadAndDestroyObject(GameObject obj, Vector3 position, Quaternion rotation, float seconds)
    {
        var newObject = Instantiate(obj, position, rotation);
        yield return new WaitForSeconds(seconds);
        GameObject.Destroy(newObject);
    }

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
        //HandleInput();
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
            else
                musicSource.Stop();
        }
        else if (state == GameState.Running && !musicSource.isPlaying)
            musicSource.Play();

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
        waveTime = Time.timeSinceLevelLoad - previousElapsedTime;

        // Check that all enemies have been spawned and there are none left in the wave
        if (enemiesSpawned == waves[waveNumber - 1].enemiesInWave.Length && enemiesLeft == 0)
        {
            playerAudio.PlaySound(playerAudio.waveEnd);
            NextWave(); // load the next wave
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

    void NextWave()
    {
        waveNumber++;
        enemiesSpawned = 0;        
        previousElapsedTime = Time.time;

        // WIN GAME CONDITION
        if (waveNumber > waves.Length)
        {
            gui.ShowLevelComplete();
            playerAudio.PlaySound(playerAudio.levelComplete);
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