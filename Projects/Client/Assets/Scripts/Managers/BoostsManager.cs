using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostsManager : MonoBehaviour
{
    public ItemBoosts[] itemBoosts;

    void Start() { Inventory.instance.onItemChangedCallback += CheckForItems; }

    void CheckForItems()
    {
        for (int i = 0; i < itemBoosts.Length; i++)
        {
            if (!itemBoosts[i].inInventory && Inventory.instance.items.Contains(itemBoosts[i].item))
            {
                PlayerMovement.instance.speed += itemBoosts[i].speedBoost;
                PlayerMovement.instance.jumpHeight += itemBoosts[i].jumpBoost;
                itemBoosts[i].inInventory = true;

                GPlayManager.instance.GetRuby();
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
