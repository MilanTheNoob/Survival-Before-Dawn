using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom Scriptable Objects/Map Generation/New Structures Settings")]
public class StructuresSettings : UpdatableSettings
{
    [Header("Basic structure settings")]
    public int pool;
    public int perChunk;

    [Header("All the structures")]
    public StructuresStruct[] StandardBuildings;

    [System.Serializable]
    public class StructuresStruct
    {
        public string name; 
        public GameObject structure;
        public float yOffset;
    }
}
