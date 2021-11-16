using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyParticleSystem : MonoBehaviour
{
    ParticleSystem[] particles;

    void Start()
    {
        particles = GetComponentsInChildren<ParticleSystem>();
    }

    void Update()
    {
        // destroy the game object
        if (AllParticlesFinished())
            GameObject.Destroy(this.gameObject);
    }

    private bool AllParticlesFinished()
    {
        foreach(ParticleSystem p in particles)
        {
            if (p.isPlaying)
                return false;
        }

        return true;
    }
}
