using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    #region veriable declaration
    public Weapon weaponObject;
    public Transform firingPoint;
    public GameObject emissionEffect;

    [Header("Sound Effects")]
    public AudioClip[] fireSounds;

    private Camera fpsCam;
    private RaycastHit hit;
    private float nextFire;
    private bool fired = false;

    private GameManager game;
    private int curAmmo;
    private PlayerSound playerAudio;
    #endregion

    // public functions and properties
    public int Ammo { get => curAmmo; }
    public int MaxAmmo { get => weaponObject.maxAmmo; }

    public void AddAmmo(int amount) 
    { 
        curAmmo += amount;

        if (curAmmo > weaponObject.maxAmmo)
            curAmmo = weaponObject.maxAmmo;
    }

    public bool isFiring { get => fired; }

    #region unity functions
    // Start is called before the first frame update
    void Start()
    {
        game = GameObject.Find("GameManager").GetComponent<GameManager>();
        fpsCam = GetComponentInParent<Camera>();        
        playerAudio = GameObject.FindWithTag("Player").GetComponent<PlayerSound>();
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
            if (!weapon.rapidFire && FirePressedOnce() && Time.time > nextFire)
            {
                FireProjectileWeapon();
            }
            // handle machine gun weapon
            else if (weapon.rapidFire && FirePressed() && Time.time > nextFire)
            {                
                FireProjectileWeapon();
            }
        }

        // handle laser weapon
        if (weaponObject.GetType() == typeof(LaserWeapon))
        {
            FireLaserWeapon();
        }

    }
    #endregion

    void FireProjectileWeapon()
    {
        ProjectileWeapon weapon = (ProjectileWeapon)weaponObject;

        nextFire = Time.time + weapon.shotDelayTime;        

        if (curAmmo > 0)
        {
            var projectile = Instantiate(weapon.projectilePrefab, firingPoint.position, firingPoint.rotation);
            // set the object who fired the projectile
            projectile.GetComponent<ProjectileController>().Firer = transform.root.gameObject;

            if (emissionEffect != null)
            {
                ParticleSystem[] particles = emissionEffect.GetComponentsInChildren<ParticleSystem>();
                foreach (ParticleSystem p in particles)
                    p.Play();
            }

            playerAudio.PlaySound(fireSounds[Random.Range(0, fireSounds.Length - 1)]);
            curAmmo--;
        }
    }

    void FireLaserWeapon()
    {
        LaserWeapon weapon = (LaserWeapon)weaponObject;

        if (FirePressed() && curAmmo > 0)
        {
            FireRaycast();

            if (Time.time > nextFire)
            {
                curAmmo--;
                nextFire = Time.time + weapon.shotDelay;
                playerAudio.PlaySound(fireSounds[Random.Range(0, fireSounds.Length - 1)], 2, true);
            }
        }
        else
        {
            //emissionEffectLine.enabled = false;
            emissionEffect.SetActive(false);
            playerAudio.StopPlaying(2);
        }
    }

    void FireRaycast()
    {
        LaserWeapon weapon = (LaserWeapon) weaponObject;

        // Create a vector at the center of our camera's viewport
        Vector3 rayOrigin = fpsCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
        emissionEffect.SetActive(true);

        // Check if our raycast has hit anything and if it did handle what it hit
        if (Physics.Raycast(rayOrigin, fpsCam.transform.forward, out hit, weapon.laserRange))
        {
            //laserLine.SetPosition(1, hit.point);
            HandleRaycastHit(hit.transform.gameObject);
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

                /*Debug.Log("The raycast has hit an enemy and dealt " + 
                    weaponObject.damagePerHit + " damage");*/
            }
        }
    }

    private bool FirePressed()
    {
        if (Input.GetButton("Fire") || Input.GetAxis("Fire") != 0)
        {
            fired = true;
            return true;
        }

        fired = false;
        return false;
    }

    private bool FirePressedOnce()
    {        
        if ((Input.GetButtonDown("Fire") || Input.GetAxis("Fire") == 1) && !fired)
        {
            fired = true;
            return true;
        }

        if (Input.GetButtonUp("Fire") || Input.GetAxis("Fire") == 0)
            fired = false;

        return false;
    }

}