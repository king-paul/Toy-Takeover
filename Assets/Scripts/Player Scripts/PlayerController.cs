using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //public variables
    public float maxHealth = 100;
    public float maxArmour = 40;
    public float maxFuel = 4;
    [Range(0, 2)]
    public float fuelDrainSpeed = 1;
    public GameObject[] weapons;
    public Transform spawnPoint;
    public float fallOffDamage = 10f;

    // private variables
    private float currentHealth;
    private float currentArmour = 0;
    private float currentFuel = 0;
    private bool hitFloor;

    private GameManager game;
    private GunController gun;

    private int weaponNum = 0;
    private int prevWeaponNum;

    // properties
    public float Health { get => currentHealth; }
    public float Fuel { get => currentFuel; }

    // Awake is called as soon s the script is run
    void Awake()
    {
        currentHealth = maxHealth;
        hitFloor = false;
        game = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void Start()
    {
        weapons[weaponNum].SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (game.State == GameState.Running && currentHealth <= 0)
        {
            game.Die();
        }

        // get mouse scroll from player
        if(Input.mouseScrollDelta.y != 0)
        {
            prevWeaponNum = weaponNum;

            if (Input.mouseScrollDelta.y == -1)
                ++weaponNum;                

            if (Input.mouseScrollDelta.y == 1)
                --weaponNum;
                
            SwitchWeapons();
        }
        
    }

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
        currentHealth -= amount;

        if (currentHealth < 0)
            currentHealth = 0;
    }

    public void AddArmour(float amount)
    {
        if (currentArmour > maxArmour)
            currentArmour = maxArmour;
    }

    public void AddAmmo(int weapon, float amount)
    {

    }

    public void DrainFuel()
    {
        currentFuel -= fuelDrainSpeed * Time.deltaTime;
    }

    private void SwitchWeapons()
    {
        if (weaponNum < 0)
            weaponNum = weapons.Length - 1;

        if (weaponNum >= weapons.Length)
            weaponNum = 0;

        // switch weapon models
        weapons[prevWeaponNum].SetActive(false);
        weapons[weaponNum].SetActive(true);        

        Debug.Log("Selected weapon " + weaponNum);
    }
   
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        /*
        if(hit.gameObject.tag == "Item")
        {
            ItemController item = hit.gameObject.GetComponent<ItemController>();

            GameObject.Destroy(hit.gameObject);
            //Debug.Log("Player has picked up an item");
        }
        */

        // send player to spawnpoint when they collide with floor
        // and subtract health
        if (hit.gameObject.layer == LayerMask.NameToLayer("Boundary") &&
            hit.gameObject.CompareTag("KillZone"))
        {
            if (!hitFloor)
            {
                currentHealth -= fallOffDamage;
                hitFloor = true;

                Invoke("ResetHit", 0.5f); // prevents more than one collision
            
                transform.position = spawnPoint.position;
                transform.rotation = spawnPoint.rotation;
            }
        }

        // projectile hits player
        //if(hit.gameObject.CompareTag("Projectile"))
        //{
        //    currentHealth -= 10f;
        //}
    }

    void ResetHit()
    {
        hitFloor = false;
    }

}