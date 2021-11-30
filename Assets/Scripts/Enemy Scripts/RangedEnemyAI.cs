using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyController))]
public class RangedEnemyAI : MonoBehaviour
{
    public Transform gun;
    public Transform gunEnd;
    public Transform projectilePrefab;
    [Range(1, 200)]
    public float viewDistance = 100f;
    //[Range(0, 2)]
    //public float firingDelay = 0.25f;

    [SerializeField][Range(1, 100)]
    float viewAngle = 45f;
    [SerializeField][Range(1, 20)]
    float turnSpeed = 10f;
    public ParticleSystem firingParticles;

    private Transform player;
    private RaycastHit rayToPlayer;
    private RaycastHit gunRay;    
    private EnemyController controller;
    private EnemySound audio;

    // vectors use to create a vision cone
    Vector3 directionToTarget;
    Vector3 shootDirection;


    private float distanceToTarget;

    GameManager game;

    public void PlayParticles()
    {
        RangedEnemyAI rangedEnemy;
    }

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<EnemyController>();
        audio = GetComponent<EnemySound>();
        player = GameObject.FindWithTag("Player").transform;
        shootDirection = transform.forward;
        game = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(game.State != GameState.Running)
            return;

        //Get the direction from the enemy to the player and normalize it
        directionToTarget = (player.position - transform.position).normalized;
        distanceToTarget = (player.position - transform.position).magnitude;        

        Debug.DrawRay(transform.position, directionToTarget * viewDistance, Color.blue); // draws line to player
        Debug.DrawRay(gunEnd.position, gunEnd.forward * viewDistance, Color.red); // draws gun ray

        if (controller.State == EnemyState.Follow)
        {
            if (firingParticles.isPlaying)
                firingParticles.Stop();

            if (!PlayerBehindWall() && distanceToTarget <= viewDistance)
            {
                controller.ChangeState(EnemyState.Aim);
            }
        }

        // Aim and fire
        if (controller.State == EnemyState.Aim || controller.State == EnemyState.Attack)
        {
            AimWeapon();

            // if the gun lines up with the player switch to attack state
            // otherwise switch back to aim state
            if (Physics.Raycast(gunEnd.position, gun.forward, out gunRay) &&
                gunRay.transform.tag == "Player")
            {
                if(controller.State == EnemyState.Aim)
                    controller.ChangeState(EnemyState.Attack);
            }
            else if (controller.State == EnemyState.Attack)
            {
                controller.ChangeState(EnemyState.Aim);
            }

            // if the player is no longer visible start following
            if (PlayerBehindWall() || distanceToTarget > viewDistance)
                controller.ChangeState(EnemyState.Follow);
        }

        // start particles when aiming
        if (controller.State == EnemyState.Aim && !firingParticles.isPlaying)
            firingParticles.Play();

        // stop particles while firing
        if (controller.State == EnemyState.Attack && firingParticles.isPlaying)
            firingParticles.Stop();
    }

    private void Fire()
    {
        AimWeapon();

        if (Physics.Raycast(gunEnd.position, gunEnd.forward, out gunRay) &&
           gunRay.transform.tag == "Player")
        {
            controller.ChangeState(EnemyState.Attack);
        }
        else if(PlayerBehindWall() || distanceToTarget > viewDistance)
        {            
            controller.ChangeState(EnemyState.Follow);
            //Debug.Log("Switching to follow state");
        }
            
    }

    private void FireProjectile()
    {
        var projectile = Instantiate(projectilePrefab, gunEnd.position, gun.rotation);
        //firingParticles.Pause();        

        projectile.GetComponent<ProjectileController>().Firer = transform.gameObject;
        audio.PlaySound(audio.attackSounds, true);
    }

    private void AimWeapon()
    {
        // rotate enemy towards the player
        Vector3 faceDirection = Vector3.RotateTowards(transform.forward, directionToTarget,
            Mathf.Deg2Rad * turnSpeed * Time.deltaTime, 0);
        transform.rotation = Quaternion.LookRotation(faceDirection);

        ///gun.rotation = Quaternion.LookRotation(faceDirection);
        //gun.localRotation = Quaternion.Euler(gun.rotation.x, gun.rotation.y+1.5f, gun.rotation.z);

        // calculate the vector that the weapon needs to be aiming at
        shootDirection = (player.position - gunEnd.position).normalized;
        shootDirection.Normalize();

        // restrict angle of aiming
        if (Vector3.Angle(shootDirection, transform.forward) >= 30)
        shootDirection = transform.forward;

        Vector3 gunDirection = Vector3.RotateTowards(gun.forward, shootDirection,
            Mathf.Deg2Rad * turnSpeed * Time.deltaTime, 0);
        gun.rotation = Quaternion.LookRotation(gunDirection);
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
