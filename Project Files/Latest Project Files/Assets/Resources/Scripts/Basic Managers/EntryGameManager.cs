using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class EntryGameManager : MonoBehaviour
{
    [Header("Loading Circle")]
    public Image circleFill;
    [Header("Loading Time")]
    public float loadDuration;

    void Start()
    {
        if (!SavingManager.SaveFile.finishedTutorial) { TutorialManager.instance.TriggerI(); }
        StartCoroutine(FadeLoadingUI());
    }

    void Update() { if (Time.time < loadDuration) { circleFill.fillAmount = Time.time / loadDuration; } else { circleFill.fillAmount = 1; } }

    IEnumerator FadeLoadingUI()
    {
        InputManager.instance.InstantToggleUISectionsInt(4);
        circleFill.fillAmount = 0;
        yield return new WaitForSeconds(loadDuration);
        InputManager.instance.ToggleUISectionsInt(0);

        if (!SavingManager.SaveFile.finishedTutorial) {TutorialManager.instance.TriggerII(); }
    }  
}

[System.Serializable]
public class LoadingUIComponent
{
    public GameObject g;

    public float startTime;
    public float fadeTime;
    public float duration;
}