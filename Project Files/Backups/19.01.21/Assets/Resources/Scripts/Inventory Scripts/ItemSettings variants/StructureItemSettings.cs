using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom Scriptable Objects/Inventory System/Structure Item Settings")]
public class StructureItemSettings : ItemSettings
{
    // Set isUsableItem & dontDrop to true on Awake
    private void Awake() { isUsableItem = true; dontDrop = true; }
    // Time to build boi on Use!
    public override void Use() { BuildingManager.instance.NewBuild(this); }
}
