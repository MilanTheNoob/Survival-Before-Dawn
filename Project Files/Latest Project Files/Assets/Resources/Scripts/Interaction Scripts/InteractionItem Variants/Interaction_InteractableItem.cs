using UnityEngine;

public class Interaction_InteractableItem : InteractableItem
{
    [Header("A variant of Interaction Settings that stores and destroys the Interactable object")]
    public ItemSettings itemSettings;

    void Awake() { interactTxt = "Pick up"; }

    public override void OnInteract() { if (Inventory.instance.Add(itemSettings)) { PropsGeneration.instance.AddToPropPool(gameObject); } }
}
