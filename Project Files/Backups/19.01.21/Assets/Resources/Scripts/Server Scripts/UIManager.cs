using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    #region Singleton

    public static UIManager instance;

    private void Awake()
    {
        instance = this;
    }

    #endregion

    public GameObject startMenu;
    public InputField usernameField;

    InputManager inputManager;

    // Called at the beginning of the game
    private void Start() { inputManager = InputManager.instance; }

    // Called when the player clicc the join button
    public void ConnectToServer()
    {
        // Connect to server
        Client.instance.ConnectToServer();
    }
}
