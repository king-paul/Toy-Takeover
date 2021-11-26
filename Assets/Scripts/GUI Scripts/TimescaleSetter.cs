using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimescaleSetter : MonoBehaviour
{
    public float timescaleToSet;
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = timescaleToSet;
    }
}