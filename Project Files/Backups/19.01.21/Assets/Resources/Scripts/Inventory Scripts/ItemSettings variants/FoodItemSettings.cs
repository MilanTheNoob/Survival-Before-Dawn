using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom Scriptable Objects/Inventory System/Food Item Settings")]
public class FoodItemSettings : ItemSettings
{
    [Header("How much does the item restore health & hunger by")]
    public float healthChange = 0.05f;
    public float hungerChange = 0.4f;

    // Set isUsableItm to true on Awake
    private void Awake() { isUsableItem = true; }

    // Called to use the food
    public override void Use()
    {
        // Call the base func
        base.Use();

        // Modify the player vitals
        VitalsManager.instance.ModifyVitalAmount(0, healthChange);
        VitalsManager.instance.ModifyVitalAmount(1, hungerChange);

        // Destroy the item in the inventory
        Inventory.instance.Destroy(this);
    }
}
