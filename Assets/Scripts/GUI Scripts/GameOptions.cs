using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.UI;
using static TMPro.TMP_Dropdown;

public class GameOptions : MonoBehaviour
{
    // variable declaration
    [Header("Tabs and panes")]
    public Button startTab;
    public GameObject audioOptions, videoOptions;

    [Header("Audio Mixers")]
    [Header("Audio Settings")]
    public AudioMixer mixer;
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
    public TMP_Dropdown screenMode;
    public TMP_Dropdown screenResolution;
    public TMP_Dropdown graphicsQuality;
    public TMP_Dropdown vSyncCount;
    public Toggle motionBlur;

    [Header("Camera Settings")]
    public Slider sensetivitySlider;

    // private variables
    private float dB;
    private float cameraSensetivity = 1;
    private Resolution[] resolutions;
    GameManager game;

    // Start is called before the first frame update
    void Start()
    {
        resolutions = Screen.resolutions;
        game = GameObject.Find("GameManager").GetComponent<GameManager>();

        // Set audio volumes
        dB = Mathf.Log10(PlayerPrefs.GetFloat("MasterVolume")) * 20;
        mixer.SetFloat("Master", dB);
        dB = Mathf.Log10(PlayerPrefs.GetFloat("MusicVolume")) * 20;
        mixer.SetFloat("Music", dB);
        dB = Mathf.Log10(PlayerPrefs.GetFloat("SoundEffectsVolume")) * 20;
        mixer.SetFloat("SoundEffects", dB);
        dB = Mathf.Log10(PlayerPrefs.GetFloat("AmbienceVolume")) * 20;
        mixer.SetFloat("Ambience", dB);

        // set video settings
        Screen.fullScreenMode = (FullScreenMode) PlayerPrefs.GetInt("ScreenMode", 0);
        QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("Quality", 0), true);
        QualitySettings.vSyncCount = PlayerPrefs.GetInt("VSync", 1);

        // set screen resolution
        int width = PlayerPrefs.GetInt("ScreenWidth", Screen.currentResolution.width);
        int height = PlayerPrefs.GetInt("ScreenHeight", Screen.currentResolution.height);

        foreach (Resolution res in resolutions)
        {
            if(res.width == width && res.height == height)
            {
                Screen.SetResolution(res.width, res.height, Screen.fullScreenMode);
                break;
            }

        }

        // Set mition blur when in game
        if(game != null)
        {
            int motionBlurOn = PlayerPrefs.GetInt("MotionBlur", 0);

            if (motionBlurOn == 1)
                game.MotionBlur = true;
            else if (motionBlurOn == 0)
                game.MotionBlur = false;
        }

        // populate the screen resolutions dropdown  
        List<string> resolutionText = new List<string>();
        foreach (var resolution in resolutions)
        {
            //resolutionText.Add(resolution.width + " x " + resolution.height);
            //Debug.Log(resolution.ToString());
            screenResolution.options.Add(new OptionData(resolution.width + " x " + resolution.height));
        }
        //screenResolution.AddOptions(resolutionText);

    }

    public void InitGUI()
    {
        // set default selected tab
        startTab.Select();
        startTab.onClick.Invoke();

        // set volume sliders
        masterSlider.value = PlayerPrefs.GetFloat("MasterVolume", defaultMasterVolume);
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", defaultMusicVolume);
        soundFxSlider.value = PlayerPrefs.GetFloat("SoundEffectsVolume", defaultSoundFXVolume);
        ambienceSlider.value = PlayerPrefs.GetFloat("AmbienceVolume", defaultAmbienceVolume);

        // set camera sensetivity slider
        sensetivitySlider.value = PlayerPrefs.GetFloat("CameraSensetivity", 1);

        // set drop down boxes
        screenMode.SetValueWithoutNotify(PlayerPrefs.GetInt("ScreenMode", 0));
        //screenResolution.SetValueWithoutNotify(Array.IndexOf(resolutions, Screen.currentResolution));

        // set the seleted screen resolution        
        int width = PlayerPrefs.GetInt("ScreenWidth", Screen.currentResolution.width);
        int height = PlayerPrefs.GetInt("ScreenHeight", Screen.currentResolution.height);
        try
        {
            foreach (Resolution res in resolutions)
            {
                if (res.width == width && res.height == height)
                {
                    screenResolution.SetValueWithoutNotify(Array.IndexOf(resolutions, res));
                    break;
                }

            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }

        graphicsQuality.SetValueWithoutNotify(PlayerPrefs.GetInt("Quality", 2));
        vSyncCount.SetValueWithoutNotify(PlayerPrefs.GetInt("VSync", 1));

        // set motion blur checkbox
        try
        {
            int motionBlurValue = PlayerPrefs.GetInt("MotionBlur", 1);
            if (motionBlurValue == 0)
                motionBlur.isOn = false;
            else
                motionBlur.isOn = true;
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    #region tab controls
    public void OpenAudioOptions()
    {
        audioOptions.SetActive(true);
        videoOptions.SetActive(false);
    }

    public void OpenVideoOptions()
    {
        audioOptions.SetActive(false);
        videoOptions.SetActive(true);
    }
    #endregion

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
    public void SetScreenMode()
    {
        Screen.fullScreenMode = (FullScreenMode)screenMode.value;
    }

    public void SetScreenResolution() 
    {
        Screen.SetResolution(resolutions[screenResolution.value].width,
        resolutions[screenResolution.value].height, (FullScreenMode)screenMode.value);
    }

    public void SetGraphicsQuality()
    {
        QualitySettings.SetQualityLevel(graphicsQuality.value, true);
    }

    public void SetVSyncCount()
    {
        QualitySettings.vSyncCount = vSyncCount.value;
    }

    public void ToggleMotionBlur(bool inMainScene)
    {
        if(inMainScene && game != null)
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
        PlayerPrefs.SetInt("ScreenMode", screenMode.value);
        PlayerPrefs.SetInt("ScreenWidth", resolutions[screenResolution.value].width);
        PlayerPrefs.SetInt("ScreenHeight", resolutions[screenResolution.value].height);
        PlayerPrefs.SetInt("Quality", graphicsQuality.value);
        PlayerPrefs.SetInt("VSync", vSyncCount.value);

        if (motionBlur.isOn)
            PlayerPrefs.SetInt("MotionBlur", 1);
        else
            PlayerPrefs.SetInt("MotionBlur", 0);

        // save controls settings
        PlayerPrefs.SetFloat("CameraSensetivity", cameraSensetivity);        
    }

}