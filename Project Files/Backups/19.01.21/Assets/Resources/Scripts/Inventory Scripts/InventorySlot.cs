using UnityEngine.UI;
using UnityEngine;

public class InventorySlot : MonoBehaviour
{
    [Header("The UI peices")]
    public Image icon;
    public GameObject removeButton;
    public Text objectTitle;

    [Header("The slot type (only needed for trading)")]
    public TradingManager.SwapType swapType;

    [HideInInspector]
    public ItemSettings itemSettings;
    [HideInInspector]
    public Button button;
    [HideInInspector]
    public bool dontUse;

    // Called at beginning of the game
    private void Start()
    {
        // Get the button
        button = gameObject.GetComponentInChildren<Button>();
    }

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
        if (objectTitle != null)
        {
            objectTitle.text = itemSettings.desc;
            objectTitle.transform.parent.gameObject.SetActive(true);   
        }
        if (removeButton != null) { removeButton.SetActive(true); }
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

        if (objectTitle != null)
        {
            objectTitle.text = "";
            objectTitle.transform.parent.gameObject.SetActive(false);
        }

        if (removeButton != null)
            removeButton.SetActive(false);
    }

    // Used to remove the item in the inventory slot
    public virtual void OnRemoveButton() { Inventory.instance.Remove(itemSettings); }
    // Used to call whatever function/power the item has
    public virtual void UseItem() { if (itemSettings != null && !dontUse) { DetailsManager.instance.SetItem(itemSettings); AudioManager.PlayEquip(); } }
}
