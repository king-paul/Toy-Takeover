using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    private Transform camera;
    public Transform[] boundaries;
    public float travelSpeed = 1000;

    private Rigidbody rigidBody;
    private float boundary = 30;

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
        rigidBody.velocity = transform.forward * travelSpeed;// * Time.deltaTime, ForceMode.Impulse);
        //Debug.Log("Bullet Velocity: " +rigidBody.velocity);
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject.Destroy(this.gameObject);
    }

}
