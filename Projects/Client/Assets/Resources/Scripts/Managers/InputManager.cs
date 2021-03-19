using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;

    [Header("UI Components")]
    public Joystick viewJoystick;
    public Joystick moveJoystick;

    [Space]

    public Button jump;

    [Header("UI Sections")]
    public GameObject[] UISections;
    public GameObject backgroundPanel;

    [Header("Random spawn items")]
    public List<ItemSettings> smallRandomItems;

    [Header("Error UI components")]
    public GameObject errorParent;
    public Text errorText;

    [Header("Misc")]
    public float gravity = -9.81f;
    public float groundDistance = 0.4f;
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
        DisableUISections();
        StartCoroutine(EnableUI());

        instance = this;
        errorParent.SetActive(false);
        SavingManager.SetPlayer();
    }

    void Start() 
    {        
        SavingManager.SaveGameCallback += SavePos;
        if (SavingManager.SaveData.completedTutorial) { MovePlayer(SavingManager.SaveFile.playerPos); }
    }

    void FixedUpdate() 
    {
        Horizontal = moveJoystick.Horizontal;
        Vertical = moveJoystick.Vertical;

        MouseX = viewJoystick.Horizontal * viewSensitivity * Time.deltaTime;
        MouseY = viewJoystick.Vertical * viewSensitivity * Time.deltaTime;

        if (jump.onClicked) { SavingManager.player.GetComponent<PlayerMovement>().Jump(); }
    }

    /// <summary>
    /// Writes the position of the player to save data
    /// </summary>
    public void SavePos() { SavingManager.SaveFile.playerPos = SavingManager.player.transform.position; }

    /// <summary>
    /// Shows an error with the UI
    /// </summary>
    /// <param name="error">The error to show to the player</param>
    public static void ShowError(string error) { TweeningLibrary.FadeIn(instance.errorParent, 0.2f); instance.errorText.text = error; }
    public void HideError() { TweeningLibrary.FadeOut(errorParent, 0.2f); }

    /// <summary>
    /// Moves the current player
    /// </summary>
    /// <param name="pos">The position to move the player (gloabl pos, not local)</param>
    public static void MovePlayer(Vector3 pos)
    {
        SavingManager.player.GetComponent<CharacterController>().enabled = false;
        SavingManager.player.transform.position = pos;
        SavingManager.player.GetComponent<CharacterController>().enabled = true;
    }

    /// <summary>
    /// Code for a basic click on a UI Image
    /// </summary>
    /// <param name="button">The button sccript to use</param>
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

    /// <summary>
    /// Disables all the UI Sections (menus, controls, etc)
    /// </summary>
    public void DisableUISections()
    {
        for (int i = 0; i < UISections.Length; i++)
        {
            if (UISections[i] != null)
            {
                if (UISections[i].activeSelf) { TweeningLibrary.FadeOut(UISections[i], 0.3f); }
            }
        }
    }

    /// <summary>
    /// Toggles a UI Section
    /// </summary>
    /// <param name="UIIndex">The Section id</param>
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

    /// <summary>
    /// Instantly toggles a UI Section
    /// </summary>
    /// <param name="UIIndex">The Section Id</param>
    public void InstantToggleUISectionsInt(int UIIndex)
    {
        for (int i = 0; i < UISections.Length; i++)
        {
            if (UISections[i] != null)
            {
                if (i == UIIndex) { UISections[i].SetActive(true); } else { UISections[i].SetActive(false); }
            }
        }
    }

    void OnApplicationPause() { SavingManager.SaveGame(); }
    void OnApplicationQuit() { SavingManager.SaveGame(); }

    /// <summary>
    /// Quits the game when called
    /// </summary>
    public static void QuitGame() { Application.Quit(); }

    /// <summary>
    /// Sends the player to the menu
    /// </summary>
    public void ToMenu()
    {
        Destroy(SavingManager.instance.gameObject);
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }

    public void ShowLeaderboards() { Social.ShowLeaderboardUI(); }
    public void ShowAchievements() { Social.ShowAchievementsUI(); }

    IEnumerator EnableUI()
    {
        yield return new WaitForSeconds(3f);
        ToggleUISectionsInt(0);
    }
}
