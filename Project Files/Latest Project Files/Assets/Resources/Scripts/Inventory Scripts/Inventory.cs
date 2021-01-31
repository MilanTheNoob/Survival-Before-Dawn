using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallback;

    [Header("Slots")]
    public InventorySlot[] slots;
    public InventorySlot[] craftingSlots;

    [Header("Multiplayer Components")]
    public Button interactButton;
    public Text interactText;
    public Text interactNameText;

    [HideInInspector]
    public int maxSlots = 14;
    [HideInInspector]
    public List<ItemSettings> items = new List<ItemSettings>();

    void Awake()
    {
        instance = this;
        onItemChangedCallback += UpdateUI;

        if (SavingManager.GameState == SavingManager.GameStateEnum.Singleplayer)
        {
            items.Clear();
            for (int i = 0; i < SavingManager.SaveFile.inventoryItems.Count; i++) { items.Add(Resources.Load<ItemSettings>("Prefabs/Interactable Items/" + SavingManager.SaveFile.inventoryItems[i])); }

            StartCoroutine(CallbackAfterStartI());
        }
    }

    void FixedUpdate() { if (interactButton.onClicked) { Interact(); } }

    public bool Add(ItemSettings itemSettings)
    {
        if (items.Count <= maxSlots)
        {
            items.Add(itemSettings);
            onItemChangedCallback.Invoke();

            SavingManager.SaveFile.inventoryItems.Add(itemSettings.name);

            return true;
        }
        else { return false; }
    }

    public void Remove(ItemSettings itemSettings)
    {
        if (!items.Contains(itemSettings)) { return; }
        items.Remove(itemSettings);

        if (itemSettings.gameObject != null && !itemSettings.dontDrop)
        {
            Vector3 pPos = InputManager.instance.player.transform.position;
            GameObject item = Instantiate(itemSettings.gameObject, new Vector3(pPos.x, pPos.y + 5, pPos.z), Quaternion.identity);
            item.transform.name = itemSettings.gameObject.transform.name;

            item.AddComponent<Rigidbody>();
            try { item.GetComponent<MeshCollider>().convex = true; } catch { }
        }

        SavingManager.SaveFile.inventoryItems.Remove(itemSettings.name);
        onItemChangedCallback.Invoke();
    }

    public void Destroy(ItemSettings itemSettings)
    {
        SavingManager.SaveFile.inventoryItems.Remove(itemSettings.name);
        items.Remove(itemSettings);
        onItemChangedCallback.Invoke();
    }

    public void DestroyAll()
    {
        SavingManager.SaveFile.inventoryItems.Clear();
        items.Clear();
        onItemChangedCallback.Invoke();
    }

    IEnumerator CallbackAfterStartI() { yield return 0; onItemChangedCallback.Invoke(); }

    public void UpdateUI()
    {
        if (SavingManager.GameState == SavingManager.GameStateEnum.Singleplayer)
        {
            for (int i = 0; i < slots.Length; i++) { slots[i].ClearSlot(); craftingSlots[i].ClearSlot(); StorageManager.instance.playerSlots[i].ClearSlot(); }
            for (int i = 0; i < items.Count; i++) { slots[i].AddItem(items[i]); craftingSlots[i].AddItem(items[i]); StorageManager.instance.playerSlots[i].AddItem(items[i]); }
        }
        else if (SavingManager.GameState == SavingManager.GameStateEnum.Multiplayer)
        {
            for (int i = 0; i < slots.Length; i++) { slots[i].ClearSlot(); craftingSlots[i].ClearSlot(); }
            for (int i = 0; i < items.Count; i++) { slots[i].AddItem(items[i]); craftingSlots[i].AddItem(items[i]); }
        }
    }

    #region Multiplayer Code

    public static void StartInteract(Packet _packet)
    {
        TweeningLibrary.FadeIn(instance.interactButton.gameObject, 0.2f);
        instance.interactText.text = _packet.ReadString();
        instance.interactNameText.text = _packet.ReadString();
    }

    public void Interact()
    {
        using (Packet _packet = new Packet((int)ClientPackets.interact)) { ClientSend.SendTCPData(_packet); }

        TweeningLibrary.FadeOut(instance.interactButton.gameObject, 0.2f);
        instance.interactText.text = "";
        instance.interactNameText.text = "";
    }

    public static void StopInteract(Packet _packet)
    {
        TweeningLibrary.FadeOut(instance.interactButton.gameObject, 0.2f);
        instance.interactText.text = "";
        instance.interactNameText.text = "";
    }

    public static void UpdateInventory(Packet _packet)
    {
        print("update inventory");
        int count = _packet.ReadInt();

        instance.items.Clear();
        for (int i = 0; i < count; i++) { instance.items.Add(Resources.Load<ItemSettings>("Prefabs/Interactable Items/" + _packet.ReadString())); }
        instance.onItemChangedCallback.Invoke();
    }

    #endregion
}