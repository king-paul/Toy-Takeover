using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PatrolRoute
{
    public Transform[] waypoints;
    public bool twoWay;
    public bool randomizeOrder;
}


public class PatrolManager : MonoBehaviour
{
    public PatrolRoute[] patrolRoutes;

    public Transform[] GetPatrolRoutes()
    {
        int random = Random.Range(0, patrolRoutes.Length);
        if (random == patrolRoutes.Length)
            random = patrolRoutes.Length - 1;

        return patrolRoutes[random].waypoints;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
