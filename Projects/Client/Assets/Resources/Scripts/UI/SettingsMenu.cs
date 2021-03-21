using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.PostProcessing;

public class SettingsMenu : MonoBehaviour
{
    [Header("UI Components")]
    public AudioMixer AudioMixer;

    [Space]

    public Slider MainAudioSlider;
    public Slider SFAudioSlider;
    public Slider MusicAudioSlider;

    [Space]

    public Slider RenderDistance;
    public Text RenderDistanceT;

    [Space]

    public Toggle LQGeneration;

    [Space]

    public Slider FramerateSlider;
    public Text FramerateTxt;

    [Space]

    public Toggle UseAliasing;
    public Toggle UseHDR;

    [Space]

    public Toggle UseTonemapping;
    public Toggle UseMotionBlur;
    public Toggle UseVignette;
    public Toggle UseBloom;

    [Space]

    public Slider Sensitivity;

    [Header("Profiles & Scriptable Objects")]
    public PostProcessVolume Volume;

    Camera cam;

    Vignette vignette;
    Bloom bloom;
    ColorGrading colorGrading;
    MotionBlur motionBlur;

    void Start()
    {
        QualitySettings.vSyncCount = 0;
        cam = SavingManager.player.GetComponentInChildren<Camera>();

        Volume.profile.TryGetSettings(out vignette);
        Volume.profile.TryGetSettings(out bloom);
        Volume.profile.TryGetSettings(out colorGrading);
        Volume.profile.TryGetSettings(out motionBlur);

        #region Audio Setup

        MainAudioSlider.value = SavingManager.SaveData.MainAudioLevel;
        SFAudioSlider.value = SavingManager.SaveData.SFAudioLevel;
        MusicAudioSlider.value = SavingManager.SaveData.MusicAudioLevel;

        AudioMixer.SetFloat("MainAudioLevel", SavingManager.SaveData.MainAudioLevel);
        AudioMixer.SetFloat("SFLevel", SavingManager.SaveData.SFAudioLevel);
        AudioMixer.SetFloat("MusicLevel", SavingManager.SaveData.MusicAudioLevel);

        #endregion

        #region Graphics

        int fps = SavingManager.SaveData.SettingsData.FPS;
        Application.targetFrameRate = fps;
        FramerateSlider.value = fps;
        FramerateTxt.text = fps.ToString();

        UseAliasing.isOn = SavingManager.SaveData.SettingsData.AA;
        cam.allowMSAA = SavingManager.SaveData.SettingsData.AA;

        UseHDR.isOn = SavingManager.SaveData.SettingsData.HDR;
        cam.allowHDR = SavingManager.SaveData.SettingsData.HDR;

        UseVignette.isOn = SavingManager.SaveData.SettingsData.Vignette;
        vignette.active = SavingManager.SaveData.SettingsData.Vignette;

        UseBloom.isOn = SavingManager.SaveData.SettingsData.Bloom;
        bloom.active = SavingManager.SaveData.SettingsData.Bloom;

        UseTonemapping.isOn = SavingManager.SaveData.SettingsData.Tonemapping;
        colorGrading.active = SavingManager.SaveData.SettingsData.Tonemapping;

        UseMotionBlur.isOn = SavingManager.SaveData.SettingsData.MotionBlur;
        motionBlur.active = SavingManager.SaveData.SettingsData.MotionBlur;

        #endregion

        InputManager.instance.viewSensitivity = SavingManager.SaveData.SettingsData.Sensitivity;
        Sensitivity.value = SavingManager.SaveData.SettingsData.Sensitivity;

        RenderDistance.value = SavingManager.SaveData.SettingsData.RenderDistance;
        RenderDistanceT.text = SavingManager.SaveData.SettingsData.RenderDistance.ToString();
        LQGeneration.isOn = SavingManager.SaveFile.LQGeneration;
    }

    #region BasicChangeFuncs

    public void SetMainVolume(float volume) { AudioMixer.SetFloat("MainAudioLevel", volume); SavingManager.SaveData.MainAudioLevel = volume; }
    public void SetSFVolume(float volume) { AudioMixer.SetFloat("SFLevel", volume); SavingManager.SaveData.SFAudioLevel = volume; }
    public void SetMusicVolume(float volume) { AudioMixer.SetFloat("MusicLevel", volume); SavingManager.SaveData.MusicAudioLevel = volume; }

    public void SetFramerate(float value) { Application.targetFrameRate = (int)value; SavingManager.SaveData.SettingsData.FPS = (int)value; FramerateTxt.text = value.ToString(); }
    public void SetAA(bool aa) { cam.allowMSAA = aa; SavingManager.SaveData.SettingsData.AA = aa; }
    public void SetHDR(bool hdr) { cam.allowHDR = hdr; SavingManager.SaveData.SettingsData.HDR = hdr; }

    public void SetVignette(bool b) { UseVignette.isOn = b; vignette.active = b; }
    public void SetBloom(bool b) { UseBloom.isOn = b; bloom.active = b; }
    public void SetTonemapping(bool b) { UseTonemapping.isOn = b; colorGrading.active = b; }
    public void SetMotionBlur(bool b) { UseMotionBlur.isOn = b; motionBlur.active = b; SavingManager.SaveData.SettingsData.MotionBlur = b; }

    public void SetSensitivity(float s) { InputManager.instance.viewSensitivity = s; SavingManager.SaveData.SettingsData.Sensitivity = s; }

    public void SetRenderDistance(float d) 
    { 
        TerrainGenerator.instance.ViewDst = (int)d; 
        TerrainGenerator.instance.UpdateViewDist();

        SavingManager.SaveData.SettingsData.RenderDistance = (int)d;
        RenderDistanceT.text = d.ToString();
    }
    public void SetLQGeneration(bool u)
    {
        SavingManager.SaveFile.LQGeneration = u;
        PropsGeneration.instance.EnableLQGeneration(u);
    }

    #endregion
}