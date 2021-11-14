using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerSound : MonoBehaviour
{    
    private AudioSource[] playerAudio;
    //private AudioSource loopingAudio1, loopingAudio2;

    [Header("PlayerMovement")]
    public AudioClip playerRunning;
    public AudioClip playerJump;
    public AudioClip playerLanding;
    public AudioClip jetpackThrust;
    public AudioClip jetpackRunout;

    [Header("PlayerDamage")]
    public AudioClip healthDamage;
    public AudioClip armourDamage;

    [Header("Game States")]
    public AudioClip gameOver;
    public AudioClip waveEnd;
    public AudioClip levelComplete;

    // Start is called before the first frame update
    void Start()
    {
        playerAudio = GetComponents<AudioSource>();
        playerAudio[1].volume = 2;
    }

    public void PlaySound(AudioClip clip)
    {
        if(clip != null)
            playerAudio[0].PlayOneShot(clip);        
    }

    public void PlaySound(AudioClip clip, int index, bool loop)
    {
        if (clip != null && index > 0 && index <= 2 &&
            (!playerAudio[index].isPlaying || playerAudio[index].clip != clip))
        {
            playerAudio[index].clip = clip;
            playerAudio[index].loop = loop;      
            playerAudio[index].Play();
        }
    }

    // stops specified audio source if it is playing something
    public void StopPlaying(int index) { 
        if(playerAudio[index].isPlaying)
            playerAudio[index].Stop(); 
    }

}