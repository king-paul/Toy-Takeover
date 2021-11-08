using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class EnemySound : MonoBehaviour
{
    private AudioSource enemySound;

    public AudioClip moveSound;
    public AudioClip attackSound;
    public AudioClip damageSound;
    public AudioClip deadSound;
    
    void Awake()
    {
        enemySound = GetComponent<AudioSource>();
    }

    public void PlaySound(AudioClip clip)
    {
        if (clip != null)
            enemySound.PlayOneShot(clip);
    }
}
