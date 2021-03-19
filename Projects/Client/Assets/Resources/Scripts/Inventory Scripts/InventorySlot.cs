using UnityEngine.UI;
using UnityEngine;

public class InventorySlot : MonoBehaviour
{
    [Header("UI components")]
    public Image icon;
    public GameObject removeButton;
    public Text objectTitle;

    public TradingManager.SwapType swapType;
    [HideInInspector]
    public ItemSettings itemSettings;
    [HideInInspector]
    public Button button;
    [HideInInspector]
    public bool dontUse;

    void Start() { button = gameObject.GetComponentInChildren<Button>(); }

    public virtual void AddItem(ItemSettings newItemSettings)
    {
        if (newItemSettings == null) { ClearSlot(); return; }
        itemSettings = newItemSettings;

        if (icon != null)
        {
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

    public virtual void ClearSlot()
    {
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

    public virtual void OnRemoveButton() { Inventory.instance.Remove(itemSettings); }
    public virtual void UseItem() { if (itemSettings != null && !dontUse) { DetailsManager.instance.SetItem(itemSettings); AudioManager.PlayEquip(); } }
}
