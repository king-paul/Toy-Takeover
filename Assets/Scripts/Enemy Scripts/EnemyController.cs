using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public enum EnemyState { Patrol, Follow, Attack, Dead };

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
        animator.SetTrigger("Damage"); // plays animation

        // switch enemy to follow after taking damage if patrolling
        if (state == EnemyState.Patrol)
        {
            state = EnemyState.Follow;
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

        if (curHealth <= 0)
        {
            // if the enemy is not a car reduce the number in the wave
            if(!enviromentalHazard)
                game.KillEnemy();

            animator.SetTrigger("Death");
            game.PlayRandomSound(audio.deadSounds, 1);

            GameObject.Destroy(this.gameObject);
            return;
        }

        // needs to be updated
        if((state == EnemyState.Patrol || state == EnemyState.Follow))
        {
            audio.PlayMoveSound();
            animator.SetBool("Moving", true);
            animator.SetBool("Attacking", false);
        }

        if (state == EnemyState.Follow)
        {
            audio.StopPlaying(0);
            agent.isStopped = false;
            agent.destination = player.position;
            animator.SetTrigger("Walk");
        }
        else if(state == EnemyState.Attack)
        {
            agent.destination = transform.position;
            agent.isStopped = true;
            animator.SetTrigger("Attack");
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
                animator.SetBool("Moving", true);
                animator.SetBool("Attacking", false);
            break;

            case EnemyState.Follow:
                audio.PlayMoveSound();
                audio.StopPlaying(0);
                agent.isStopped = false;

                agent.destination = player.position;
                animator.SetTrigger("Walk");
            break;

            case EnemyState.Attack:
                agent.destination = transform.position;
                agent.isStopped = true;
                animator.SetTrigger("Attack");
                animator.SetBool("Walking", false);
                animator.SetBool("Attacking", true);
                break;

            case EnemyState.Dead:
                agent.isStopped = false;
                animator.SetTrigger("Death");
                game.PlayRandomSound(audio.deadSounds, 1);
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

    //play death particles when the enemy dies
    private void OnDestroy()
    {
        if (deathParticles != null)        
            Instantiate(deathParticles, transform.position, transform.rotation);
    }

}