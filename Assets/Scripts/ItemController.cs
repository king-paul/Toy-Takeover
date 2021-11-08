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

    private PlayerController player;
    private PlayerSound audio;

    private void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        audio = GameObject.FindWithTag("Player").GetComponent<PlayerSound>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            ApplyItemEffect();
            audio.PlaySound(pickupSound);
            Destroy(this.gameObject);
        }
        
    }

    private void ApplyItemEffect()
    {
        switch(statChanged)
        {
            case PlayerStats.Health: player.AddHealth(increaseAmount);
                break;

            case PlayerStats.JetpackFuel: player.AddJetpackFuel(increaseAmount);
                break;

            case PlayerStats.Armour: player.AddArmour(increaseAmount);
                break;

            case PlayerStats.CrossBowAmmo: player.AddAmmo(0, (int)increaseAmount);
                break;

            case PlayerStats.WaterGunAmmo: player.AddAmmo(1, (int)increaseAmount);
                break;

            case PlayerStats.LaserAmmo: player.AddAmmo(2, (int)increaseAmount);
                break;
        }
    }

}