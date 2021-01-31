using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;

    [Header("Joystick inputs")]
    public Joystick viewJoystick;
    public Joystick moveJoystick;

    [Header("Jump button")]
    public Button jump;

    [Header("UI Sections")]
    public GameObject[] UISections;
    public GameObject backgroundPanel;

    [Header("The Player")]
    public GameObject player;

    [Header("Simple world parameters")]
    public float gravity = -9.81f;
    public float groundDistance = 0.4f;

    [Header("Random spawn items")]
    public List<ItemSettings> smallRandomItems;

    [Header("Error UI components")]
    public GameObject errorParent;
    public Text errorText;

    [Header("Misc")]
    public int cullDist;
    public GameObject craftingRecipeUI;
    public float viewSensitivity = 100f;

    public delegate void MultiplayerDelegate();
    public MultiplayerDelegate MultiplayerCallback;

    public static float MouseX;
    public static float MouseY;
    public static float Horizontal;
    public static float Vertical;

    public static bool moving;

    private void Awake()
    {
        instance = this;

        DisableUISections();
        errorParent.SetActive(false);
    }

    void Start() 
    {        
        SavingManager.SaveGameCallback += SavePos;
        MovePlayer(SavingManager.SaveFile.playerPos);
    }

    void Update() 
    {
        Horizontal = moveJoystick.Horizontal;
        Vertical = moveJoystick.Vertical;

        MouseX = viewJoystick.Horizontal * viewSensitivity * Time.deltaTime;
        MouseY = viewJoystick.Vertical * viewSensitivity * Time.deltaTime;
    }

    public void SavePos() { SavingManager.SaveFile.playerPos = player.transform.position; }

    public static void ShowError(string error) { TweeningLibrary.FadeIn(instance.errorParent, 0.2f); instance.errorText.text = error; }
    public void HideError() { TweeningLibrary.FadeOut(errorParent, 0.2f); }

    public static void MovePlayer(Vector3 pos)
    {
        instance.player.GetComponent<CharacterController>().enabled = false;
        instance.player.transform.position = pos;
        instance.player.GetComponent<CharacterController>().enabled = true;
    }

    public void Click(Button button, bool reset)
    {
        AudioManager.PlayClick();
        TweeningLibrary.LerpColor(button.button, button.tabIdle, button.tabActive, 0.1f);

        button.onClicked = true;
        button.isClicked = true;

        if (reset)
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
            if (UISections[i].activeSelf) { TweeningLibrary.FadeOut(UISections[i], 0.3f); }
        }
    }

    public void ToggleUISectionsInt(int UIIndex)
    {
        for (int i = 0; i < UISections.Length; i++)
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

        if (UIIndex == 0 && backgroundPanel.activeSelf) { TweeningLibrary.FadeOut(backgroundPanel, 0.3f); } else if (UIIndex != 0 && !backgroundPanel.activeSelf) { TweeningLibrary.FadeIn(backgroundPanel, 0.3f); }
    }

    public void InstantToggleUISectionsInt(int UIIndex)
    {
        for (int i = 0; i < UISections.Length; i++)
        {
            if (i == UIIndex) { UISections[i].SetActive(true); } else { UISections[i].SetActive(false); }
        }
    }

    void OnApplicationPause() { SavingManager.SaveGame(); }
    void OnApplicationQuit() { SavingManager.SaveGame(); }

    public static void QuitGame() { SavingManager.SaveGame(); Application.Quit(); }
}
