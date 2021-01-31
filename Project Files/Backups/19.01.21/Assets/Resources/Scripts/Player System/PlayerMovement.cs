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
    InputManager inputManager;
    Animator anim;

    Vector3 velocity;

    // Start is called before the first frame update
    void Start()
    {
        // Get all the components needed
        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        inputManager = InputManager.instance;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (InputManager.Vertical == 0 && InputManager.Horizontal == 0) { InputManager.moving = false; } else { InputManager.moving = true; }

        // What play type rn?
        if (SavingManager.GameState == SavingManager.GameStateEnum.Singleplayer)
        {
            // grAvITy sTUfF
            if (controller.isGrounded && velocity.y < 0)
                velocity.y = -2f;

            // Animate the player
            if (InputManager.Vertical == 0) { anim.SetFloat("MovementValue", 0f); } else { anim.SetFloat("MovementValue", 1f); }

            // Create a Vector3 for the movement of the player
            Vector3 move = transform.right * InputManager.Horizontal + transform.forward * InputManager.Vertical;
            // Move the player
            controller.Move(move * speed * Time.deltaTime);

            // Calculate the velocity of the player falling to the ground
            velocity.y += InputManager.instance.gravity * Time.deltaTime;
            // Apply the velocity to the character controller
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
            // Disable the controller
            if (controller.enabled)
                controller.enabled = false;

            // Send any input to the server for movement calculations
            ClientSend.PlayerMovement(InputManager.Horizontal, InputManager.Vertical, inputManager.jump.onClicked);
        }
    }

    public void Jump()
    {
        // Return if we are falling
        if (!controller.isGrounded)
            return;

        // Set the jump velocity
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * InputManager.instance.gravity);
        // Move the controller
        controller.Move(velocity * Time.deltaTime);
    }
}