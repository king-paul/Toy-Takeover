using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    public float travelSpeed = 1000;

    private Rigidbody rigidBody;
    private float boundary = 30;
    public GameObject firer;

    // properties
    //public float Damage { get => damage; set => damage = value; }
    public GameObject Firer { set => firer = value; }

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        rigidBody.velocity = transform.forward * travelSpeed;// * Time.deltaTime;
        //Debug.Log("Bullet Velocity: " + rigidBody.velocity);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (other.gameObject != firer)
            {
                PlayerController player = other.GetComponent<PlayerController>();
                var enemy = firer.GetComponent<EnemyController>();                

                player.TakeDamage(enemy.DamageDealt);
            }
        }
        // projectile hits an enemy
        else if(other.gameObject.CompareTag("Enemy"))
        {
            EnemyController enemy = other.GetComponent<EnemyController>();
            DollyCartEnemy dollyEnemy = other.GetComponent<DollyCartEnemy>();
            PlayerController player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();

            if (enemy != null && enemy.State != EnemyState.Dead)
            {
                enemy.TakeDamage(player.CurrentWeapon.damagePerHit, true);
            }
            else if (dollyEnemy != null)
            {
                dollyEnemy.ApplyHit();
            }
                
        }

        // check for Ignore Raycast layer or ViewModel layer
        if (other.gameObject.layer != 2 && other.gameObject.layer != 12 
            && other.gameObject != firer)
            GameObject.Destroy(this.gameObject);
    }
}