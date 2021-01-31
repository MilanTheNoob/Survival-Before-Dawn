using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
public class Trader_InteractableItem : InteractableItem
{
    [Header("Basic GameObjects")]
    public GameObject bodyMesh;
    public GameObject groundCheck;

    [Header("The Trader's Inventory")]
    public List<ItemSettings> inventory = new List<ItemSettings>();

    [HideInInspector]
    public Animator anim;
    [HideInInspector]
    public CharacterController controller;

    [HideInInspector]
    public bool isGrounded;
    [HideInInspector]
    public Vector3 velocity;

    void Awake() { interactTxt = "Talk to"; }

    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
        controller = gameObject.GetComponent<CharacterController>();
        
        TradingManager.instance.RandomizeTraderInventory(this);
    }

    //void Update() { CalculateGravity(); }

    public override void OnInteract()
    {
        base.OnInteract();
        TradingManager.instance.TradeWithTrader(this);
    }
    /*
     * NEEDS WORK
     * 
    // Applies gravity to the player
    public virtual void CalculateGravity()
    {
        // Check if we are alread 
        if (!controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;
        
        // Calculate the velocity of the NPC falling to the ground
        velocity.y += InputManager.instance.gravity * Time.deltaTime;

        // Apply the velocity to the character controller
        controller.Move(velocity * Time.deltaTime); 
    }

    */
}
