using UnityEngine;

[CreateAssetMenu(menuName = "Custom Scriptable Objects/Inventory System/Item Settings")]
[System.Serializable]
public class ItemSettings : ScriptableObject
{
    [Header("Name of item")]
    public string desc;
    [Header("Long name")]
    public string descName;

    [Space]

    public Sprite icon;
    public GameObject gameObject;
    public string description;

    [Space]

    public float sellValue;
    public float buyValue;

    [Space]

    public bool ignoreGravity;

    [HideInInspector]
    public TradingManager.OriginType originType;
    [HideInInspector]
    public bool isUsableItem;
    [HideInInspector]
    public bool dontDrop = false;

    public virtual void Use() { }
}
