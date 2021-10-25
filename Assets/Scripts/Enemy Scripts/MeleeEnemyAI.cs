using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemyAI : MonoBehaviour
{
    [SerializeField][Range(0, 2)]
    float attackRange = 1.1f;
    //[SerializeField]
    //[Tooltip("Destroy the game object when it collides tithe the player")]
    //bool destroyOnContact = false;

    EnemyController controller;
    PlayerController player;

    [SerializeField]
    float damagePerAttack = 5f;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<EnemyController>();
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (controller.State == EnemyState.Follow)
        {
            if (controller.Distance <= attackRange)
            {
                controller.State = EnemyState.Attack;
                Invoke("AttackPlayer", 1.0f);
                return;
            } 

        }

        if (controller.State == EnemyState.Attack)
        {
            if (controller.Distance > attackRange)
            {
                controller.State = EnemyState.Follow;
                CancelInvoke();
            }
        }
    }

    public void AttackPlayer()
    {
        player.TakeDamage(damagePerAttack);

        if(controller.State == EnemyState.Attack)
            Invoke("AttackPlayer", 1.0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        /*if (destroyOnContact && other.gameObject.tag == "Player")
        {
            GameObject.Destroy(this.gameObject);

            // damage the player
        }*/
    }
}
