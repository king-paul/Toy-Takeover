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

    [Header("Video Settings")]
    public Toggle vSync;
    public Toggle motionBlur;

    [Header("Camera Settings")]
    public Slider sensetivitySlider;

    private float dB;

    private float cameraSensetivity = 1;

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
        // set volume sliders
        masterSlider.value = PlayerPrefs.GetFloat("MasterVolume", defaultMasterVolume);
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", defaultMusicVolume);
        soundFxSlider.value = PlayerPrefs.GetFloat("SoundEffectsVolume", defaultSoundFXVolume);
        ambienceSlider.value = PlayerPrefs.GetFloat("AmbienceVolume", defaultAmbienceVolume);

        // set checkboxes
        int vSyncValue = PlayerPrefs.GetInt("VSync", 1);
        if(vSyncValue == 0)
            vSync.isOn = false;
        else
            vSync.isOn = true;

        int motionBlurValue = PlayerPrefs.GetInt("MotionBlur", 1);
        if (motionBlurValue == 0)
            motionBlur.isOn = false;
        else
            motionBlur.isOn = true;

        // set camera sensetivity slider
        sensetivitySlider.value = PlayerPrefs.GetFloat("CameraSensetivity", 1);
    }

    #region volume Controls
    public void SetMasterVolume()
    {
        float dB = Mathf.Log10(masterSlider.value) * 20;
        mixer.SetFloat("Master", dB);        
    }

    public void SetMusicVolume()
    {
        float dB = Mathf.Log10(musicSlider.value) * 20;
        mixer.SetFloat("Music", dB);        
    }

    public void SetSoundVolume()
    {
        float dB = Mathf.Log10(soundFxSlider.value) * 20;
        mixer.SetFloat("SoundEffects", dB);        
    }

    public void SetAmbienceVolume()
    {
        float dB = Mathf.Log10(ambienceSlider.value) * 20;
        mixer.SetFloat("Ambience", dB);
        PlayerPrefs.SetFloat("AmbienceVolume", ambienceSlider.value);
    }
    #endregion

    #region video options
    public void ToggleVSync()
    {
        if (QualitySettings.vSyncCount == 1)
            QualitySettings.vSyncCount = 0;
        else
            QualitySettings.vSyncCount = 1;
    }

    public void ToggleMotionBlur()
    {
        GameManager game = GameObject.Find("GameManager").GetComponent<GameManager>();
        game.MotionBlur = motionBlur.isOn;
    }
    #endregion

    #region controls options
    public void SetCameraSensetivity(bool inMainScene)
    {
        cameraSensetivity = sensetivitySlider.value;

        if (inMainScene)
        {
            PlayerMovement player = GameObject.Find("Player").GetComponent<PlayerMovement>();
            player.CameraSensetivty = cameraSensetivity;
        }
    }
    #endregion

    public void SaveSettings()
    {
        // Save volume settings
        PlayerPrefs.SetFloat("MasterVolume", masterSlider.value);
        PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
        PlayerPrefs.SetFloat("SoundEffectsVolume", soundFxSlider.value);

        // save video settings
        PlayerPrefs.SetInt("VSync", QualitySettings.vSyncCount);
        if(motionBlur.isOn)
            PlayerPrefs.SetInt("MotionBlur", 1);
        else
            PlayerPrefs.SetInt("MotionBlur", 0);

        // save controls settings
        PlayerPrefs.SetFloat("CameraSensetivity", cameraSensetivity);

        Debug.Log("Options have been saved.");
    }

}