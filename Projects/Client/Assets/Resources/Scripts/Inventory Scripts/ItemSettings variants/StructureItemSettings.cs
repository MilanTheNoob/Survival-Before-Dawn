using UnityEngine;

[CreateAssetMenu(menuName = "Custom Scriptable Objects/Inventory System/Structure Item Settings")]
public class StructureItemSettings : ItemSettings
{
    private void Awake() { isUsableItem = true; dontDrop = true; }
    public override void Use() { BuildingManager.instance.NewBuild(this); }
}
