using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    public float travelSpeed = 1000;
    float damage = 20f;

    private Rigidbody rigidBody;
    private float boundary = 30;
    private GameObject firer;

    // properties
    public float Damage { get => damage; set => damage = value; }
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
                player.TakeDamage(damage);
            }
        }
        // projectile hits an enemy
        else if(other.gameObject.CompareTag("Enemy"))
        {
            EnemyController enemy = other.GetComponent<EnemyController>();

            if(enemy.State != EnemyState.Dead)
                enemy.TakeDamage(damage);
        }

        // check for Ignore Raycast layer or ViewModel layer
        if (other.gameObject.layer != 2 && other.gameObject.layer != 12 
            && other.gameObject != firer)
            GameObject.Destroy(this.gameObject);
    }
}