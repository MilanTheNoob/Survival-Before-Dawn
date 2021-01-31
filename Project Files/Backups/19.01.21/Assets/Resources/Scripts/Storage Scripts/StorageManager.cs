using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageManager : MonoBehaviour
{
    #region Singleton

    public static StorageManager instance;

    private void Awake()
    {
        instance = this;
    }

    #endregion

    public StorageSlot[] playerSlots;
    public StorageSlot[] chestSlots;

    public List<ItemSettings> currentStorage;

    int maxStorageSize = 20;
    bool isStoring;
    Vector3 currentPos;

    public void ToStorage(int slot)
    {
        if (SavingManager.SaveFile.storage[currentPos].items.Count < maxStorageSize && isStoring)
        {
            currentStorage.Add(playerSlots[slot].itemSettings);
            UpdateStorageUI();

            Inventory.instance.Destroy(playerSlots[slot].itemSettings);
        }
    }

    public void ToPlayer(int slot)
    {
        if (Inventory.instance.items.Count < Inventory.instance.maxSlots && isStoring)
        {
            Inventory.instance.Add(chestSlots[slot].itemSettings);

            currentStorage.RemoveAt(slot);
            UpdateStorageUI();
        }
    }

    public void UpdateStorageUI()
    {
        for (int i = 0; i < chestSlots.Length; i++) { chestSlots[i].ClearSlot(); }
        for (int i = 0; i < currentStorage.Count; i++) { chestSlots[i].AddItem(currentStorage[i]); }
    }

    public void InteractWithStorage(Vector3 pos)
    {
        if (isStoring)
            return;

        InputManager.instance.ToggleUISectionsInt(12);

        for (int i = 0; i < chestSlots.Length; i++) { chestSlots[i].ClearSlot(); }

        if (SavingManager.SaveFile.storage.ContainsKey(pos))
        {
            for (int i = 0; i < SavingManager.SaveFile.storage[pos].items.Count; i++)
            {
                try { currentStorage.Add(Resources.Load<ItemSettings>("Prefabs/Interactable Items/" + SavingManager.SaveFile.storage[pos].items[i])); chestSlots[i].AddItem(currentStorage[i]); } catch { }
            }
        }
        else
        {
            SavingManager.SaveFile.storage.Add(pos, new SavingManager.StorageData());
            currentStorage = new List<ItemSettings>();
        }

        isStoring = true;
        currentPos = pos;
    }

    public void StopInteractWithStorage()
    {
        isStoring = false;
        InputManager.instance.ToggleUISectionsInt(0);

        SavingManager.SaveFile.storage[currentPos].items.Clear();
        for (int i = 0; i < currentStorage.Count; i++) { SavingManager.SaveFile.storage[currentPos].items.Add(currentStorage[i].name); print(currentStorage.Count); }
        currentStorage.Clear();

        currentPos = new Vector3();
    }
}
