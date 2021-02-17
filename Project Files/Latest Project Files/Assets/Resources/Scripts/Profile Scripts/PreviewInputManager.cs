using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class PreviewInputManager : MonoBehaviour
{
    [Header("The different UI Sections")]
    public GameObject[] UISections;

    [Header("Background panel")]
    public GameObject BackgroundUIPanel;

    [Header("UI components incase of error")]
    public GameObject errorParent;
    public Text errorText;

    public static PreviewInputManager instance;

    void Awake()
    {
        instance = this;
        ToggleUISectionsInt(0);
        errorParent.SetActive(false);
    }

    public static void ShowError(string error)
    {
        TweeningLibrary.FadeIn(instance.errorParent, 0.2f);
        instance.errorText.text = error;
    }
    public void HideError() { TweeningLibrary.FadeOut(errorParent, 0.2f); }

    public void ResetButton(PreviewButton button)
    {
        AudioManager.PlayClick();
        TweeningLibrary.LerpColor(button.button, button.tabIdle, button.tabActive, 0.1f);

        button.onClicked = true;
        button.isClicked = true;

        StartCoroutine(Button_OnPointerDown(button));
    }

    public IEnumerator Button_OnPointerDown(PreviewButton button)
    {
        yield return new WaitForSeconds(0.2f);

        button.onClicked = false;
        button.onReleased = false;
        button.isClicked = false;

        TweeningLibrary.LerpColor(button.button, button.tabActive, button.tabIdle, 0.1f);
    }

    public void DisableUISections()
    {
        for (int i = 0; i < UISections.Length; i++) { if (UISections[i].activeSelf) { TweeningLibrary.FadeOut(UISections[i], 0.2f); } }
        TweeningLibrary.FadeOut(BackgroundUIPanel, 0.2f);
    }

    public void ToggleUISectionsInt(int UIIndex)
    {
        for (int i = 0; i < UISections.Length; i++)
        {
            if (i == UIIndex)
            {
                TweeningLibrary.FadeIn(UISections[i], 0.2f);
            }
            else if (UISections[i].activeSelf)
            {
                TweeningLibrary.FadeOut(UISections[i], 0.2f);
            }
        }
        if (UIIndex == 0) { TweeningLibrary.FadeOut(BackgroundUIPanel, 0.2f); } else { TweeningLibrary.FadeIn(BackgroundUIPanel, 0.2f); }
    }

    public void InstantToggleUISectionsInt(int UIIndex)
    {
        for (int i = 0; i < UISections.Length; i++) { if (i == UIIndex) { UISections[i].SetActive(true); } else { UISections[i].SetActive(false); } }
        if (UIIndex == 0) { TweeningLibrary.FadeOut(BackgroundUIPanel, 0.2f); } else { TweeningLibrary.FadeIn(BackgroundUIPanel, 0.2f); }
    }
    
    public void OpenUrl(string url) { Application.OpenURL(url); }
}
