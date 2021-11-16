using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Rendering;

public class PlayerController : MonoBehaviour
{
    #region variable declaration
    //public variables
    [Header("Max Stats")]
    public float maxHealth = 100;
    public float maxArmour = 40;
    public float maxFuel = 4;
    [Range(0, 2)]
    public float fuelDrainSpeed = 1;

    [Header("Weapons")]
    public GameObject[] weapons;

    [Header("Fall off and spawning")]
    public Transform spawnPoint;
    public float fallOffDamage = 10f;

    [Header("Debug Cheats")]
    public bool godMode = false;

    // private variables
    private float currentHealth = 100;
    private float currentArmour = 0;
    private float currentFuel = 0;
    private bool hitFloor;

    private GameManager game;
    private WeaponController weapon;
    private EnemyController enemy;
    private Animator animator;
    private PlayerSound audio;
    private Transform playerParticles;

    private int weaponNum = 0;
    private int prevWeaponNum;

    private int totalFrames = 0; // used to count frames from last collision damage
    private bool collidingWithEnemy = false;
    #endregion

    #region properties
    public float Health { get => currentHealth; }
    public float Fuel { get => currentFuel; }
    public int Ammo {
        get {
            weapon = weapons[weaponNum].GetComponent<WeaponController>();
            return weapon.Ammo;
        }
    }
    public int MaxAmmo
    {
        get
        {
            weapon = weapons[weaponNum].GetComponent<WeaponController>();
            return weapon.weaponObject.maxAmmo;
        }
    }
    public float Armour { get => currentArmour; }
    #endregion

    #region Unity functions
    // Awake is called as soon s the script is run
    void Awake()
    {
        currentHealth = maxHealth;
        hitFloor = false;
        game = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Start is called after awake
    private void Start()
    {
        animator = GetComponent<Animator>();
        audio = GetComponent<PlayerSound>();
        weapons[weaponNum].SetActive(true);
        playerParticles = transform.Find("ParticleSystems");


    }

    // Update is called once per frame
    void Update()
    {
        if (game.State == GameState.Running && currentHealth <= 0)
        {
            game.Die();
        }

        UpdateSwitchWeapons();
    }

    private void UpdateSwitchWeapons()
    {
        // get mouse scroll from player
        if (Input.mouseScrollDelta.y != 0)
        {
            //prevWeaponNum = weaponNum;

            if (Input.mouseScrollDelta.y == -1)
                SwitchWeapons(weaponNum + 1);

            if (Input.mouseScrollDelta.y == 1)
                SwitchWeapons(weaponNum - 1);
        }
        else if (Input.GetButtonDown("NextWeapon")) // right bumper
        {
            SwitchWeapons(weaponNum + 1);
        }
        else if (Input.GetButtonDown("PrevWeapon")) // left bumper
        {
            SwitchWeapons(weaponNum - 1);
        }

        // number keys
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SwitchWeapons(0);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            SwitchWeapons(1);
        if (Input.GetKeyDown(KeyCode.Alpha3))
            SwitchWeapons(2);

    }

    private void FixedUpdate()
    {
        // resolve collisions with enemy
        if (collidingWithEnemy)
        {
            totalFrames++;
            if (totalFrames > enemy.framesPerDamage)
            {
                TakeDamage(enemy.collisionDamage);
                totalFrames = 0;
                collidingWithEnemy = false;
            }

        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // damage the player if they collide with an enemy
        if (hit.gameObject.tag == "Enemy" && !collidingWithEnemy)
        {
            collidingWithEnemy = true;
            enemy = hit.gameObject.GetComponent<EnemyController>();

            TakeDamage(enemy.collisionDamage);
        }

        // send player to spawnpoint when they collide with floor
        // and subtract health
        if (hit.gameObject.layer == LayerMask.NameToLayer("Boundary") &&
            hit.gameObject.CompareTag("KillZone"))
        {
            if (!hitFloor)
            {                
                TakeDamage(fallOffDamage);
                hitFloor = true;
                animator.SetTrigger("PlayerTakesDamage");                

                Invoke("ResetHit", 0.5f); // prevents more than one collision

                GetComponent<CharacterController>().enabled = false;
                transform.position = spawnPoint.position;
                transform.rotation = spawnPoint.rotation;
                GetComponent<CharacterController>().enabled = true;
            }
        }
    }

    /*
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("KillZone") && !hitFloor)
        {
            currentHealth -= fallOffDamage;
            hitFloor = true;

            Invoke("ResetHit", 0.5f); // prevents more than one collision

            GetComponent<CharacterController>().enabled = false;
            transform.position = spawnPoint.position;
            transform.rotation = spawnPoint.rotation;
            GetComponent<CharacterController>().enabled = true;
        }
    }*/

    #endregion

    #region public functions
    // Setters
    public void AddJetpackFuel(float amount)
    {
        currentFuel += amount;

        if (currentFuel > maxFuel)
            currentFuel = maxFuel;
    }

    public void AddHealth(float amount)
    {
        currentHealth += amount;

        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        // if the player has armour damage the armour
        // otherwise damage the health
        if (currentArmour > 0)
        {
            currentArmour -= amount;

            // if the damage taken is greater than the current armour
            // deal remaining damage to health
            if (currentArmour < 0)
            {
                currentHealth += currentArmour;
                currentArmour = 0;
                audio.PlaySound(audio.healthDamage);
            }
            else
                audio.PlaySound(audio.armourDamage);
        }
        else
        {
            currentHealth -= amount;
            audio.PlaySound(audio.healthDamage);
        }

        if (currentHealth < 0)
        currentHealth = 0;

        animator.SetTrigger("TakeDamage");
        
    }

    public void AddArmour(float amount)
    {
        if(currentArmour < maxArmour)
            currentArmour += amount;
    }

    public bool AddAmmo(int weaponNum, int amount)
    {
        weapon = weapons[weaponNum].GetComponent<WeaponController>();        

        if(weapon.Ammo < weapon.MaxAmmo)
        {
            weapon.AddAmmo(amount);
            return true;
        }

        return false;
    }

    public void DrainFuel()
    {
        currentFuel -= fuelDrainSpeed * Time.deltaTime;
    }

    public void StopDamage()
    {
        animator.SetBool("Damage", false);
        //Debug.Log("Stopping damage animation");
    }

    public void PlayParticles(int index)
    {        
        ParticleSystem[] particles = playerParticles.GetChild(index).GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem p in particles)
            p.Play();
    }
    #endregion

    #region private functions
    private void SwitchWeapons(int number)
    {
        prevWeaponNum = weaponNum;
        weaponNum = number;

        if (weaponNum < 0)
            weaponNum = weapons.Length - 1;

        if (weaponNum >= weapons.Length)
            weaponNum = 0;

        // switch weapon models
        weapons[prevWeaponNum].SetActive(false);
        weapons[weaponNum].SetActive(true);        

        //Debug.Log("Selected weapon " + weaponNum);
    }

    private void ResetHit()
    {
        hitFloor = false;
    }
    #endregion

}