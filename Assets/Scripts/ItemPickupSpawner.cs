using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickupSpawner : MonoBehaviour
{
    // public variables
    public GameObject[] itemPickups;
    public bool spawnRandomPickup = true;
    [Range(-180, 180)]
    public float spawnAngle = 0;

    GameManager game;

    public void SpawnPickup()
    {
        game = GameObject.Find("GameManager").GetComponent<GameManager>();
        Quaternion rotation = Quaternion.Euler(0, spawnAngle, 0);

        // if there is already an item pickup on the spawnpoint don't spawn anything
        if (transform.childCount > 0)
            return;

        // if random is checked select a random pickup at the start of the wave
        if(spawnRandomPickup)
        {
            int randomPick = Random.Range(0, itemPickups.Length-1);
            Instantiate(itemPickups[randomPick], transform.position, rotation, transform); 
        }
        else
        {
            if (game.WaveNumber < itemPickups.Length)
                Instantiate(itemPickups[game.WaveNumber - 1], transform.position, rotation, transform);
            else
                Instantiate(itemPickups[itemPickups.Length - 1], transform.position, rotation, transform);
        }

    }


}
