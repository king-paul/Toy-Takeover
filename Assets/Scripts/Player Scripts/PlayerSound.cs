using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerSound : MonoBehaviour
{
    private AudioSource playerAudio;

    [Header("PlayerMovement")]
    public AudioClip playerFootsteps;
    public AudioClip playerLanding;
    public AudioClip jetpackThrust;
    public AudioClip jetpackRunout;

    [Header("PlayerDamage")]
    public AudioClip healthDamage;
    public AudioClip armourDamage;

    // Start is called before the first frame update
    void Start()
    {
        playerAudio = GetComponent<AudioSource>();
    }

    public void PlaySound(AudioClip clip)
    {
        if(clip != null)
            playerAudio.PlayOneShot(clip);
    }
}
