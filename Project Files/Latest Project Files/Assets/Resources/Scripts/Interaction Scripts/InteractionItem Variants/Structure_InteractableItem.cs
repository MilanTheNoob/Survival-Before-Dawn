public class Structure_InteractableItem : InteractableItem
{
    void Awake() { interactTxt = "Demolish"; toolType = ToolsManager.ToolType.Hammer; }

    public override void OnInteract()
    {
        for (int i = 0; i < SavingManager.SaveFile.structures.Count; i++) { if (SavingManager.SaveFile.structures[i].pos == transform.position) { SavingManager.SaveFile.structures.RemoveAt(i); } }
        Destroy(gameObject);
    }
}
