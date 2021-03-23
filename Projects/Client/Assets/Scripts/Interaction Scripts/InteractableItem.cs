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
        gameObject.layer = 9;
        isInteractable = true;

        MeshCollider col = gameObject.GetComponent<MeshCollider>();
        if (col != null) { col.convex = true; }
    }

    // The base OnInteract func
    public virtual void OnInteract() { }
}
