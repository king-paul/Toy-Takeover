using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    private Transform camera;
    public float travelSpeed = 1000;
    [SerializeField]
    float damage = 20f;

    private Rigidbody rigidBody;
    private float boundary = 30;

    // properties
    public float Damage { get => damage; }

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        camera = GameObject.FindWithTag("MainCamera").transform;

        //Debug.Log("Firing in direction: " + camera.forward);
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
            PlayerController player = other.GetComponent<PlayerController>();
            player.TakeDamage(damage);
        }

        GameObject.Destroy(this.gameObject);
    }

}