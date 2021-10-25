using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GunType
{
    SingleShot, RapidFire, Laser
}

public class GunController : MonoBehaviour
{
    public GameObject bulletPrefab;
    public GameObject gluePrefab;
    public Transform gunEnd;

    [SerializeField]
    float gunDamage = 1;
    [SerializeField][Range(0, 1)]
    float shotDelayTime = .25f; // the amount of time inbetween shots
    [SerializeField]
    float weaponRange = 50f;
    [SerializeField]
    GunType gunType;

    private Camera fpsCam;
    private WaitForSeconds shotDuration;
    private LineRenderer laserLine;
    private RaycastHit hit;
    private Vector3 rayOrigin;
    private float nextFire;

    private GameManager game;

    // Start is called before the first frame update
    void Start()
    {
        fpsCam = GetComponentInChildren<Camera>();
        laserLine = GetComponentInChildren<LineRenderer>();
        shotDuration = new WaitForSeconds(0.05f);

        game = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        // draw debug line on scene view
        //Debug.DrawRay(rayOrigin, fpsCam.transform.forward * weaponRange, Color.green);

        if (game.State != GameState.Running)
            return;

        switch (gunType)
        {
            case GunType.SingleShot:                
                if (Input.GetButtonDown("Fire1") && Time.time > nextFire)
                {
                    nextFire = Time.time + shotDelayTime;
                    Instantiate(bulletPrefab, gunEnd.position, gunEnd.rotation);
                }
            break;

            case GunType.RapidFire:
                if (Input.GetButton("Fire1") && Time.time > nextFire)
                {
                    nextFire = Time.time + shotDelayTime;
                    Instantiate(gluePrefab, gunEnd.position, gunEnd.rotation);
                }
            break;

            case GunType.Laser:
                if (Input.GetButton("Fire1"))
                {
                    FireRaycast();
                }
                
                if(Input.GetMouseButtonUp(0))
                {
                    laserLine.enabled = false;
                }                
            break;
        }

    }

    void FireRaycast()
    {
        // Create a vector at the center of our camera's viewport
        Vector3 rayOrigin = fpsCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));

        laserLine.enabled = true;

        // Set the start position
        laserLine.SetPosition(0, gunEnd.position); 

        // Check if our raycast has hit anything and if it did handle what it hit
        if (Physics.Raycast(rayOrigin, fpsCam.transform.forward, out hit, weaponRange))
        {
            laserLine.SetPosition(1, hit.point);
            //HandleHit();
        }
        else
        {
            // fire laser until range has been reached
            laserLine.SetPosition(1, rayOrigin + (fpsCam.transform.forward) * weaponRange);
        }
    }

    private IEnumerator ShotDelay()
    {
        laserLine.enabled = true;
        yield return shotDuration;
        laserLine.enabled = false;
    }

    void HandleHit()
    {

    }
}