using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyController))]
public class RangedEnemyAI : MonoBehaviour
{
    public Transform gun;
    public Transform gunEnd;
    public Transform projectile;
    [Range(1, 200)]
    public float viewDistance = 100f;
    [Range(0, 2)]
    public float firingDelay = 0.25f;

    [SerializeField][Range(1, 100)]
    float viewAngle = 45f;
    [SerializeField][Range(1, 100)]
    float turnSpeed = 10f;
    [SerializeField][Range(1, 100)]
    float aimSpeed = 30f;

    private Transform player;
    private RaycastHit rayToPlayer;
    private RaycastHit gunRay;
    private bool firing;
    private EnemyController controller;

    // vectors use to create a vision cone
    Vector3 directionToTarget;

    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<EnemyController>();
        player = GameObject.FindWithTag("Player").transform;
        timer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //if (game.State != GameState.Running)
            //return;

        //Get the direction from the enemy to the player and normalize it
        directionToTarget = (player.position - transform.position).normalized;

        float distanceToTarget = (player.position - transform.position).magnitude;

        //DrawVisionCone();

        Debug.DrawRay(transform.position, directionToTarget * viewDistance, Color.blue); // draws line to player
        Debug.DrawRay(gunEnd.position, gunEnd.forward * viewDistance, Color.red); // draws gun ray

        switch (controller.State)
        {
            case EnemyState.Follow:
                if (!PlayerBehindWall() && distanceToTarget <= viewDistance)//PlayerInVision())
                {
                    //Debug.Log("Enemy can see player");

                    controller.State = EnemyState.Attack;
                    Debug.Log("Switching to attack state");
                }
                break;

            case EnemyState.Aim: Aim();
                break;

            case EnemyState.Attack: Fire();
                break;
        }

    }

    // perform aim state
    void Aim()
    {
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, directionToTarget, 
            Mathf.Deg2Rad * aimSpeed * Time.deltaTime, 0);        
        transform.rotation = Quaternion.LookRotation(newDirection);

        // if the gun is pointing at the player switch to the attack state
        if (Physics.Raycast(gunEnd.position, gunEnd.forward, out gunRay) &&
           gunRay.transform.tag == "Player")
        {
            Debug.Log("Switching to attack state");
            controller.State = EnemyState.Attack;
            Invoke("FireProjectile", firingDelay);
            return;
        }

        // check that the raycast from the enemy is hitting something other than the player
        if (Physics.Raycast(transform.position, directionToTarget, out rayToPlayer)
            && rayToPlayer.transform.tag != "Player")
        {
            // If the object is the player switch to the attack state
            controller.State = EnemyState.Follow;
            Debug.Log("Switching to follow state");
        }
        
    }

    void Fire()
    {
        AimWeapon();

        if (Physics.Raycast(gunEnd.position, gunEnd.forward, out gunRay) &&
           gunRay.transform.tag == "Player")
        {
            // if the time delay has been reached fire another shot
            timer += Time.deltaTime;
            if (timer > firingDelay)
            {
                Instantiate(projectile, gunEnd.position, gunEnd.rotation);
                timer = 0;
            }
        }

        //if (!PlayerInVision())
        //{
        //    controller.State = EnemyState.Follow;
        //    Debug.Log("Switching to follow state");
        //    return;
        //}

        //// check if the blue line is hitting the player
        //if (!PlayerInVision() ||
        //    Physics.Raycast(gunEnd.position, gunEnd.forward, out gunRay) &&  gunRay.transform.tag != "Player" && 
        //    gunRay.transform.gameObject.layer != LayerMask.NameToLayer("Boundary"))
        if(PlayerBehindWall())
        {
            //controller.State = EnemyState.Aim;
            controller.State = EnemyState.Follow;
            Debug.Log("Switching to follow state");
        }
    }

    void AimWeapon()
    {
        // Turn towards the player
        Vector3 faceDirection = Vector3.RotateTowards(transform.forward, directionToTarget,
            Mathf.Deg2Rad * turnSpeed * Time.deltaTime, 0);
        transform.rotation = Quaternion.LookRotation(faceDirection);

        //Vector3 aimDirection = (player.position - gunEnd.position).normalized;

        Vector3 gunDirection = Vector3.RotateTowards(gun.forward, directionToTarget,
            Mathf.Deg2Rad * aimSpeed * Time.deltaTime, 0);

        gun.rotation = Quaternion.LookRotation(gunDirection);
    }

    bool PlayerInVision()
    {
        // convert the cone's field of view into the same unit type that is returned by a dot product
        float coneValue = Mathf.Cos((Mathf.Deg2Rad * viewAngle) * 0.5f);
        float dotProuct = Vector3.Dot(directionToTarget, transform.forward);

        // check if target is inside the cone
        if (dotProuct >= coneValue)
        {
            // check if player is obscured by a wall. If not return true
            if( Physics.Raycast(transform.position, directionToTarget, out rayToPlayer)
            && rayToPlayer.transform.tag != "Player")
                return true;
        }

        return false;
    }

    bool PlayerBehindWall()
    {
        if (Physics.Raycast(transform.position, directionToTarget, out rayToPlayer)
            && rayToPlayer.transform.gameObject.layer == LayerMask.NameToLayer("Level"))
        {
            return true;
        }
            
        return false;
    }

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

        for(int i=0; i< coneEnds.Length; i++)
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

}
