using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class EntryGameManager : MonoBehaviour
{
    [Header("Loading Circle")]
    public Image circleFill;
    [Header("Loading Time")]
    public float loadDuration;

    bool finished;
    float rTime;

    void Start()
    {
        if (!SavingManager.SaveData.completedTutorial) { TutorialManager.instance.TriggerI(); }
        StartCoroutine(FadeLoadingUI());
    }

    void FixedUpdate() 
    {
        if (!finished)
        {
            rTime += Time.fixedDeltaTime;

            if (rTime < loadDuration)
            {
                circleFill.fillAmount = rTime / loadDuration;
            }
            else
            {
                circleFill.fillAmount = 0;
                rTime = 0f;
            }
        }
    }

    IEnumerator FadeLoadingUI()
    {
        InputManager.instance.InstantToggleUISectionsInt(4);
        circleFill.fillAmount = 0;
        yield return new WaitForSeconds(loadDuration);
        InputManager.instance.ToggleUISectionsInt(0);
        finished = true;

        if (!SavingManager.SaveData.completedTutorial) {TutorialManager.instance.TriggerII(); }
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