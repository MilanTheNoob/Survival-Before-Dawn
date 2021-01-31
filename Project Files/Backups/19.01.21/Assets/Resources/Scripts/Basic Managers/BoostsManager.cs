using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostsManager : MonoBehaviour
{
    public ItemBoosts[] itemBoosts;

    // Start is called before the first frame update
    void Start()
    {
        // Add to inventory callback
        Inventory.instance.onItemChangedCallback += CheckForItems;
    }

    // Called to check if we have boost items
    void CheckForItems()
    {
        // Loop through all boost items
        for (int i = 0; i < itemBoosts.Length; i++)
        {
            // Update if there have been changes
            if (!itemBoosts[i].inInventory && Inventory.instance.items.Contains(itemBoosts[i].item))
            {
                PlayerMovement.instance.speed += itemBoosts[i].speedBoost;
                PlayerMovement.instance.jumpHeight += itemBoosts[i].jumpBoost;
                itemBoosts[i].inInventory = true;
            }
            else if (itemBoosts[i].inInventory && !Inventory.instance.items.Contains(itemBoosts[i].item))
            {
                PlayerMovement.instance.speed -= itemBoosts[i].speedBoost;
                PlayerMovement.instance.jumpHeight -= itemBoosts[i].jumpBoost;
                itemBoosts[i].inInventory = false;
            }
        }
    }

    [System.Serializable]
    public class ItemBoosts
    {
        public ItemSettings item;
        public float speedBoost;
        public float jumpBoost;

        [HideInInspector]
        public bool inInventory;
    }
}
