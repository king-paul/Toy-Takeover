using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class EnemySound : MonoBehaviour
{
    private AudioSource[] enemyAudio;

    public AudioClip moveSound;
    public AudioClip[] attackSounds;
    public AudioClip[] damageSounds;
    public AudioClip[] deadSounds;
    
    void Awake()
    {
        enemyAudio = GetComponents<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        enemyAudio = GetComponents<AudioSource>();        
    }

    public void PlaySound(AudioClip clip)
    {
        if (clip != null)
            enemyAudio[0].PlayOneShot(clip);
    }

    public void PlaySound(AudioClip clip, int index, bool loop)
    {
        if (clip != null && index > 0 && index <= 2 &&
            (!enemyAudio[index].isPlaying || enemyAudio[index].clip != clip))
        {
            enemyAudio[index].clip = clip;
            enemyAudio[index].loop = loop;
            enemyAudio[index].Play();
        }
    }

    public void PlaySound(AudioClip[] clips)
    {
        if (clips.Length > 0)
        {
            AudioClip randomSound = clips[Random.Range(0, clips.Length - 1)];
            Debug.Log("Playing Enemy Sound: " + randomSound);
            enemyAudio[0].PlayOneShot(randomSound);
        }
    }

    // stops specified audio source if it is playing something
    public void StopPlaying(int index)
    {
        if (enemyAudio[index].isPlaying)
            enemyAudio[index].Stop();
    }
}
