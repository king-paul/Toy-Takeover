using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestructingEnemy : MonoBehaviour
{
    // destroys the enemy when colliding with the player
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            GameObject.Destroy(this.gameObject);

            // damage the player
        }
    }
}
