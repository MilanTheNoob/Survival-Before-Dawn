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

    // Called to return if this recipe can be crafted using stuff
    // in the players inventory
    public bool CanCraft()
    {
        // Loop through the recipes
        for (int i = 0; i < recipes.Length; i++)
        {
            // return true if we can
            if (recipes[i].CanCraft())
                return true;
        }

        // Return false
        return false;
    }

    // Returns a recipe variant the player can craft
    public CraftingVariant GetCraftableVariant()
    {
        // Loop through the recipes
        for (int i = 0; i < recipes.Length; i++)
        {
            // If we can craft it then return it
            if (recipes[i].CanCraft())
                return recipes[i];
        }

        // Return null
        return null;
    }

    // Called to craft a recipe
    public void Craft()
    {
        // Loop through the recipes and escape if it crafts succesfully
        for (int i = 0; i < recipes.Length; i++) { if (recipes[i].Craft()) { return; } }
    }
}

[System.Serializable]
public class CraftingVariant
{
    [Header("The Input for the recipe")]
    public ItemSettings[] Input;
    [Header("The Tools needed for the recipe (e.g. needed but won't be removed from inventory)")]
    public ItemSettings[] Tools;
    [Header("The Ouput of the recipe")]
    public ItemSettings[] Output;

    // Returns if the recipe can be crafted
    public bool CanCraft()
    {
        // A temp inventory
        List<ItemSettings> tempInventory = new List<ItemSettings>(Inventory.instance.items);

        // Loop through the inputs
        for (int i = 0; i < Input.Length; i++)
        {
            if (!tempInventory.Contains(Input[i]))
                return false;

            // Remove from the temp Inventory
            tempInventory.Remove(Input[i]);
        }

        // Loop through the tools
        for (int i = 0; i < Tools.Length; i++)
        {
            if (!Inventory.instance.items.Contains(Tools[i]))
                return false;

            // Remove from the temp Inventory
            tempInventory.Remove(Output[i]);
        }

        // If we get past the for loops, then return true
        return true;
    }

    // Called to craft the recipe
    public bool Craft()
    {
        if (CanCraft())
        {
            // Destroy the inputs
            for (int i = 0; i < Input.Length; i++) Inventory.instance.Destroy(Input[i]);

            // Add the inputs
            for (int i = 0; i < Output.Length; i++)
            {
                if (!Inventory.instance.Add(Output[i]))
                {
                    // Create a new gameObject (& rigidbody) if the inventory can't store the item
                    GameObject item = UnityEngine.Object.Instantiate(Output[i].gameObject, InputManager.instance.player.transform.position, Quaternion.identity);
                    item.AddComponent<Rigidbody>();
                }
            }

            // Return true
            return true;
        }

        // Return false
        return false;

    }
}
