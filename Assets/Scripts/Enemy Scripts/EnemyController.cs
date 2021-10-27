using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public enum EnemyState { Follow, Attack, Aim };

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour
{
    // public variables
    public Color hitColor = Color.red;
    public ParticleSystem damageParticles;

    // variables
    Transform player;
    NavMeshAgent agent;
    Rigidbody rigidbody;
    float distance;
    EnemyState state;

    [SerializeField]
    float maxHealth = 30;
    float curHealth;

    [SerializeField]
    bool flyingEnemy = false;

    Material material;

    public EnemyState State { get => state; set => state = value; }
    public float Distance { get => (player.position - transform.position).magnitude; }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;

        if(!flyingEnemy)
            agent = GetComponent<NavMeshAgent>();  
        
        state = EnemyState.Follow;
        curHealth = maxHealth;

        material = GetComponent<Renderer>().material;
        if(material == null)
            material = GetComponentInChildren<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        //if (game.State != GameState.Running)
            //return;

        //Debug.Log("Distance: " + Distance);

        if (curHealth <= 0)
        {
            GameObject.Destroy(this.gameObject);
            return;
        }

        if (state == EnemyState.Follow)
        {
            if (!flyingEnemy)                
                agent.destination = player.position;
        }
        else {
            if (!flyingEnemy)
                agent.destination = transform.position;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Projectile")
        {
            // flash enemy red
            StartCoroutine(FlashColor());            

            curHealth -= 10;

            //Debug.Log("Enemy hit by projectile");
        }
    }

    IEnumerator FlashColor()
    {
        material = GetComponentInChildren<Renderer>().material;
        material.color = hitColor;
        yield return new WaitForSeconds(0.1f);
        GetComponent<Renderer>().material.color = Color.white;
    }
}