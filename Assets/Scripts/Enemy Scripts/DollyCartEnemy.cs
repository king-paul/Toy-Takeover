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
    public ParticleSystem destructionParticles;

    // private variables    
    CinemachineDollyCart dollyCart;
    EnemySound enemyAudio;
    GameManager game;
    bool alive;

    private void Start()
    {
        //enemy = GetComponent<EnemyController>();
        dollyCart = GetComponentInParent<CinemachineDollyCart>();
        enemyAudio = GetComponent<EnemySound>();
        game = GameObject.Find("GameManager").GetComponent<GameManager>();
        alive = true;
    }

    private void Update()
    {
        if (game.State != GameState.Running && game.State != GameState.Paused 
            && dollyCart.isActiveAndEnabled)
            dollyCart.enabled = false;
        else if (game.State == GameState.Running && !dollyCart.isActiveAndEnabled)
            dollyCart.enabled = true;
    }

    // destroys the enemy when colliding with the player
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            // damage the player
            var player = other.gameObject.GetComponent<PlayerController>();
            player.TakeDamage(collisionDamage);

            if (destroyOnCollision)
            {
                game.PlayRandomSound(enemyAudio.deadSounds, 1);

                //Vector3 spawnPos = new Vector3(other.transform.position.x, other.transform.position.y,
                //    other.transform.position.z + other.transform.forward.z * 50);

                //Debug.Log("Player Position: " + other.transform.position + ", Particles position: " + spawnPos);

                Instantiate(destructionParticles, transform.position, transform.rotation);
                GameObject.Destroy(this.gameObject);
            }
            else
            {
                enemyAudio.PlaySound(enemyAudio.attackSounds, false);
            }
        }
        
    }

    public void ApplyHit()
    {
        enemyAudio.PlaySound(enemyAudio.damageSounds, false);        
    }

}
