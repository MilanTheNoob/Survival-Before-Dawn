using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    #region Singleton

    public static AudioManager instance;

    private void Awake()
    {
        instance = this;
    }

    #endregion

    [Header("Click noises for UI (picks a random one)")]
    public AudioSource[] clicks;
    [Header("Movement noises (plays all of them)")]
    public AudioSource[] moving;
    [Header("Music & ambience (plays all ambience)")]
    public AudioSource[] music;
    public AudioSource[] ambience;
    [Header("Noises for the game")]
    public AudioSource build;
    public AudioSource equip;
    public AudioSource hurt;
    public AudioSource chop;

    bool oldMoving;

    void Start()
    {
        StartCoroutine(PlayMusicI());
        for (int i = 0; i < ambience.Length; i++) { ambience[i].loop = true; ambience[i].Play(); }
    }

    void Update()
    {
        if (InputManager.moving != oldMoving)
        {
            if (InputManager.moving == true)
            {
                for (int i = 0; i < moving.Length; i++) { moving[i].Play(); }
            }
            else
            {
                for (int i = 0; i < moving.Length; i++) { moving[i].Stop(); }
            }
        }

        oldMoving = InputManager.moving;
    }

    IEnumerator PlayMusicI()
    {
        music[Random.Range(0, music.Length)].Play();
        yield return new WaitForSeconds(Random.Range(180, 240));
        StartCoroutine(PlayMusicI());
    }

    public static void PlayClick() { instance.clicks[Random.Range(0, instance.clicks.Length)].Play(); }
    public static void PlayBuild() { instance.build.Play(); }
    public static void PlayEquip() { instance.equip.Play(); }
    public static void PlayHurt() { instance.hurt.Play(); }
    public static void PlayChop() { instance.chop.Play(); }
}
