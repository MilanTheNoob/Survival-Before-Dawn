public class Structure_InteractableItem : InteractableItem
{
    // Set some vars before Start with Awake
    void Awake() { interactTxt = "Demolish"; toolType = ToolsManager.ToolType.Hammer; }

    // Destroy ourselves if we interacted with
    public override void OnInteract()
    {
        // Delete the save file of us
        for (int i = 0; i < SavingManager.SaveFile.structures.Count; i++)
        {
            if (SavingManager.SaveFile.structures[i].pos == transform.position)
                SavingManager.SaveFile.structures.RemoveAt(i);
        }

        // Destroy ourselves
        Destroy(gameObject);
    }
}
