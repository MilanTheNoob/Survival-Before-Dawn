using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Present_InteractableItem : InteractableItem
{
    [Header("List of things the present could have in it")]
    public ItemSettings[] items;

    void Awake() { interactTxt = "Open"; }

    // Override the OnInteract function in the InteractionSettings
    public override void OnInteract()
    {
        // Call the original OnInteract function
        base.OnInteract();

        // Instantiate a new random item
        GameObject g = Instantiate(items[UnityEngine.Random.Range(0, items.Length)].gameObject, new Vector3(transform.position.x, transform.position.y + 10f, transform.position.z), Quaternion.identity);
        g.AddComponent<Rigidbody>();
        try { g.GetComponent<MeshCollider>().convex = true; } catch(Exception ex) { }

        // Destroy the present gameObject
        Destroy(gameObject);
    }
}
