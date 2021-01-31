using UnityEngine;

public class MouseLook : MonoBehaviour
{
    #region Singleton

    public static MouseLook instance;

    void Awake()
    {
        instance = this;
    }

    #endregion

    public Transform player;
    bool multiplayer;
    float xRotation = 0f;

    private void Start()
    {
        if (SavingManager.GameState == SavingManager.GameStateEnum.Singleplayer) { multiplayer = false; } else { multiplayer = true; }
    }

    void Update()
    {
        if (!multiplayer) { xRotation -= InputManager.MouseY; } else { xRotation -= MultiplayerInputManager.MouseY; }
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        if (!multiplayer) { player.Rotate(Vector3.up * InputManager.MouseX); } else { player.Rotate(Vector3.up * MultiplayerInputManager.MouseX); }
    }
}
