using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
public class Trader_InteractableItem : InteractableItem
{
    [Header("Basic GameObjects")]
    public GameObject[] hairPrefabs;
    public GameObject[] randomColorObj;
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

    // Set the interact text to 'Talk To' at the beginning of the game
    void Awake() { interactTxt = "Talk to"; }

    // Start is called before the first frame update
    void Start()
    {
        // Get the trader's animator & controller
        anim = gameObject.GetComponent<Animator>();
        controller = gameObject.GetComponent<CharacterController>();
        
        // Randomize our inventory
        TradingManager.instance.RandomizeTraderInventory(this);
    }

    // Update is called once per frame
    void Update()
    {
        // Apply the laws of physics to the trader
        //CalculateGravity();
    }

    // Called when the player interacts with us
    public override void OnInteract()
    {
        // Call the base func
        base.OnInteract();
        // Call the manager func
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
