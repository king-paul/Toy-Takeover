using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType { Grunt, Robot, Car};
public enum SpawnPoint { element0, element1, element2, element3, element4, element5 };

[System.Serializable]
public class EnemySpawn
{
    [Tooltip("The enemy prefab to be spawned in the wave")]
    public EnemyType enemy;
    [Tooltip("The numbar the the enemy is spawned at")]
    public SpawnPoint spawnPoint;
    [Tooltip("The number of seconds after the start of the wave that the enemy is spawned")]
    [Range(0, 60)]
    public float timePeriod;

    [HideInInspector]
    public bool hasSpawned = false;
}

[CreateAssetMenu(fileName = "Wave", menuName = "EnemyWave", order = 2)]
public class EnemyWave : ScriptableObject
{
    public EnemySpawn[] enemiesInWave;
}
