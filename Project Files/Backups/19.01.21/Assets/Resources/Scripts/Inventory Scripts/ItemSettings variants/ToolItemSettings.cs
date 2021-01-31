using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom Scriptable Objects/Inventory System/Tool Item Settings")]
public class ToolItemSettings : ItemSettings
{
    // Set isUsableItem to true on Awake
    private void Awake() { isUsableItem = true; }
    // Equip a tool on use
    public override void Use() { ToolsManager.instance.EquipTool(this); InputManager.instance.ToggleUISectionsInt(0); }
}
