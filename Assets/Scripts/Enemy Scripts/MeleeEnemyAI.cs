using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemyAI : MonoBehaviour
{
    [SerializeField]
    float attackRange = 1.1f;
    [SerializeField]
    [Tooltip("Destroy the game object when it collides tithe the player")]
    bool destroyOnContact = false;

    EnemyController controller;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<EnemyController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (controller.State == EnemyState.Follow)
        {
            if (controller.Distance <= attackRange)
            {
                controller.State = EnemyState.Attack;
                return;
            } 

        }

        if (controller.State == EnemyState.Attack)
        {
            if (controller.Distance > attackRange)
                controller.State = EnemyState.Follow;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (destroyOnContact && other.gameObject.tag == "Player")
        {
            GameObject.Destroy(this.gameObject);

            // damage the player
        }
    }
}
