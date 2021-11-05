using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolEnemyAI : MonoBehaviour
{
    Transform spawnpoint;
    Transform[] patrolRoute;
    NavMeshAgent agent;

    private int waypoint = 0;

    public void SetSpawnPoint(Transform spawnpoint)
    {
        this.spawnpoint = spawnpoint;        
    }

    // Start is called before the first frame update
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();    
    }

    private void Start()
    {
        patrolRoute = spawnpoint.GetComponent<PatrolRoute>().wayPoints;
        agent.SetDestination(patrolRoute[waypoint].position);
    }

    // Update is called once per frame
    void Update()
    {
        //if(agent.transform.position == patrolRoute[waypoint].position)
        //{
        //    if (waypoint < patrolRoute.Length - 1)
        //        waypoint++;
        //    else
        //        waypoint = 0;

        //    agent.SetDestination(patrolRoute[waypoint].position);
        //}
        
    }    

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.transform == patrolRoute[waypoint])
        {
            Debug.Log("Agent has reached waypoint: " + waypoint);

            if (waypoint < patrolRoute.Length - 1)
                waypoint++;
            else
                waypoint = 0;

            agent.SetDestination(patrolRoute[waypoint].position);
        }
    }

}
