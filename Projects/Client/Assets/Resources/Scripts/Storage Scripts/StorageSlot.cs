using UnityEngine.UI;
using UnityEngine;

public class StorageSlot : MonoBehaviour
{
    public Image icon;

    public ItemSettings itemSettings;

    // Used to add a new item to the slot
    public virtual void AddItem(ItemSettings newItemSettings)
    {
        // Clears the slot if we got null
        if (newItemSettings == null) { ClearSlot(); return; }

        // Add the new itemSettings to the variable up above
        itemSettings = newItemSettings;

        // Set all the UI peices

        if (icon != null)
        {
            // Set all the icon, title and remove button settings correctly
            icon.sprite = itemSettings.icon;
            icon.enabled = true;
        }
    }

    // Used to clear the inventory slot
    public virtual void ClearSlot()
    {
        // Remove the item settings
        itemSettings = null;

        if (icon != null)
        {
            icon.sprite = null;
            icon.enabled = false;
        }
    }
}
