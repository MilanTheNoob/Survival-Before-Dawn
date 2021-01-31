using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public InventorySlot[] slots;
    public InventorySlot[] craftingSlots;

    Inventory inventory;

    // Start is called before the first frame update
    void Start()
    {
        // Get the Inventory instance and cache it
        inventory = Inventory.instance;
        // Subscribe to the UpdateUI method to the inventory's callback
        inventory.onItemChangedCallback += UpdateUI;
    }

    // Called whenever the onItemChanged Callback is triggered
    void UpdateUI()
    {
        // Clear all the slots
        for (int i = 0; i < slots.Length; i++) { slots[i].ClearSlot(); craftingSlots[i].ClearSlot(); StorageManager.instance.playerSlots[i].ClearSlot(); }
        // Add all the item to the slots
        for (int i = 0; i < inventory.items.Count; i++) { slots[i].AddItem(inventory.items[i]); craftingSlots[i].AddItem(inventory.items[i]); StorageManager.instance.playerSlots[i].AddItem(inventory.items[i]); }
    }
}
