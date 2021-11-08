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
    public ParticleSystem projectileDamageParticles;
    public ParticleSystem laserDamageParticles;

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
    private float distance;
    EnemyState state;
    Material material;
    GameManager game;

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
            GameObject.Destroy(this.gameObject);
            return;
        }

        if (state == EnemyState.Follow)
        {
            //if (!flyingEnemy)     
            agent.isStopped = false;
            agent.destination = player.position;
        }
        else if(state == EnemyState.Attack)
        {
            //if (!flyingEnemy)
            agent.isStopped = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Projectile")
        {
            // flash enemy red
            //StartCoroutine(FlashColor());
            //PlayDamageParticles();

            //curHealth -= 10;
            //Debug.Log("Enemy hit by projectile");
        }
    }

    //IEnumerator FlashColor()
    //{
    //    material = GetComponentInChildren<Renderer>().material;
    //    material.color = hitColor;
    //    yield return new WaitForSeconds(0.1f);
    //    GetComponent<Renderer>().material.color = Color.white;
    //}

    public void PlayDamageParticles()
    {
        projectileDamageParticles.gameObject.SetActive(true);
        projectileDamageParticles.Play();
    }

    public void PlayLaserParticles()
    {
        laserDamageParticles.gameObject.SetActive(true);
        laserDamageParticles.Play();
    }

}