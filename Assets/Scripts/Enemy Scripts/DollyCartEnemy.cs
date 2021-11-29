using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DollyCartEnemy : MonoBehaviour
{
    [SerializeField]
    float collisionDamage = 35;
    [Tooltip("Determines if the enemy will destroy itself upon collison with the player")]
    [SerializeField]
    bool destroyOnCollision = true;
    public GameObject destructionParticles;

    //EnemyController enemy;
    CinemachineDollyCart dollyCart;
    EnemySound enemyAudio;
    GameManager game;

    private void Start()
    {
        //enemy = GetComponent<EnemyController>();
        dollyCart = GetComponentInParent<CinemachineDollyCart>();
        enemyAudio = GetComponent<EnemySound>();
        game = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void Update()
    {
        if (game.State != GameState.Running && dollyCart.isActiveAndEnabled)
            dollyCart.enabled = false;
    }

    // destroys the enemy when colliding with the player
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            // damage the player
            var player = other.gameObject.GetComponent<PlayerController>();
            player.TakeDamage(collisionDamage);

            game.PlayRandomSound(enemyAudio.deadSounds, 1);

            if (destroyOnCollision)
                GameObject.Destroy(this.gameObject);
        }
        
    }

    public void ApplyHit()
    {
        enemyAudio.PlaySound(enemyAudio.damageSounds, false);
    }
}
