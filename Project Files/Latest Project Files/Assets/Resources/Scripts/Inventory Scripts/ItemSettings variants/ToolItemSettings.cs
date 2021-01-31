using UnityEngine;

[CreateAssetMenu(menuName = "Custom Scriptable Objects/Inventory System/Tool Item Settings")]
public class ToolItemSettings : ItemSettings
{
    private void Awake() { isUsableItem = true; }
    public override void Use() { ToolsManager.instance.EquipTool(this); InputManager.instance.ToggleUISectionsInt(0); }
}
