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

    float defaultY = 0;
    float timer = 0;

    void Start()
    {
        if (SavingManager.GameState == SavingManager.GameStateEnum.Singleplayer) { multiplayer = false; } else { multiplayer = true; }
        defaultY = transform.localPosition.y;
    }

    void FixedUpdate()
    {
        if (!multiplayer) { xRotation -= InputManager.MouseY * InputManager.instance.viewSensitivity; } else { xRotation -= MultiplayerInputManager.MouseY * MultiplayerInputManager.instance.viewSensitivity; }
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        if (!multiplayer) { player.Rotate(Vector3.up * InputManager.MouseX * InputManager.instance.viewSensitivity); } else { player.Rotate(Vector3.up * MultiplayerInputManager.MouseX * MultiplayerInputManager.instance.viewSensitivity); }

        if (InputManager.moving)
        {
            timer += Time.deltaTime * 14f;
            transform.localPosition = new Vector3(transform.localPosition.x, defaultY + Mathf.Sin(timer) * 0.05f, transform.localPosition.z);
        }
        else
        {
            timer = 0;
            transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Lerp(transform.localPosition.y, defaultY, Time.deltaTime * 14f), transform.localPosition.z);
        }
    }
}
