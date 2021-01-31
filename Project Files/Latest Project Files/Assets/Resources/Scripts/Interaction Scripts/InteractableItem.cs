using UnityEngine;

public class InteractableItem : MonoBehaviour
{
    [Header("Interactable Settings")]
    public ToolsManager.ToolType toolType = ToolsManager.ToolType.None;

    [HideInInspector]
    public string interactTxt;
    [HideInInspector]
    public bool isInteractable;

    // Called at the beginning of the game
    void Start()
    {
        // Set our layer and set isInteractable to true
        gameObject.layer = 9;
        isInteractable = true;

        // If we have a mesh collider set it to be convex
        MeshCollider col = gameObject.GetComponent<MeshCollider>();
        if (col != null) { col.convex = true; }
    }

    // The base OnInteract func
    public virtual void OnInteract() { }
}
