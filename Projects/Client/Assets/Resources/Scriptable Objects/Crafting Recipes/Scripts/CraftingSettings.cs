using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom Scriptable Objects/Crafting System/New Crafting Settings")]
public class CraftingSettings : ScriptableObject
{
    [Header("The recipe name")]
    public string recipeName;
    [Header("The recipe description")]
    public string recipeDesc;
    [Header("Recipe variants")]
    public CraftingVariant[] recipes;

    bool dontCraft = false;

    public bool CanCraft()
    {
        for (int i = 0; i < recipes.Length; i++) { if (recipes[i].CanCraft()) { return true; } }
        return false;
    }

    public CraftingVariant GetCraftableVariant()
    {
        for (int i = 0; i < recipes.Length; i++) { if (recipes[i].CanCraft()) { return recipes[i]; } }
        return null;
    }

    public void Craft()
    {
        for (int i = 0; i < recipes.Length; i++) { if (recipes[i].Craft()) { return; } }
    }
}

[System.Serializable]
public class CraftingVariant
{
    public ItemSettings[] Input;
    public ItemSettings[] Tools;
    public ItemSettings[] Output;

    public bool CanCraft()
    {
        List<ItemSettings> tempInventory = new List<ItemSettings>(Inventory.instance.items);

        for (int i = 0; i < Input.Length; i++)
        {
            if (!tempInventory.Contains(Input[i])) { return false; }
            tempInventory.Remove(Input[i]);
        }
        for (int i = 0; i < Tools.Length; i++)
        {
            if (!Inventory.instance.items.Contains(Tools[i])) { return false; }
            tempInventory.Remove(Output[i]);
        }

        return true;
    }

    public bool Craft()
    {
        if (CanCraft())
        {
            if (SavingManager.GameState == SavingManager.GameStateEnum.Singleplayer)
            {
                for (int i = 0; i < Input.Length; i++) Inventory.instance.Destroy(Input[i]);

                for (int i = 0; i < Output.Length; i++)
                {
                    if (Output[i].name == "ruby-crystal") { GPlayManager.instance.AddCraftedRubies(); }

                    if (!Inventory.instance.Add(Output[i]))
                    {
                        GameObject item = UnityEngine.Object.Instantiate(Output[i].gameObject, SavingManager.player.transform.position, Quaternion.identity);
                        item.AddComponent<Rigidbody>();
                    }
                }
                return true;
            }
            else if (SavingManager.GameState == SavingManager.GameStateEnum.Multiplayer)
            {
                ClientSend.Craft(this);
            }
        }
        return false;
    }
}
