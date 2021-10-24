using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemyAI : MonoBehaviour
{
    public float flySpeed = 10;
    public Transform[] wayPoints;

    Rigidbody rigidbody;
    Transform player;
    EnemyController controller;

    private int wayPointNum;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        player = GameObject.FindWithTag("Player").transform;
        controller = GetComponent<EnemyController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (controller.State == EnemyState.Follow)
        {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            rigidbody.velocity = directionToPlayer * flySpeed;
        }

        Debug.DrawLine(transform.position, player.position, Color.blue);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag != "Projectile")
            Destroy(this.gameObject);
    }
}
