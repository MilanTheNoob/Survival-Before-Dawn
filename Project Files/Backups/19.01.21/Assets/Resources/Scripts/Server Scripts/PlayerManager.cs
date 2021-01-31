using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public int id;
    public string username;

    Vector3 oldPos;

    InputManager inputManager;
    Animator anim;

    // Called at the beginning of the game
    private void Start()
    {
        // Set the oldPos
        oldPos = transform.position;

        // Get the InoutManager instance
        inputManager = InputManager.instance;
        // Get the anim
        anim = gameObject.GetComponent<Animator>();
    }

    // Called every client tick
    private void FixedUpdate()
    {
        // If the game is in multiplayer
        if (SavingManager.GameState == SavingManager.GameStateEnum.Multiplayer)
        {
            // Change the anim state depending on if we are moving
            if (transform.position == oldPos)
            {
                anim.SetFloat("MovementValue", 0f);
            }
            else
            {
                anim.SetFloat("MovementValue", 1f);
            }

            // Update the oldPos
            oldPos = transform.position;
        }
    }
}
