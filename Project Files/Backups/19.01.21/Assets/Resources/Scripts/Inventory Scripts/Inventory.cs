using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    #region Singleton

    public static Inventory instance;

    #endregion

    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallback;

    public ItemSettings emptyItemSettings;

    public int maxSlots = 14;
    public List<ItemSettings> items = new List<ItemSettings>();

    // Called at the beginning of the game
    void Awake()
    {
        // Set the singleton
        instance = this;

        LoadInventoryData();
    }

    // Checks for any empty spaces
    void CheckEmpty()
    {
        // Loop through all items and remove empty spaces
        for (int i = 0; i < items.Count; i++) { if (items[i] == null) { items.RemoveAt(i); } }
    }

    // Add a new ItemSettings to the inventory list
    public bool Add(ItemSettings itemSettings)
    {
        if (items.Count < maxSlots) 
        {
            // Add the item
            items.Add(itemSettings);
            // Invoke the changed Callback
            onItemChangedCallback.Invoke();

            // Add the item to the Inventory SaveData
            SavingManager.SaveFile.inventoryItems.Add(itemSettings.name);
            
            // Return true
            return true;
            
        } else { return false; }
    }

    // Called to Remove an item
    public void Remove(ItemSettings itemSettings)
    {
        // Remove the item from the list
        items.Remove(itemSettings);
        
        if (itemSettings.gameObject != null && !itemSettings.dontDrop)
        {
            // Instantiate the object and set the name correctly
            Vector3 pPos = InputManager.instance.player.transform.position;
            GameObject item = Instantiate(itemSettings.gameObject, new Vector3(pPos.x, pPos.y + 5, pPos.z), Quaternion.identity);
            item.transform.name = itemSettings.gameObject.transform.name;

            // Add a rigidbody to it
            item.AddComponent<Rigidbody>();
            try { item.GetComponent<MeshCollider>().convex = true; } catch (Exception ex) { }
        }

        // Remove the item to the Inventory SaveData
        SavingManager.SaveFile.inventoryItems.Remove(itemSettings.name);

        // Invoke the onItemChanged Callback
        onItemChangedCallback.Invoke();
    }

    // Called to simply destroy the item and not drop it
    public void Destroy(ItemSettings itemSettings)
    {
        // Remove the item to the Inventory SaveData
        SavingManager.SaveFile.inventoryItems.Remove(itemSettings.name);
        // Remove the item from the list
        items.Remove(itemSettings);
        // Invoke the changed Callback
        onItemChangedCallback.Invoke();
    }

    // Called to simply destroy everything
    public void DestroyAll()
    {
        // Remove everything from the Inventory SaveData
        SavingManager.SaveFile.inventoryItems.Clear();
        // Remove everything from the list
        items.Clear();
        // Invoke the changed Callback
        onItemChangedCallback.Invoke();
    }

    // Called to load the inventory data
    public void LoadInventoryData()
    {
        // Clear the Items before adding new ones
        items.Clear();

        // Loop through all the items and to list
        for (int i = 0; i < SavingManager.SaveFile.inventoryItems.Count; i++) { items.Add(Resources.Load<ItemSettings>("Prefabs/Interactable Items/" + SavingManager.SaveFile.inventoryItems[i])); }

        // Remove empty spaces
        CheckEmpty();
        // Invoke the onItemChanged Callback
        StartCoroutine(callbackAfterStart());
    }

    // This here to an annoying feature of callback, LEAVE AS IS
    IEnumerator callbackAfterStart() { yield return 0; onItemChangedCallback.Invoke(); }
}