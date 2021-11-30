using System;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawner
{
    public Transform spawnPoint;
    public float spawnTimeElapsed;
    public bool respawning;

    public CarSpawner(Transform spawnPoint, float spawnTimeElapsed, bool respawning)
    {
        this.spawnPoint = spawnPoint;
        this.spawnTimeElapsed = spawnTimeElapsed;
        this.respawning = respawning;
    }

    public void Reset()
    {
        spawnTimeElapsed = 0;
        respawning = false;
    }
}
