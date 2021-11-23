using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public enum EnemyState { Patrol, Follow, Aim, Attack, Damage, Dead };

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour
{
    // public variables
    //public Color hitColor = Color.red;
    public EnemyState initialState;

    [Header("Damage Particles")]
    public GameObject damageParticles;
    public GameObject deathParticles;

    [SerializeField]
    float maxHealth = 30;
    float curHealth;

    // collision damage
    [Header("Collision damage")]
    public float collisionDamage = 1;
    public float framesPerDamage = 5;

    [Header("Animation Speed")]
    [Range(0.1f, 5)]
    public float movementSpeed = 1;
    [Range(0.1f, 5)]
    public float attackSpeed = 1;
    [Range(0.1f, 5)]
    public float damageSpeed = 1;
    [Range(0.1f, 5)]
    public float deathSpeed = 1;

    [SerializeField]
    bool enviromentalHazard = false;
    //bool flyingEnemy = false;

    // private variables
    Transform player;
    NavMeshAgent agent;
    Rigidbody rigidbody;
    Animator animator;
    EnemyState state;
    Material material;
    GameManager game;
    EnemySound audio;
    private float distance;

    // properties and public functions
    public EnemyState State { get => state; set => state = value; }
    public float Distance { get => (player.position - transform.position).magnitude; }
    public void TakeDamage(float amount){ 

        curHealth -= amount; // reduces health

        ChangeState(EnemyState.Damage);

        // switch enemy to follow after taking damage if patrolling
        if (state == EnemyState.Patrol)
        {
            ChangeState(EnemyState.Follow);
        }

        if (curHealth > 0)
        {
            audio.PlaySound(audio.damageSounds);
            PlayDamageParticles();
        }

    }

    private void Awake()
    {
        state = initialState;

        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        audio = GetComponent<EnemySound>();
        game = GameObject.Find("GameManager").GetComponent<GameManager>();
        player = GameObject.FindWithTag("Player").transform;

        curHealth = maxHealth;

        material = GetComponent<Renderer>().material;
        if(material == null)
            material = GetComponentInChildren<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Distance: " + Distance);

        if (curHealth <= 0 && state != EnemyState.Dead)
        {
            // if the enemy is not a car reduce the number in the wave
            if(!enviromentalHazard)
                game.KillEnemy();
            
             ChangeState(EnemyState.Dead);

            return;
        }

        if (state == EnemyState.Follow)
        {
            agent.destination = player.position;            
        }
    }

    // Not currently used
    public void ChangeState(EnemyState newState)
    {
        state = newState;

        switch(state)
        {
            case EnemyState.Patrol:
                audio.PlayMoveSound();
                animator.SetTrigger("Walk");
                break;

            case EnemyState.Follow:
                audio.PlayMoveSound();
                //audio.StopPlaying(0);

                agent.isStopped = false;
                agent.destination = player.position;

                animator.ResetTrigger("Attack");
                animator.SetTrigger("Walk");
                animator.speed = movementSpeed;
            break;

            case EnemyState.Attack:                
                agent.isStopped = true;

                animator.ResetTrigger("Walk");
                animator.SetTrigger("Attack");
                animator.speed = attackSpeed;
                break;

            case EnemyState.Aim:
                agent.isStopped = true;
                animator.ResetTrigger("Attack");
                animator.SetTrigger("Walk");
                animator.speed = movementSpeed;
            break;

            case EnemyState.Damage:
                agent.isStopped = true; // stops the agent form moving

                // update animation state to the damage state
                animator.ResetTrigger("Walk");
                animator.ResetTrigger("Attack");
                animator.SetTrigger("Damage");
                animator.speed = damageSpeed;
            break;

            case EnemyState.Dead:
                agent.isStopped = true;
                audio.PlaySound(audio.deadSounds);
                animator.SetTrigger("Death");
                animator.speed = movementSpeed;

                GetComponent<CapsuleCollider>().enabled = false; // turns off collisions
            break;
        }

    }

    public void DestroyEnemy()
    {
        if (deathParticles != null)
            Instantiate(deathParticles, transform.position, transform.rotation);

        GameObject.Destroy(this.gameObject);
    }

    public void PlayDamageParticles()
    {
        // play damage particles
        if (damageParticles != null)        
           Instantiate(damageParticles, transform);        
    }

    public void StartFollowing()
    {
        animator.speed = 1;
        ChangeState(EnemyState.Follow);
    }

}