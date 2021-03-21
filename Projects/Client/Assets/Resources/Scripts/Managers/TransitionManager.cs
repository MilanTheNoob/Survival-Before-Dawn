using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TransitionManager : MonoBehaviour
{
    #region Singleton

    public static TransitionManager instance;
    void Awake() { instance = this; }

    #endregion

    public Animator anim;
    public Text LoadingText;

    [HideInInspector]
    public GameObject UI;

    static bool loading;

    void Start()
    {
        transform.parent = null;
        DontDestroyOnLoad(this);

        UI = GameObject.Find("UI");
        UI.SetActive(false);
        StartCoroutine(ResetI());
    }

    IEnumerator ResetI()
    {
        yield return new WaitForSeconds(6f);
        UI.SetActive(true);
        TweeningLibrary.FadeIn(UI, 0.3f);
    }

    public static void ToMenu() 
    { 
        if (loading) { return; }
        loading = true;

        TweeningLibrary.FadeOut(GameObject.Find("UI"), 1f); 
        instance.anim.SetTrigger("DefaultTransition"); 
        instance.StartCoroutine(ToMenuI()); 
    }
    static IEnumerator ToMenuI() 
    { 
        yield return new WaitForSeconds(3f); 
        SceneManager.LoadScene(0, LoadSceneMode.Single);

        loading = false;
    }

    public static void ToSingleplayer() 
    {
        if (loading) { return; }
        loading = true;

        TweeningLibrary.FadeOut(instance.UI, 1f);
        instance.anim.SetTrigger("DefaultTransition"); 
        instance.StartCoroutine(ToSingleplayerI()); 
    }
    static IEnumerator ToSingleplayerI() 
    { 
        yield return new WaitForSeconds(3f); 
        SceneManager.LoadScene(1, LoadSceneMode.Single);

        loading = false;
    }

    public static void ToMultiplayer() 
    {
        if (loading) { return; }
        loading = true;

        TweeningLibrary.FadeOut(instance.UI, 0.3f); 
        instance.anim.SetTrigger("DefaultTransition"); 
        instance.StartCoroutine(ToMultiplayerI()); 
    }
    static IEnumerator ToMultiplayerI() 
    { 
        yield return new WaitForSeconds(3f); 
        SceneManager.LoadScene(2, LoadSceneMode.Single);

        loading = false;
    }
}
