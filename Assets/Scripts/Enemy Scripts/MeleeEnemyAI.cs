using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemyAI : MonoBehaviour
{
    [SerializeField][Range(2, 4)]
    [Tooltip("The distance that the melee enemy attacks the player from")]
    float attackRange = 2f;
    [SerializeField][Range(1, 5)]
    [Tooltip("Number of times per second the melee enemy attacks the player")]
    float attackRate = 1;

    //[SerializeField]
    //[Tooltip("Destroy the game object when it collides tithe the player")]
    //bool destroyOnContact = false;

    GameManager game;
    EnemyController controller;
    EnemySound audio;
    PlayerController player;

    [SerializeField]
    float damagePerAttack = 5f;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<EnemyController>();
        audio = GetComponent<EnemySound>();
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        game = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (game.State != GameState.Running)
            return;

        if (controller.State == EnemyState.Follow)
        {
            if (controller.Distance <= attackRange)
            {
                controller.ChangeState(EnemyState.Attack);
                Invoke("AttackPlayer", 1 / attackRate);
                return;
            }

            //Debug.Log("Distance from player: " + controller.Distance +
            //          "Attack Range: " + attackRange);

        }

        if (controller.State == EnemyState.Attack)
        {
            if (controller.Distance > attackRange)
            {
                controller.ChangeState(EnemyState.Follow);
                CancelInvoke();
            }
        }
    }

    public void AttackPlayer()
    {
        audio.PlaySound(audio.attackSounds, true);
        player.TakeDamage(damagePerAttack);
    }

}
