using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerSound : MonoBehaviour
{    
    private AudioSource[] playerAudio;

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

    public bool isPlaying(int index)
    {
        if (playerAudio[index].isPlaying)
            return true;

        return false;
    }

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

    public void PlaySound(AudioClip clip, float volumeScale)
    {
        if (clip != null)
            playerAudio[0].PlayOneShot(clip, volumeScale);
    }

    public void PlaySound(AudioClip clip, int index, bool loop)
    {
        //Debug.Log("index: " + index);
        //Debug.Log("Sound playing: " + playerAudio[index].isPlaying);

        if (clip != null && index <= 2 &&
            (!playerAudio[index].isPlaying || playerAudio[index].clip != clip))
        {
            if (loop)
            {
                playerAudio[index].clip = clip;
                playerAudio[index].loop = true;                
                //Debug.Log("Playing sound");
                playerAudio[index].Play();
            }
            else
            {
                //Debug.Log("Playing sound " + clip);
                playerAudio[index].PlayOneShot(clip);
            }

        }
    }

    // stops specified audio source if it is playing something
    public void StopPlaying(int index) {
        if (playerAudio[index].isPlaying)
        {
            //Debug.Log("Stopping sound");
            playerAudio[index].Stop();
        }
    }

}