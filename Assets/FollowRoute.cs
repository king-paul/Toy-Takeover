using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class FollowRoute : MonoBehaviour
{
    [Range(0, 12)]
    public int firstCheckpoint = 1;
    NavMeshAgent agent;
    GameManager game;
    
    int checkpoint;

    // Start is called before the first frame update
    void Start()
    {
        checkpoint = firstCheckpoint;
        agent = GetComponent<NavMeshAgent>();
        game = GameObject.Find("GameManager").GetComponent<GameManager>();
        agent.SetDestination(game.roadCheckpoints[checkpoint].position);
    }

    private void OnTriggerEnter(Collider other)
    {
        // move to next checkpoint when the objet collides with one
        if (other.gameObject.transform == game.roadCheckpoints[checkpoint])
        {
            if (checkpoint < game.roadCheckpoints.Length - 1)
                checkpoint++;
            else
                checkpoint = 0;

            //Debug.Log("Driving towards: " + game.roadCheckpoints[checkpoint]);
            agent.SetDestination(game.roadCheckpoints[checkpoint].position);
        }

    }
}
