using System.Collections;
using UnityEngine.Audio;
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

    [Header("The list of click noises for UI (picks a random one)")]
    public AudioSource[] clicks;
    [Header("List of movement noises (plays all of them)")]
    public AudioSource[] moving;
    [Header("Lists of music & ambience (plays all ambience)")]
    public AudioSource[] music;
    public AudioSource[] ambience;
    [Header("Different noises for the game")]
    public AudioSource build;
    public AudioSource equip;
    public AudioSource hurt;
    public AudioSource chop;

    bool oldMoving;

    // Start is called before the first frame update
    void Start()
    {
        // Start the loop coroutine to play music peridocially
        StartCoroutine(PlayMusic());

        // Loop through all the ambience noises 
        for (int i = 0; i < ambience.Length; i++)
        {
            // Start an ambience noise
            ambience[i].loop = true;
            ambience[i].Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the player has changed moving
        if (InputManager.moving != oldMoving)
        {
            // If he is moving then play music, if not then stop
            if (InputManager.moving == true)
            {
                for (int i = 0; i < moving.Length; i++) { moving[i].Play(); }
            }
            else
            {
                for (int i = 0; i < moving.Length; i++) { moving[i].Stop(); }
            }
        }

        // Update the old moving var
        oldMoving = InputManager.moving;
    }

    // Called to update music periodically
    IEnumerator PlayMusic()
    {
        music[Random.Range(0, music.Length)].Play();

        // Wait between 3 to 4 minutes
        yield return new WaitForSeconds(Random.Range(180, 240));

        StartCoroutine(PlayMusic());
    }

    // All the basic sf funcs
    public static void PlayClick() { instance.clicks[Random.Range(0, instance.clicks.Length)].Play(); }
    public static void PlayBuild() { instance.build.Play(); }
    public static void PlayEquip() { instance.equip.Play(); }
    public static void PlayHurt() { instance.hurt.Play(); }
    public static void PlayChop() { instance.chop.Play(); }
}
