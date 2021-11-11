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
    private EnemySound audio;

    // vectors use to create a vision cone
    Vector3 directionToTarget;
    Vector3 shootDirection;

    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<EnemyController>();
        audio = GetComponent<EnemySound>();
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

        Debug.DrawRay(transform.position, directionToTarget * viewDistance, Color.blue); // draws line to player
        Debug.DrawRay(gunEnd.position, shootDirection * viewDistance, Color.red); // draws gun ray

        switch (controller.State)
        {
            case EnemyState.Follow:
                if (!PlayerBehindWall() && distanceToTarget <= viewDistance)//PlayerInVision())
                {
                    //Debug.Log("Enemy can see player");

                    controller.State = EnemyState.Aim;
                    //Debug.Log("Switching to attack state");
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
        shootDirection = (player.position - gun.position).normalized;
        shootDirection.Normalize();

        // restrict angle of aiming
        if (Vector3.Angle(shootDirection, transform.forward) >= 30)
            shootDirection = transform.forward;

        //Vector3 newDirection = Vector3.RotateTowards(transform.forward, shootDirection, 
        //    Mathf.Deg2Rad * aimSpeed * Time.deltaTime, 0);        
        //gun.rotation = Quaternion.LookRotation(shootDirection);

        gun.forward = shootDirection;

        // if the gun is pointing at the player switch to the attack state
        if (Physics.Raycast(gunEnd.position, shootDirection, out gunRay) &&
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

        if (Physics.Raycast(gunEnd.position, shootDirection, out gunRay) &&
           gunRay.transform.tag == "Player")
        {
            // if the time delay has been reached fire another shot
            timer += Time.deltaTime;
            if (timer > firingDelay)
            {
                Instantiate(projectile, gunEnd.position, gunEnd.rotation);
                audio.PlaySound(audio.attackSound);
                timer = 0;
            }
        }

        if(PlayerBehindWall())
        {
            //controller.State = EnemyState.Aim;
            controller.State = EnemyState.Follow;
            Debug.Log("Switching to follow state");
        }
    }

    void AimWeapon()
    {
        shootDirection = (player.position - gun.position).normalized;
        shootDirection.Normalize();

        // restrict angle of aiming
        if (Vector3.Angle(shootDirection, transform.forward) >= 30)
            shootDirection = transform.forward;

        // Turn towards the player
        Vector3 faceDirection = Vector3.RotateTowards(transform.forward, directionToTarget,
            Mathf.Deg2Rad * turnSpeed * Time.deltaTime, 0);
        transform.rotation = Quaternion.LookRotation(faceDirection);

        //Vector3 aimDirection = (player.position - gunEnd.position).normalized;

        //Vector3 gunDirection = Vector3.RotateTowards(gun.forward, directionToTarget,
            //Mathf.Deg2Rad * aimSpeed * Time.deltaTime, 0);

        //gun.rotation = Quaternion.LookRotation(gunDirection);
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

}
