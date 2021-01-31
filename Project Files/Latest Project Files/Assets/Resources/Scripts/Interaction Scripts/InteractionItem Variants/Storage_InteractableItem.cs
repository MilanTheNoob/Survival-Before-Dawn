using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storage_InteractableItem : InteractableItem
{
    [Header("A variant of Interaction Settings that stores and destroys the Interactable object")]
    public ItemSettings itemSettings;

    void Awake() { interactTxt = "Open"; }

    public override void OnInteract()
    {
        base.OnInteract();
        StorageManager.instance.InteractWithStorage(transform.position);
    }
}