using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public enum EnemyState { Patrol, Follow, Attack, Aim };

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
    bool flyingEnemy = false;

    // private variables
    Transform player;
    NavMeshAgent agent;
    Rigidbody rigidbody;    
    EnemyState state;
    Material material;
    GameManager game;
    EnemySound audio;
    private float distance;

    // properties and public functions
    public EnemyState State { get => state; set => state = value; }
    public float Distance { get => (player.position - transform.position).magnitude; }
    public void TakeDamage(float amount){ 
        curHealth -= amount;

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

        if (!flyingEnemy)
            agent = GetComponent<NavMeshAgent>();
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
            game.KillEnemy();
            game.PlayRandomSound(audio.deadSounds, 1);
            GameObject.Destroy(this.gameObject);
            return;
        }

        // play sound while enemy is moving
        if((state == EnemyState.Patrol || state == EnemyState.Follow))
        {
            audio.PlayMoveSound();
        }

        if (state == EnemyState.Follow)
        {
            //if (!flyingEnemy) 
            audio.StopPlaying(0);
            agent.isStopped = false;
            agent.destination = player.position;
        }
        else if(state == EnemyState.Attack)
        {
            //if (!flyingEnemy)
            agent.destination = transform.position;
            agent.isStopped = true;
        }
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