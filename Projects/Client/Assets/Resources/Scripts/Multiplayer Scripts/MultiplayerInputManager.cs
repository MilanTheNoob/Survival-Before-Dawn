using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class MultiplayerInputManager : MonoBehaviour
{
    public static MultiplayerInputManager instance;

    [Header("Joystick inputs")]
    public Joystick viewJoystick;
    public Joystick moveJoystick;

    [Header("Jump button")]
    public Button jump;

    [Header("UI Sections")]
    public GameObject[] UISections;
    public GameObject backgroundPanel;

    [Header("Error UI components")]
    public GameObject errorParent;
    public Text errorText;

    [Header("Misc")]
    public float viewSensitivity = 100f;

    public delegate void MultiplayerDelegate();
    public MultiplayerDelegate MultiplayerCallback;

    public static float MouseX;
    public static float MouseY;
    public static float Horizontal;
    public static float Vertical;

    public static bool moving;

    public static GameObject player;

    private void Awake()
    {
        DisableUISections();
        StartCoroutine(EnableUI());

        instance = this;
        errorParent.SetActive(false);
    }

    void Update()
    {
        Horizontal = moveJoystick.Horizontal;
        Vertical = moveJoystick.Vertical;

        MouseX = viewJoystick.Horizontal * viewSensitivity * Time.deltaTime;
        MouseY = viewJoystick.Vertical * viewSensitivity * Time.deltaTime;
    }

    public void GoToProfile() { SceneManager.LoadScene(0, LoadSceneMode.Single); }

    public void Click(Button button)
    {
        AudioManager.PlayClick();
        TweeningLibrary.LerpColor(button.button, button.tabIdle, button.tabActive, 0.1f);

        button.onClicked = true;
        button.isClicked = true;

        StartCoroutine(ResetClickI(button));
    }

    public IEnumerator ResetClickI(Button button)
    {
        yield return new WaitForSeconds(0.2f);

        button.onClicked = false;
        button.onReleased = false;
        button.isClicked = false;

        TweeningLibrary.LerpColor(button.button, button.tabActive, button.tabIdle, 0.1f);
    }

    public void DisableUISections()
    {
        for (int i = 0; i < UISections.Length; i++)
        {
            if (UISections[i] != null) { if (UISections[i].activeSelf) { TweeningLibrary.FadeOut(UISections[i], 0.3f); } }
        }
    }

    public void ToggleUISectionsInt(int UIIndex)
    {
        for (int i = 0; i < UISections.Length; i++)
        {
            if (UISections[i] != null)
            {
                if (i == UIIndex)
                {
                    TweeningLibrary.FadeIn(UISections[i], 0.3f);
                }
                else if (UISections[i].activeSelf)
                {
                    TweeningLibrary.FadeOut(UISections[i], 0.3f);
                }
            }
        }

        if (UIIndex == 0 && backgroundPanel.activeSelf) { TweeningLibrary.FadeOut(backgroundPanel, 0.3f); } else if (UIIndex != 0 && !backgroundPanel.activeSelf) { TweeningLibrary.FadeIn(backgroundPanel, 0.3f); }
    }

    public void InstantToggleUISectionsInt(int UIIndex)
    {
        for (int i = 0; i < UISections.Length; i++)
        {
            if (i == UIIndex) { UISections[i].SetActive(true); } else { UISections[i].SetActive(false); }
        }
    }

    public void Quit() { Application.Quit(); }

    IEnumerator EnableUI()
    {
        yield return new WaitForSeconds(3f);
        ToggleUISectionsInt(0);
    }
}
