using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Spawn
{
    [Tooltip("The enemy prefab to be spawned in the wave")]
    public GameObject enemy;
    [Tooltip("The spawnpoint that the enemy is spawned at")]
    public Transform spawnPoint;
    [Tooltip("The number of seconds after the start of the wave that the enemy is spawned")]
    public float timePeriod;

    [HideInInspector]
    public bool hasSpawned = false;
}

public class SpawnManager : MonoBehaviour
{
    public Spawn[] wave1;
    public Spawn[] wave2;
    public Spawn[] wave3;
    public Spawn[] wave4;
    public Spawn[] wave5;

    private int waveNumber = 1;
    private int enemiesLeft = 0;
    private int enemiesSpawned = 0;
    private Spawn[] currentWave;

    private GameManager game;

    // Start is called before the first frame update
    void Start()
    {
        game = GameObject.Find("GameManager").GetComponent<GameManager>();
        currentWave = wave1;
    }

    // Update is called once per frame
    void Update()
    {
        if (waveNumber > 5)
            return;

        foreach(Spawn spawn in currentWave)
        {
            if(!spawn.hasSpawned && Time.time >= spawn.timePeriod)
            {
                Instantiate(spawn.enemy, spawn.spawnPoint.position, spawn.spawnPoint.rotation);
                spawn.hasSpawned = true;
                enemiesSpawned++;
                enemiesLeft++;

                Debug.Log("Enemies Spawned: " + enemiesSpawned +
                          ", Enemies in Scene " + enemiesLeft);
            }
        }

        // Check that all enemies have been spawned and there are none left in the wave
        if(enemiesSpawned == currentWave.Length && enemiesLeft == 0)
        {
            NextWave();
        }
    }

    void NextWave()
    {
        waveNumber++;

        switch (waveNumber)
        {
            case 1: currentWave = wave1;
                break;
            case 2: currentWave = wave2;
                break;
            case 3: currentWave = wave3;
                break;
            case 4: currentWave = wave4;
                break;
            case 5: currentWave = wave5;
                break;
        }

        enemiesSpawned = 0;
    }
}
