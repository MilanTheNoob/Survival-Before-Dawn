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

    // The instance of ourselves
    public static PreviewInputManager instance;

    void Awake()
    {
        instance = this;

        // Disable all UI Sections
        ToggleUISectionsInt(0);

        // Disable the error box
        errorParent.SetActive(false);
    }

    // Shows an error to the player
    public static void ShowError(string error)
    {
        TweeningLibrary.FadeIn(instance.errorParent, 0.2f);
        instance.errorText.text = error;
    }
    // Hides the error page
    public void HideError() { TweeningLibrary.FadeOut(errorParent, 0.2f); }

    // Called to reset the inputted button
    public void ResetButton(PreviewButton button, bool reset)
    {
        // Play click noise
        AudioManager.PlayClick();

        // Get all the txt & img children of the button
        Text[] txt = button.GetComponentsInChildren<Text>();
        Image[] img = button.GetComponentsInChildren<Image>();

        // Loop through all the children and set the colors
        for (int i = 0; i < txt.Length; i++) { TweeningLibrary.LerpColor(txt[i], button.tabActive, button.tabIdle, 0.1f); }
        for (int i = 0; i < img.Length; i++) { TweeningLibrary.LerpColor(img[i], button.tabActive, button.tabIdle, 0.1f); }

        // Set the color of the button
        TweeningLibrary.LerpColor(button.button, button.tabIdle, button.tabActive, 0.1f);

        // Set onClicked & isClicked to true
        button.onClicked = true;
        button.isClicked = true;

        // Start the coroutine for the next part
        if (reset)
            StartCoroutine(Button_OnPointerDown(button, false));
    }

    // Called to reset the inputted button without affecting the kids
    public void ResetButton(PreviewButton button, bool reset, bool n)
    {
        // Play click noise
        AudioManager.PlayClick();

        // Set the color of the button
        TweeningLibrary.LerpColor(button.button, button.tabIdle, button.tabActive, 0.1f);

        // Set onClicked & isClicked to true
        button.onClicked = true;
        button.isClicked = true;

        // Start the coroutine for the next part
        if (reset)
            StartCoroutine(Button_OnPointerDown(button, true));
    }

    // Called to continue the resetting of the desired button
    public IEnumerator Button_OnPointerDown(PreviewButton button, bool leaveChildren)
    {
        // Wait until the end of the frame
        yield return new WaitForSeconds(0.2f);

        // Set all of the states of the button to false
        button.onClicked = false;
        button.onReleased = false;
        button.isClicked = false;

        if (!leaveChildren)
        {
            // Get all the txt & img children of the button
            Text[] txt = button.GetComponentsInChildren<Text>();
            Image[] img = button.GetComponentsInChildren<Image>();

            // Loop through all the children and set the colors
            for (int i = 0; i < txt.Length; i++) { TweeningLibrary.LerpColor(txt[i], button.tabIdle, button.tabActive, 0.1f); }
            for (int i = 0; i < img.Length; i++) { TweeningLibrary.LerpColor(img[i], button.tabIdle, button.tabActive, 0.1f); }
        }

        // Set the color of the button
        TweeningLibrary.LerpColor(button.button, button.tabActive, button.tabIdle, 0.1f);
    }

    // Called to disable all UI Sections
    public void DisableUISections()
    {
        for (int i = 0; i < UISections.Length; i++)
        {
            if (UISections[i].activeSelf) { TweeningLibrary.FadeOut(UISections[i], 0.2f); }
        }

        TweeningLibrary.FadeOut(BackgroundUIPanel, 0.2f);
    }

    // Called to toggle a ui section using an int
    public void ToggleUISectionsInt(int UIIndex)
    {
        for (int i = 0; i < UISections.Length; i++)
        {
            // Toggle the ui sections
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

    // Called to instantly toggle a ui section
    public void InstantToggleUISectionsInt(int UIIndex)
    {
        for (int i = 0; i < UISections.Length; i++)
        {
            // Toggle the ui sections
            if (i == UIIndex) { UISections[i].SetActive(true); } else { UISections[i].SetActive(false); }
        }

        if (UIIndex == 0) { TweeningLibrary.FadeOut(BackgroundUIPanel, 0.2f); } else { TweeningLibrary.FadeIn(BackgroundUIPanel, 0.2f); }
    }
}
