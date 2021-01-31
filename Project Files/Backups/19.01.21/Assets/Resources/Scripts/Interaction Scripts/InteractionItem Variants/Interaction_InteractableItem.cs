using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction_InteractableItem : InteractableItem
{
    [Header("A variant of Interaction Settings that stores and destroys the Interactable object")]
    public ItemSettings itemSettings;

    // Set the interact text to 'Pick up'
    void Awake() { interactTxt = "Pick up"; }

    // Override the OnInteract function in the InteractionSettings
    public override void OnInteract()
    {
        // Call the original OnInteract function
        base.OnInteract();

        // Add a new item to the inventory if possible
        if (Inventory.instance.Add(itemSettings)) { Destroy(gameObject); }
    }
}
