using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Choppable_InteractableItem : InteractableItem
{
    [Header("The items that are dropped from the tree when chopped")]
    public ItemSettings[] droppedItemSettings;

    // Set the interact text to 'Chop' at the beginning of the game
    void Awake() { interactTxt = "Chop"; }

    // Override the OnInteract function in the InteractionSettings
    public override void OnInteract()
    {
        // Call the original OnInteract function
        base.OnInteract();

        // Do the swing animation
        InputManager.instance.player.GetComponent<Animator>().SetTrigger("Swing");

        // Make audio
        AudioManager.PlayChop();

        // Swing the axe using animations
        ToolsManager.instance.SwingAnim();
        // Set isInteractable to false 
        isInteractable = false;

        // Get the rigidbody of the item
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();

        // If the item doesn't have then add one
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody>();

        // Use gravity
        rb.useGravity = true;
        // Set collision detection mode to continuous dynamic
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        // Add a force to the rigidbody
        rb.AddForce(Vector3.forward, ForceMode.Impulse);

        // Replace the tree
        StartCoroutine(replaceTree());
    }

    // Called to replace the tree with materials
    IEnumerator replaceTree()
    {
        // Wait for 0.7 seconds
        yield return new WaitForSeconds(0.7f);

        // Loop through all the dropped item settings
        for (int i = 0; i < droppedItemSettings.Length; i++)
        {
            // Instantiate a new Prop
            GameObject newProp = Instantiate(droppedItemSettings[i].gameObject);

            // Set the parent and location
            newProp.transform.parent = gameObject.transform.parent;
            newProp.transform.localPosition = gameObject.transform.localPosition;
            newProp.transform.name = droppedItemSettings[i].gameObject.name;

            // Add a mesh collider if we dont have one
            if (newProp.GetComponent<MeshCollider>() == null)
                newProp.AddComponent<MeshCollider>();

            // Set the collider to convex
            newProp.GetComponent<MeshCollider>().convex = true;

            // Add a new rigidbody
            Rigidbody tmpRb = newProp.AddComponent<Rigidbody>();
            // Set the rb's detection mode to continuous dynamic
            tmpRb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            // Use gravity
            tmpRb.useGravity = true;
        }

        // Destroy ourselves
        DestroyImmediate(gameObject);
    }
}
