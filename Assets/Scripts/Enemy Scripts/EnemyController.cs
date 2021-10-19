using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public enum EnemyState { Follow, Attack, Aim };
public class EnemyController : MonoBehaviour
{
    // public variables
    public Color hitColor = Color.red;

    // variables
    Transform player;
    NavMeshAgent agent;    
    float distance;
    EnemyState state;

    [SerializeField]
    float maxHealth = 30;
    float curHealth;

    Material material;

    public EnemyState State { get => state; set => state = value; }
    public float Distance { get => (player.position - transform.position).magnitude; }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        state = EnemyState.Follow;
        curHealth = maxHealth;

        material = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Distance: " + Distance);

        if (curHealth <= 0)
        {
            GameObject.Destroy(this.gameObject);
            return;
        }

        if (state == EnemyState.Follow)
        {
            agent.destination = player.position;
        }
        else {
            agent.destination = transform.position;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Projectile")
        {
            // flash enemy red
            StartCoroutine(FlashColor());

            curHealth -= 5;

            //Debug.Log("Enemy hit by projectile");
        }
    }

    IEnumerator FlashColor()
    {
        material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        GetComponent<Renderer>().material.color = Color.white;
    }
}