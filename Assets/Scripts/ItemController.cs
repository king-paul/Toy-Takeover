using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum PlayerStats { Health, JetpackFuel, Armour, CrossBowAmmo, WaterGunAmmo, LaserAmmo }

public class ItemController : MonoBehaviour
{
    public PlayerStats statChanged;
    public float increaseAmount;
    public AudioClip pickupSound;
    public AudioClip alreadyFullSound;
    public string pickupMessage;
    public string alreadyFullMassage;
    public bool collectAtFullCapacity = false;

    private GUIController gui;
    private PlayerController player;
    private PlayerSound audio;

    private void Start()
    {
        gui = GameObject.Find("GameManager").GetComponent<GUIController>();
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        audio = GameObject.FindWithTag("Player").GetComponent<PlayerSound>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (ApplyItemEffect())
            {                
                gui.ShowPickupMessage(pickupMessage);
                audio.PlaySound(pickupSound);
                Destroy(this.gameObject);
            }
            else
            {
                gui.ShowPickupMessage(alreadyFullMassage);
                audio.PlaySound(alreadyFullSound);
            }
            
        }
        
    }

    private bool ApplyItemEffect()
    {
        switch(statChanged)
        {
            case PlayerStats.Health:
                if (player.Health < player.maxHealth || collectAtFullCapacity)
                {
                    player.AddHealth(increaseAmount);
                    return true;
                }
                break;

            case PlayerStats.JetpackFuel:
                if (player.Fuel < player.maxFuel || collectAtFullCapacity)
                {
                    player.AddJetpackFuel(increaseAmount);
                    return true;
                }
                break;

            case PlayerStats.Armour:
                if (player.Armour < player.maxArmour || collectAtFullCapacity)
                {
                    player.AddArmour(increaseAmount);
                    return true;
                }                
                break;

            case PlayerStats.CrossBowAmmo:
                if (player.AddAmmo(0, (int)increaseAmount) || collectAtFullCapacity) 
                    return true;
                break;

            case PlayerStats.WaterGunAmmo: 
                if(player.AddAmmo(1, (int)increaseAmount) || collectAtFullCapacity) 
                    return true;
                break;

            case PlayerStats.LaserAmmo: 
                if(player.AddAmmo(2, (int)increaseAmount) || collectAtFullCapacity) 
                    return true;
                break;
        }

        return false;

    }

}