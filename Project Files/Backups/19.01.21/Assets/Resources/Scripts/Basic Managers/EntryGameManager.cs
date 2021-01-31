using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class EntryGameManager : MonoBehaviour
{
    [Header("The Fill of the Loading Circle")]
    public Image circleFill;

    [Header("The duration we spend loading")]
    public float loadDuration;

    // Called at the awakening of the game
    void Start()
    {
        // If the player hasn't played the turoial then trigger it
        if (!SavingManager.SaveFile.finishedTutorial)
            TutorialManager.instance.TriggerI();

        // Start the coroutine to fade loading ui
        StartCoroutine(FadeLoadingUI());
    }

    // Called every frame
    void Update()
    {
        // Set the loading fill amount correctly
        if (Time.time < loadDuration) { circleFill.fillAmount = Time.time / loadDuration; } else { circleFill.fillAmount = 1; }
    }

    // Called to fade out the loading ui
    IEnumerator FadeLoadingUI()
    {
        // Toggle the UIs smoothly with fading
        InputManager.instance.InstantToggleUISectionsInt(4);
        yield return new WaitForSeconds(loadDuration);
        InputManager.instance.ToggleUISectionsInt(0);

        // Tell the InputManager that we have finished loading
        InputManager.onGameLoaded = true;

        // Load the Tutorial if the player hasn't finished it
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