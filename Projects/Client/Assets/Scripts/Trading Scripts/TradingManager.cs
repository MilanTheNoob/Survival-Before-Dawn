using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class TradingManager : MonoBehaviour
{
    #region Singleton

    public static TradingManager instance;

    void Awake()
    {
        instance = this;
    }

    #endregion

    public List<ItemSettings> spawnableItems;
    public int invSize = 20;

    [Space(20)]
    
    public Button closeButton;
    public Button tradeButton;
    public Text priceTxt;
    public Text fundsTxt;

    [Space(20)]

    public InventorySlot[] playerSlots;
    public InventorySlot[] traderSlots;

    public enum SwapType
    {
        None,
        ToPlayer,
        ToTrader
    }

    public enum OriginType
    {
        None,
        Player,
        Trader
    }

    [HideInInspector]
    public float tradeCost;
    [HideInInspector]
    public bool isTrading;
    [HideInInspector]
    public Trader_InteractableItem currentTrader;  

    List<ItemSettings> previewPlayerInv = new List<ItemSettings>();
    List<ItemSettings> previewTraderInv = new List<ItemSettings>();

    List<string> originPreviewPlayerInv = new List<string>();
    List<string> originPreviewTraderInv = new List<string>();
    /*
    // Update is called once per frame
    void Update()
    {
        
        // Loop through the player slots
        for (int i = 0; i < playerSlots.Length; i++)
        {
            // Preview Trade if the button has been clicked
            //if (playerSlots[i].button.onClicked)
                //(playerSlots[i]);
        }

        // Loop through the trader slots
        for (int i = 0; i < traderSlots.Length; i++)
        {
            // Preview Trade if the button has been clicked
            //if (traderSlots[i].button.onClicked)
                //PreviewTrade(traderSlots[i]);
        }

        // Has the close button been clicked? If so then we ain't tradin
        //if (closeButton.onClicked)
            //isTrading = false;

        // Check if we are trading
        if (isTrading)
        {
            // Enable the price text & trade button
            priceTxt.transform.parent.gameObject.SetActive(true);
            tradeButton.gameObject.SetActive(true);

            // Has the trade button been clicked?
            if (tradeButton.onClicked)
            {
                // If the player can't afford the transaction
                if (tradeCost > 0 && SavingManager.SaveFile.funds < tradeCost)
                {
                    // Reset trading
                    ResetTrading();
                    // Tell the player
                    priceTxt.text = "Not enough Funds";
                    // Reset the text after a while
                    StartCoroutine(resetTxtAfterDelay("", 1));
                } 
                // If the player can afford it
                else if (SavingManager.SaveFile.funds >= tradeCost)
                {
                    // Subtract the cost from the players money
                    SavingManager.SaveFile.funds -= tradeCost;

                    // Remove everything from the player's & trader's inventory
                    Inventory.instance.DestroyAll();
                    currentTrader.inventory.Clear();

                    // Loop through all slots of the associated inventories and add the correct items
                    for (int i = 0; i < previewPlayerInv.Count; i++)
                    {
                        Inventory.instance.items.Add(previewPlayerInv[i]);
                    }
                    for (int i = 0; i < previewTraderInv.Count; i++)
                    {
                        currentTrader.inventory.Add(previewTraderInv[i]);
                    }

                    // Reset Trading
                    ResetTrading();

                    // Tell the player
                    priceTxt.text = "Traded!";
                    // Reset the text after 5 seconds
                    StartCoroutine(resetTxtAfterDelay("", 5));
                }
            }
        }
    }

    // Called to randomize the given inventory
    public void RandomizeTraderInventory(Trader_InteractableItem traderII)
    {
        // Loop through a random amount of inventory slots
        for (int i = 0; i < Random.Range(1, invSize); i++)
        {
            // Add a random item to 
            traderII.inventory.Add(spawnableItems[Random.Range(0, spawnableItems.Count)]);
        }
    }

    // Called to start interaction with a trader
    public void TradeWithTrader(Trader_InteractableItem traderII)
    {
        // Set the current trader
        currentTrader = traderII;
        // Reset trading
        ResetTrading();
    }

    // Here to preview a trade action
    public void PreviewTrade(InventorySlot inventorySlot)
    {
        // Check if the item is null
        if (inventorySlot.itemSettings == null)
            return;

        // Check what swap type this preview transaction is
        if (inventorySlot.swapType == SwapType.ToPlayer && previewPlayerInv.Count <= Inventory.instance.maxSlots)
        {
            // Where does it origin stem from?
            if (originPreviewTraderInv.Contains("Player-" + inventorySlot.itemSettings.name))
            {
                // Add to the trade cost with the sell value
                tradeCost += inventorySlot.itemSettings.sellValue;

                // Remove the item id from the trader and add it to the player
                originPreviewPlayerInv.Add("Player-" + inventorySlot.itemSettings.name);
                originPreviewTraderInv.Remove("Player-" + inventorySlot.itemSettings.name);
            } 
            else if (originPreviewTraderInv.Contains("Trader-" + inventorySlot.itemSettings.name))
            {
                // Add to the trade coast with the buy value
                tradeCost += inventorySlot.itemSettings.buyValue;

                // Remove the item id from the trader and add it to the player
                originPreviewPlayerInv.Add("Trader-" + inventorySlot.itemSettings.name);
                originPreviewTraderInv.Remove("Trader-" + inventorySlot.itemSettings.name);
            } 
            else { return; }

            // Add the item itself to the player while removing it from the trader
            previewPlayerInv.Add(inventorySlot.itemSettings);
            previewTraderInv.Remove(inventorySlot.itemSettings);

            // Clear the original item slot & its UI
            inventorySlot.ClearSlot();
        } 
        else if (inventorySlot.swapType == SwapType.ToTrader && previewTraderInv.Count <= invSize)
        {
            // Check where the origin of the item is
            if (originPreviewPlayerInv.Contains("Trader-" + inventorySlot.itemSettings.name))
            {
                // Subtract the buy value from the trade cost
                tradeCost -= inventorySlot.itemSettings.buyValue;

                // Add the item id to the trader while removing it from the player
                originPreviewTraderInv.Add("Trader-" + inventorySlot.itemSettings.name);
                originPreviewPlayerInv.Remove("Trader-" + inventorySlot.itemSettings.name);
            } 
            else if (originPreviewPlayerInv.Contains("Player-" + inventorySlot.itemSettings.name))
            {
                // Subtract the sell value from the trade cost
                tradeCost -= inventorySlot.itemSettings.sellValue;

                // Add the item id to the trader while removing it from the player
                originPreviewTraderInv.Add("Player-" + inventorySlot.itemSettings.name);
                originPreviewPlayerInv.Remove("Player-" + inventorySlot.itemSettings.name);
            } 
            else { return; }

            // Remove the item from the player and add it to the trader
            previewPlayerInv.Remove(inventorySlot.itemSettings);
            previewTraderInv.Add(inventorySlot.itemSettings);

            // Clear the inventory slot
            inventorySlot.ClearSlot();
        }

        // If the trade preview is exactly the same as the player's & trader's original inventory
        if (previewPlayerInv == Inventory.instance.items && previewTraderInv == currentTrader.inventory)
        {
            // Disable the basic trade components
            priceTxt.gameObject.transform.parent.gameObject.SetActive(false);
            tradeButton.gameObject.SetActive(false);
        }

        // Update the UI
        UpdatePreviewTrading();
    }

    // Called to update the UI
    void UpdatePreviewTrading()
    {
        // Show how much money the player has
        fundsTxt.text = "You have $" + SavingManager.SaveFile.funds;

        // Check through all the possible values of the trade cost
        if (tradeCost > 0)
        {
            // Tell the player he has to pay
            priceTxt.text = "Pay $" + tradeCost;
        } 
        else if (tradeCost == 0)
        {
            // Tell the player its even
            priceTxt.text = "Pay Nothing";
        }
        else if (tradeCost < 0)
        {
            // Tell the player he gets money
            priceTxt.text = "Get $" + -tradeCost;
        }

        // Loop through all the inventory slots & make sure they show the appropiate values
        for (int i = 0; i < playerSlots.Length; i++)
        {
            if (i < previewPlayerInv.Count) { playerSlots[i].AddItem(previewPlayerInv[i]); } else { playerSlots[i].ClearSlot(); }
        }
        for (int i = 0; i < traderSlots.Length; i++)
        {
            if (i < previewTraderInv.Count) { traderSlots[i].AddItem(previewTraderInv   [i]); } else { traderSlots[i].ClearSlot(); }
        }
    }

    // Called to reset trading
    public void ResetTrading()
    {
        // Clear the preview inventories
        previewPlayerInv.Clear();
        previewTraderInv.Clear();

        // Clear the origin preview inventories
        originPreviewPlayerInv.Clear();
        originPreviewTraderInv.Clear();

        // Set trade cost to 0
        tradeCost = 0;

        // Enable the trading UI
        InputManager.instance.ToggleUISectionsInt(7);

        // Set the price text to nothing
        priceTxt.text = "";

        // Loop through all the preview slots and add the actual values
        for (int i = 0; i < Inventory.instance.items.Count; i++)
        {
            previewPlayerInv.Add(Inventory.instance.items[i]);
            originPreviewPlayerInv.Add("Player-" + previewPlayerInv[i].name);
        }
        for (int i = 0; i < currentTrader.inventory.Count; i++)
        {
            previewTraderInv.Add(currentTrader.inventory[i]);
            originPreviewTraderInv.Add("Trader-" + previewTraderInv[i].name);
        }

        // Update the preview UI
        UpdatePreviewTrading();
 
        // Set isTrading to true
        isTrading = true;
    }

    // Calle to reset the price txt after a delay
    public IEnumerator resetTxtAfterDelay(string txt, int wait)
    {
        // Wait the delay
        yield return new WaitForSeconds(wait);

        // Reset the text to normale
        if (tradeCost > 0)
        {
            priceTxt.text = "Pay $" + tradeCost;
        } 
        else if (tradeCost == 0)
        {
            priceTxt.text = "Pay Nothing";
        }
        else if (tradeCost < 0)
        {
            priceTxt.text = "Get $" + -tradeCost;
        }
    }
    */
}