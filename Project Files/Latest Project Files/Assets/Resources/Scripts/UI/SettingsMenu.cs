using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    #region Singleton

    public static SettingsMenu instance;

    #endregion

    [Header("Deals with all the functions for the settings menu")]
    public AudioMixer mainAudioMixer;

    [Header("The audio sliders")]
    public Slider MainAudioSlider;
    public Slider SFAudioSlider;
    public Slider MusicAudioSlider;

    [Header("The different Mesh Settings for View Distance")]
    public MeshSettings[] meshSettings;

    [Header("The Post Processing GameObjects")]
    public GameObject basicEffects;
    public GameObject goodEffects;

    [Header("The camera")]
    public Camera cam;

    // Called at the beginning of the game
    void Awake()
    {
        // Set the Vsync count to 0
        QualitySettings.vSyncCount = 0;

        // Set the instance to us
        instance = this;
    }

    // Called after Awake
    void Start()
    {
        // Set all the slider values
        MainAudioSlider.value = SavingManager.SaveData.MainAudioLevel;
        SFAudioSlider.value = SavingManager.SaveData.SFAudioLevel;
        MusicAudioSlider.value = SavingManager.SaveData.MusicAudioLevel;

        // Set all the Audio Levels
        mainAudioMixer.SetFloat("MainAudioLevel", SavingManager.SaveData.MainAudioLevel);
        mainAudioMixer.SetFloat("SFLevel", SavingManager.SaveData.SFAudioLevel);
        mainAudioMixer.SetFloat("MusicLevel", SavingManager.SaveData.MusicAudioLevel);

        // Set the FPS target level
        Application.targetFrameRate = SavingManager.SaveData.FPSLimit;

        // Set the other graphic settings
        SetVDLevel(SavingManager.SaveData.vd);
        SetPPLevel(SavingManager.SaveData.pp);

        SetAALevel(SavingManager.SaveData.aa);
        SetHDRLevel(SavingManager.SaveData.hdr);
        SetDynamicResolution(SavingManager.SaveData.dr);
    }

    #region BasicChangeFuncs

    // Set the Main Audio Level
    public void SetMainVolume(float volume)
    {
        mainAudioMixer.SetFloat("MainAudioLevel", volume);
        SavingManager.SaveData.MainAudioLevel = volume;
    }

    // Set the SF Audio Level
    public void SetSFVolume(float volume)
    {
        mainAudioMixer.SetFloat("SFLevel", volume);
        SavingManager.SaveData.SFAudioLevel = volume;
    }

    // Set the Music Level
    public void SetMusicVolume(float volume) 
    {
        mainAudioMixer.SetFloat("MusicLevel", volume);
        SavingManager.SaveData.MusicAudioLevel = volume;
    }

    // Set the FPS level
    public void SetFPSLevel(int value)
    {
        Application.targetFrameRate = value;
        SavingManager.SaveData.FPSLimit = value;
    }

    // Change if we use AA
    public void SetAALevel(bool aa) { cam.allowMSAA = aa; SavingManager.SaveData.aa = aa; }
    // Change if we use HDR
    public void SetHDRLevel(bool hdr) { cam.allowHDR = hdr; SavingManager.SaveData.hdr = hdr; }
    // Change if we use Dynamic Resolution
    public void SetDynamicResolution(bool dr) { cam.allowDynamicResolution = dr; SavingManager.SaveData.dr = dr; }

    // Set the AA Level
    public void SetVDLevel(int level)
    {
        SavingManager.SaveData.vd = level;

        TerrainGenerator.instance.meshSettings = meshSettings[level];
        TerrainGenerator.instance.UpdateViewDist();
        TerrainGenerator.instance.ResetChunks();
    }

    public void SetPPLevel(int level)
    {
        SavingManager.SaveData.pp = level;
        switch (level)
        {
            case 0:
                basicEffects.SetActive(false);
                goodEffects.SetActive(false);
                break;
            case 1:
                basicEffects.SetActive(true);
                goodEffects.SetActive(false);
                break;
            case 2:
                basicEffects.SetActive(false);
                goodEffects.SetActive(true);
                break;
            default:
                basicEffects.SetActive(false);
                goodEffects.SetActive(true);
                break;
        }
    }

    #endregion
}