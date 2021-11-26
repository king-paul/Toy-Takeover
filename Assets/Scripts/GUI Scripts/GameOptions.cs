using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class GameOptions : MonoBehaviour
{
    [Header("Audio Mixers")]
    [Header("Audio Settings")]    
    public AudioMixer mixer;
    public AudioMixerGroup master, music, soundEffects, Ambience;
    [Header("Volume Sliders")]
    public Slider masterSlider;
    public Slider soundFxSlider, musicSlider, ambienceSlider;
    [Header("Default Volumes")]
    [Range(0, 1)]
    public float defaultMasterVolume = 0.75f;
    [Range(0, 1)]
    public float defaultMusicVolume = 0.5f;
    [Range(0, 1)]
    public float defaultSoundFXVolume = 0.75f;
    [Range(0, 1)]
    public float defaultAmbienceVolume = 0.75f;

    private float dB;

    // Start is called before the first frame update
    void Start()
    {
        //if (PlayerPrefs.HasKey("MasterVolume"))
        //    Debug.Log("Player Prefs has Key : MasterVolume Value = " +
        //        PlayerPrefs.GetFloat("MasterVolume"));
        //if (PlayerPrefs.HasKey("MusicVolume"))
        //    Debug.Log("Player Prefs has Key : MusicVolume Value = " +
        //        PlayerPrefs.GetFloat("MusicVolume"));
        //if (PlayerPrefs.HasKey("SoundEffectsVolume"))
        //    Debug.Log("Player Prefs has Key : SoundEffectsVolume Value = " +
        //        PlayerPrefs.GetFloat("SoundEffectsVolume"));
        //if (PlayerPrefs.HasKey("AmbienceVolume"))
        //    Debug.Log("Player Prefs has Key : AmbienceVolume Value = " +
        //        PlayerPrefs.GetFloat("AmbienceVolume"));

        // Set default volumes
        dB = Mathf.Log10(PlayerPrefs.GetFloat("MasterVolume")) * 20;
        mixer.SetFloat("Master", dB);
        dB = Mathf.Log10(PlayerPrefs.GetFloat("MusicVolume")) * 20;
        mixer.SetFloat("Music", dB);
        dB = Mathf.Log10(PlayerPrefs.GetFloat("SoundEffectsVolume")) * 20;
        mixer.SetFloat("SoundEffects", dB);
        dB = Mathf.Log10(PlayerPrefs.GetFloat("AmbienceVolume")) * 20;
        mixer.SetFloat("Ambience", dB);
    }

    public void InitGUI()
    {
        masterSlider.value = PlayerPrefs.GetFloat("MasterVolume", defaultMasterVolume);
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", defaultMusicVolume);
        soundFxSlider.value = PlayerPrefs.GetFloat("SoundEffectsVolume", defaultSoundFXVolume);
        ambienceSlider.value = PlayerPrefs.GetFloat("AmbienceVolume", defaultAmbienceVolume);
    }

    public void SetMasterVolume()
    {
        float dB = Mathf.Log10(masterSlider.value) * 20;
        mixer.SetFloat("Master", dB);
        PlayerPrefs.SetFloat("MasterVolume", masterSlider.value);
    }

    public void SetMusicVolume()
    {
        float dB = Mathf.Log10(musicSlider.value) * 20;

        mixer.SetFloat("Music", dB);
        PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
    }

    public void SetSoundVolume()
    {
        float dB = Mathf.Log10(soundFxSlider.value) * 20;
        mixer.SetFloat("SoundEffects", dB);
        PlayerPrefs.SetFloat("SoundEffectsVolume", soundFxSlider.value);
    }

    public void SetAmbienceVolume()
    {
        float dB = Mathf.Log10(ambienceSlider.value) * 20;
        mixer.SetFloat("Ambience", dB);
        PlayerPrefs.SetFloat("AmbienceVolume", ambienceSlider.value);
    }
}
