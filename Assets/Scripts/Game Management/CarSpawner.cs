using System;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawner
{
    public Transform dollyCart;
    public float spawnTimeElapsed;
    public bool respawning;
    public float position;

    public CarSpawner(Transform dollyCart, float position)
    {
        this.dollyCart = dollyCart;
        this.position = position;
        spawnTimeElapsed = 0;
        respawning = false;
    }

    public void Reset()
    {
        spawnTimeElapsed = 0;
        respawning = false;
    }
}
