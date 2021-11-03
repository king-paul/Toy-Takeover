using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public Transform firingPoint;
    public Weapon weaponObject;

    private Camera fpsCam;
    private LineRenderer laserLine;
    private RaycastHit hit;
    private float nextFire;

    private GameManager game;
    private int curAmmo;

    // public functions and properties
    public int Ammo { get => curAmmo; }

    public void AddAmmo(int amount) 
    { 
        curAmmo += amount;

        if (curAmmo > weaponObject.maxAmmo)
            curAmmo = weaponObject.maxAmmo;
    }

    #region unity functions
    // Start is called before the first frame update
    void Start()
    {
        game = GameObject.Find("GameManager").GetComponent<GameManager>();
        fpsCam = GetComponentInParent<Camera>();
        laserLine = GetComponentInChildren<LineRenderer>();
        curAmmo = weaponObject.startingAmmo;
    }

    // Update is called once per frame
    void Update()
    {
        // draw debug line on scene view
        //Debug.DrawRay(rayOrigin, fpsCam.transform.forward * weaponRange, Color.green);

        if (game.State != GameState.Running)
            return;

        if (weaponObject.GetType() == typeof(ProjectileWeapon))
        {
            ProjectileWeapon weapon = (ProjectileWeapon)weaponObject;

            // handle single shot weapon
            if (!weapon.rapidFire && Input.GetButtonDown("Fire1") && Time.time > nextFire)
            {
                nextFire = Time.time + weapon.shotDelayTime;

                if (curAmmo > 0)
                {
                    Instantiate(weapon.projectilePrefab, firingPoint.position, firingPoint.rotation);
                    curAmmo--;
                }
            }

            // handle machine gun weapon
            else if (weapon.rapidFire && Input.GetButton("Fire1") && Time.time > nextFire)
            {
                nextFire = Time.time + weapon.shotDelayTime;

                if (curAmmo > 0)
                {
                    Instantiate(weapon.projectilePrefab, firingPoint.position, firingPoint.rotation);
                    curAmmo--;
                }
                    
            }
        }

        // handle laser weapon
        if (weaponObject.GetType() == typeof(LaserWeapon))
        {
            LaserWeapon weapon = (LaserWeapon) weaponObject;

            if (Input.GetButton("Fire1") && curAmmo > 0)
            {
                FireRaycast();                

                if (Time.time > nextFire)
                {
                    curAmmo--;
                    nextFire = Time.time + weapon.shotDelay;
                }
            }
            else
                laserLine.enabled = false;
        }

    }
    #endregion

    void FireRaycast()
    {
        LaserWeapon weapon = (LaserWeapon) weaponObject;

        // Create a vector at the center of our camera's viewport
        Vector3 rayOrigin = fpsCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));

        laserLine.enabled = true;

        // Set the start position
        laserLine.SetPosition(0, firingPoint.position); 

        // Check if our raycast has hit anything and if it did handle what it hit
        if (Physics.Raycast(rayOrigin, fpsCam.transform.forward, out hit, weapon.laserRange))
        {
            laserLine.SetPosition(1, hit.point);
            HandleRaycastHit(hit.transform.gameObject);
        }
        else
        {
            // fire laser until range has been reached
            laserLine.SetPosition(1, rayOrigin + (fpsCam.transform.forward) * weapon.laserRange);
        }
    }

    void HandleRaycastHit(GameObject obj)
    {
        if(obj.tag == "Enemy")
        {
            EnemyController enemy = obj.GetComponent<EnemyController>();

            if (Time.time > nextFire)
            {
                enemy.TakeDamage(weaponObject.damagePerHit);
                enemy.PlayLaserParticles();

                /*Debug.Log("The raycast has hit an enemy and dealt " + 
                    weaponObject.damagePerHit + " damage");*/
            }
        }
    }
}