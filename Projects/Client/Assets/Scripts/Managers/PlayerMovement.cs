using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region Singleton

    public static PlayerMovement instance;

    private void Awake()
    {
        instance = this;
    }

    #endregion

    public float speed = 12f;
    public float jumpHeight = 3f;

    CharacterController controller;
    Animator anim;

    Vector3 velocity;

    void Start()
    {
        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
    }

    void FixedUpdate()
    {
        if (InputManager.Vertical == 0 || InputManager.Horizontal == 0) { InputManager.moving = false; } else { InputManager.moving = true; }

        if (SavingManager.GameState == SavingManager.GameStateEnum.Singleplayer)
        {
            if (controller.isGrounded && velocity.y < 0)
                velocity.y = -2f;

            if (InputManager.Vertical == 0) { anim.SetFloat("MovementValue", 0f); } else { anim.SetFloat("MovementValue", 1f); }

            Vector3 move = transform.right * InputManager.Horizontal + transform.forward * InputManager.Vertical;
            controller.Move(move * speed * Time.deltaTime);

            velocity.y += InputManager.instance.gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);

            if (transform.position.y < -15f)
            {
                controller.enabled = false;
                transform.position = new Vector3(transform.position.x, 25f, transform.position.z);
                controller.enabled = true;
            }
        }
        else if (SavingManager.GameState == SavingManager.GameStateEnum.Multiplayer)
        {
            if (controller.enabled)
                controller.enabled = false;

            ClientSend.SendPlayerMovement(MultiplayerInputManager.Horizontal, MultiplayerInputManager.Vertical, MultiplayerInputManager.instance.jump.onClicked);
        }
    }

    public void Jump()
    {
        if (!controller.isGrounded)
            return;

        velocity.y = Mathf.Sqrt(jumpHeight * -2f * InputManager.instance.gravity);
        controller.Move(velocity * Time.deltaTime);
    }
}