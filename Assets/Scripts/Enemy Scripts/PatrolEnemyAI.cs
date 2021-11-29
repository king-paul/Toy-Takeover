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
    //Transform[] patrolRoute;
    [Header("Debug Info")]
    [SerializeField]
    Transform nextWaypoint;
    [SerializeField]
    Transform[] waypoints;
    PatrolRoute patrolRoute;

    NavMeshAgent agent;
    EnemyController enemy;
    Transform player;
    GameManager game;
    PatrolManager patrolManager;

    // vision cone variables
    private Vector3 directionToTarget;
    private RaycastHit rayToPlayer;

    // other variables
    private int waypoint = 0;
    private float distanceToPlayer;
    private bool reverseOrder = false;

    public Vector3 nextWaypointPos { get => nextWaypoint.position; }

    public void SetSpawnPoint(Transform spawnpoint)
    {
        this.spawnpoint = spawnpoint;        
    }

    // Start is called before the first frame update
    void Awake()
    {        
        agent = GetComponent<NavMeshAgent>();
        game = GameObject.Find("GameManager").GetComponent<GameManager>();
        patrolManager = GameObject.Find("GameManager").GetComponent<PatrolManager>();
    }

    private void Start()
    {
        enemy = GetComponent<EnemyController>();
        player = GameObject.FindWithTag("Player").transform;
                
        patrolRoute = patrolManager.GetPatrolRoute();
        waypoints = patrolRoute.waypoints;

        if (enemy.State == EnemyState.Patrol) {           
            agent.SetDestination(waypoints[waypoint].position);

            nextWaypoint = waypoints[waypoint];
        }
    }

    // Update is called once per frame
    void Update()
    {
        distanceToPlayer = (player.position - transform.position).magnitude;

        //Get the direction from the enemy to the player and normalize it
        directionToTarget = (player.position - transform.position).normalized;

        if (useVisionCone)
        {
            DrawVisionCone();
            //Debug.DrawLine(transform.position, (transform.position+directionToTarget) * viewDistance, Color.red);
        }

        // if the enemy can see the player while it is patrolling switch to follow state
        if (enemy.State == EnemyState.Patrol)
        {
            // follow the player if in vision or they get too close
            if (useVisionCone && PlayerInVision() ||
                followWhenInRange && distanceToPlayer <= minDistanceFromPlayer)
                enemy.ChangeState(EnemyState.Follow);
        }
    }   
    
    // Causes the enemy to select a random route from the waypoints in the scene
    //private void SelectRandomRoute()
    //{
    //    float totalWaypoints = 0;
    //    int maxRange, chosenNumber;
    //    bool alreadyInRoute;

    //        if (canUseRamps)
    //            maxRange = game.groundWaypoints.Length + game.platformWaypoints.Length;
    //        else
    //            maxRange = game.groundWaypoints.Length;

    //    // selects random waypoints until the route is full
    //    while (totalWaypoints < waypointsInRoute && totalWaypoints < maxRange)
    //    {
    //        do
    //        {
    //            chosenNumber = Random.Range(0, maxRange);
    //            //Debug.Log("Chosen Number: " + chosenNumber + " of " + maxRange);

    //                // if the number is greater than a certain value select from the platform waypoints
    //                if (canUseRamps && chosenNumber > game.groundWaypoints.Length - 1)
    //                {
    //                    chosenNumber -= game.groundWaypoints.Length;
    //                    alreadyInRoute = AlreadyInRoute(game.platformWaypoints[chosenNumber]);

    //                    // break out of loop if null is detected
    //                    if (game.platformWaypoints[chosenNumber] == null)
    //                        return;

    //                    if (!preventDuplicates || !alreadyInRoute)
    //                        patrolRoute.Add(game.platformWaypoints[chosenNumber]);
    //                }
    //                else // if it is less than the value select a ground waypoint
    //                {
    //                    alreadyInRoute = AlreadyInRoute(game.groundWaypoints[chosenNumber]);

    //                    // break out of loop if null is detected
    //                    if (game.groundWaypoints[chosenNumber] == null)
    //                        return;

    //                    if (!preventDuplicates || !alreadyInRoute)
    //                        patrolRoute.Add(game.groundWaypoints[chosenNumber]);
    //                }

    //        } while (preventDuplicates && alreadyInRoute);

    //        totalWaypoints++;
    //    }

    //    // return to spawnpoint after completing the route
    //    if (includeSpawnpoint)
    //        patrolRoute.Add(spawnpoint);
    //}

    //private bool AlreadyInRoute(Transform chosenWaypoint)
    //{
    //    foreach(Transform waypoint in patrolRoute)
    //    {
    //        if (waypoint == chosenWaypoint)
    //            return true;
    //    }

    //    return false;
    //}

    private void OnTriggerEnter(Collider other)
    {
        // Update the waypoints when the enemy walks into one
        if (enemy.State == EnemyState.Patrol &&
            other.gameObject.transform == waypoints[waypoint])
        {
            Transform curWaypoint = waypoints[waypoint];

            if (!reverseOrder)
            {
                if (waypoint < waypoints.Length - 1)
                    waypoint++;
                else if(patrolRoute.twoWay)
                {
                    waypoint--;
                    reverseOrder = true;
                }
                else
                    waypoint = 0;
            }
            else
            {
                if (waypoint > 0)
                    waypoint--;
                else if (patrolRoute.twoWay)
                {
                    waypoint++;
                    reverseOrder = false;
                }
                else
                    waypoint = waypoints.Length - 1;
            }

            nextWaypoint = waypoints[waypoint];

            //Debug.Log("Agent " + this.gameObject.GetInstanceID() + " has reached waypoint: " + curWaypoint +
                      //"\nand will now move to waypoint " + nextWaypoint);
            agent.SetDestination(nextWaypoint.position);
        }

    }

    #region enemy vision functions
    private bool PlayerInVision()
    {
        // convert the cone's field of view into the same unit type that is returned by a dot product
        float coneValue = Mathf.Cos((Mathf.Deg2Rad * viewAngle) * 0.5f);
        float dotProuct = Vector3.Dot(directionToTarget, transform.forward);

        // check if target is inside the cone
        if (dotProuct >= coneValue)
        {
            // check if player is obscured by a wall. If not return true
            if (Physics.Raycast(transform.position, directionToTarget, out rayToPlayer, viewDistance)
            && rayToPlayer.transform.tag == "Player")
                return true;
        }

        return false;
    }

    // displays the vision cone of each enemy in the scene window at runtime
    // by drawing debug lines
    private void DrawVisionCone()
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
