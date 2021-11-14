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
        shootDirection = transform.forward;
    }

    // Update is called once per frame
    void Update()
    {
        //Get the direction from the enemy to the player and normalize it
        directionToTarget = (player.position - transform.position).normalized;
        float distanceToTarget = (player.position - transform.position).magnitude;        

        Debug.DrawRay(transform.position, directionToTarget * viewDistance, Color.blue); // draws line to player
        Debug.DrawRay(gunEnd.position, gunEnd.forward * viewDistance, Color.red); // draws gun ray

        switch (controller.State)
        {
            case EnemyState.Follow:
                if (!PlayerBehindWall() && distanceToTarget <= viewDistance)
                {
                    controller.State = EnemyState.Attack;
                }
                break;

            case EnemyState.Attack: Fire();
                break;
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
                Instantiate(projectile, gunEnd.position, gun.rotation);
                audio.PlaySound(audio.attackSound);
                timer = 0;
            }
        }
        else if(PlayerBehindWall())
        {
            //controller.State = EnemyState.Aim;
            controller.State = EnemyState.Follow;
            Debug.Log("Switching to follow state");
        }
            
    }

    void AimWeapon()
    {
        // rotate enemy towards the player
        Vector3 faceDirection = Vector3.RotateTowards(transform.forward, directionToTarget,
            Mathf.Deg2Rad * turnSpeed * Time.deltaTime, 0);
        transform.rotation = Quaternion.LookRotation(faceDirection);

        // calculate the vector that the weapon needs to be aiming at
        shootDirection = (player.position - gunEnd.position).normalized;
        shootDirection.Normalize();

        // restrict angle of aiming
        if (Vector3.Angle(shootDirection, transform.forward) >= 30)
            shootDirection = transform.forward;

        gun.forward = shootDirection; // get the gun to face the player
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
