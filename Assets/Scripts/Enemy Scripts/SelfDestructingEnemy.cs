using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestructingEnemy : MonoBehaviour
{
    EnemyController enemy;

    private void Start()
    {
        enemy = GetComponent<EnemyController>();
    }

    // destroys the enemy when colliding with the player
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            GameObject.Destroy(this.gameObject);

            // damage the player
            var player = other.gameObject.GetComponent<PlayerController>();
            player.TakeDamage(enemy.collisionDamage);
        }
    }
}
