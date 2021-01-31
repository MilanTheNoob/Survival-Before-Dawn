using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom Scriptable Objects/Inventory System/Item Settings")]
[System.Serializable]
public class ItemSettings : ScriptableObject
{
    [Header("Name of item")]
    public string desc;
    [Header("Long name")]
    public string descName;

    [Header("The icon")]
    public Sprite icon;

    [Header("GameObjbect for the item")]
    public GameObject gameObject;

    [Header("The description of the item")]
    public string description;

    [Header("How much does it cost to sell/buy with a trader")]
    public float sellValue;
    public float buyValue;

    [HideInInspector]
    public TradingManager.OriginType originType;
    [HideInInspector]
    public bool isUsableItem;
    [HideInInspector]
    public bool dontDrop = false;

    // Called when the item is used
    public virtual void Use() { }
}
