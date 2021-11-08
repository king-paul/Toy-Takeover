using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolEnemyAI : MonoBehaviour
{
    // adjustable variables
    [Header("Enemy Vision")]
    [SerializeField]
    bool useVisionCone = true;
    [SerializeField][Range(1, 200)]
    float viewDistance = 100f;
    [SerializeField][Range(1, 100)]
    float viewAngle = 45f;

    [Header("Enemy Hearing")]    
    [SerializeField]
    bool followWhenInRange = true;
    [SerializeField]
    [Range(3, 20)]
    float minDistanceFromPlayer = 3f;

    // Game objects and components
    Transform spawnpoint;
    Transform[] patrolRoute;
    NavMeshAgent agent;
    EnemyController enemy;
    Transform player;

    // vision cone variables
    private Vector3 directionToTarget;
    private RaycastHit rayToPlayer;

    // other variables
    private int waypoint = 0;
    private float distanceToPlayer;

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
        enemy = GetComponent<EnemyController>();
        player = GameObject.FindWithTag("Player").transform;

        patrolRoute = spawnpoint.GetComponent<PatrolRoute>().wayPoints;

        if (enemy.State == EnemyState.Patrol)        
            agent.SetDestination(patrolRoute[waypoint].position);

    }

    // Update is called once per frame
    void Update()
    {
        distanceToPlayer = (player.position - transform.position).magnitude;

        //Get the direction from the enemy to the player and normalize it
        directionToTarget = (player.position - transform.position).normalized;

        if(useVisionCone)
            DrawVisionCone();

        // if the enemy can see the player while it is patrolling switch to follow state
        if (enemy.State == EnemyState.Patrol)
        {
            // follow the player if in vision or they get too close
            if (useVisionCone && PlayerInVision() ||
                followWhenInRange && distanceToPlayer <= minDistanceFromPlayer)
                enemy.State = EnemyState.Follow;
        }
    }    

    private void OnTriggerEnter(Collider other)
    {
        if(enemy.State == EnemyState.Patrol &&
            other.gameObject.transform == patrolRoute[waypoint])
        {
            //Debug.Log("Agent has reached waypoint: " + waypoint);

            if (waypoint < patrolRoute.Length - 1)
                waypoint++;
            else
                waypoint = 0;

            agent.SetDestination(patrolRoute[waypoint].position);
        }

    }

    #region enemy vision functions
    bool PlayerInVision()
    {
        // convert the cone's field of view into the same unit type that is returned by a dot product
        float coneValue = Mathf.Cos((Mathf.Deg2Rad * viewAngle) * 0.5f);
        float dotProuct = Vector3.Dot(directionToTarget, transform.forward);

        // check if target is inside the cone
        if (dotProuct >= coneValue)
        {
            // check if player is obscured by a wall. If not return true
            if (Physics.Raycast(transform.position, directionToTarget, out rayToPlayer, viewDistance)
            && rayToPlayer.transform.tag != "Player")
                return true;
        }

        return false;
    }

    // displays the vision cone of each enemy in the scene window at runtime
    // by drawing debug lines
    void DrawVisionCone()
    {
        float radians = Mathf.Deg2Rad * (viewAngle * 0.5f);

        //Create ends of the lines as unit length vectors
        Vector3[] coneEnds = {
            new Vector3(-Mathf.Sin(radians), 0, Mathf.Cos(radians)), // left
            new Vector3(0, -Mathf.Sin(radians), Mathf.Cos(radians)), // top
            new Vector3(Mathf.Sin(radians), 0, Mathf.Cos(radians)), // right            
            new Vector3(0, Mathf.Sin(radians), Mathf.Cos(radians)), // bottom
        };

        for (int i = 0; i < coneEnds.Length; i++)
        {
            // Rotate the lines to match the rotation of the character
            coneEnds[i] = transform.rotation * coneEnds[i];

            // Scale the lines out to their correct length
            coneEnds[i] *= viewDistance;

            // Translate the lines if the location of the character
            coneEnds[i] += transform.position;

            //Draw the line
            Debug.DrawLine(transform.position, coneEnds[i], Color.green);
        }

        // draw conections
        Debug.DrawLine(coneEnds[0], coneEnds[1], Color.green);
        Debug.DrawLine(coneEnds[1], coneEnds[2], Color.green);
        Debug.DrawLine(coneEnds[2], coneEnds[3], Color.green);
        Debug.DrawLine(coneEnds[3], coneEnds[0], Color.green);
    }
    #endregion

}
