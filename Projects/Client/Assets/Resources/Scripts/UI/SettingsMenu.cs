using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SettingsMenu : MonoBehaviour
{
    [Header("Audio vars")]
    public AudioMixer AudioMixer;

    [Space]

    public Slider MainAudioSlider;
    public Slider SFAudioSlider;
    public Slider MusicAudioSlider;

    [Header("Graphic vars")]
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

    [Header("Misc vars")]
    public Slider Sensitivity;

    [Header("Volume Profile")]
    public VolumeProfile ProcessingVolume;

    Camera cam;

    Vignette vignette;
    Bloom bloom;
    Tonemapping tonemapping;
    MotionBlur motionBlur;

    void Start()
    {
        QualitySettings.vSyncCount = 0;
        cam = SavingManager.player.GetComponentInChildren<Camera>();

        ProcessingVolume.TryGet(out vignette);
        ProcessingVolume.TryGet(out bloom);
        ProcessingVolume.TryGet(out tonemapping);
        ProcessingVolume.TryGet(out motionBlur);

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
        tonemapping.active = SavingManager.SaveData.SettingsData.Tonemapping;

        UseMotionBlur.isOn = SavingManager.SaveData.SettingsData.MotionBlur;
        motionBlur.active = SavingManager.SaveData.SettingsData.MotionBlur;

        #endregion

        InputManager.instance.viewSensitivity = SavingManager.SaveData.SettingsData.Sensitivity;
        Sensitivity.value = SavingManager.SaveData.SettingsData.Sensitivity;
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
    public void SetTonemapping(bool b) { UseTonemapping.isOn = b; tonemapping.active = b; }
    public void SetMotionBlur(bool b) { UseMotionBlur.isOn = b; motionBlur.active = b; SavingManager.SaveData.SettingsData.MotionBlur = b; }

    public void SetSensitivity(float s) { InputManager.instance.viewSensitivity = s; SavingManager.SaveData.SettingsData.Sensitivity = s; }

    #endregion
}