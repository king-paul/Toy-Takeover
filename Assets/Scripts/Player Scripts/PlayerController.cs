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

    // private variables
    private float currentHealth;
    private float currentArmour = 0;
    private float currentFuel = 0;

    private GameManager gameManager;
    private GunController gun;

    // properties
    public float CurrentFuel { get => currentFuel; }

    // Awake is called as soon s the script is run
    void Awake()
    {
        currentHealth = maxHealth;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHealth <= 0)
        {
            gameManager.Die();
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
    }

}
