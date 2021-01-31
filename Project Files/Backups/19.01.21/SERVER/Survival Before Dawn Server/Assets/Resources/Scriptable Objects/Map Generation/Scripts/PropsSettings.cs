using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom Scriptable Objects/Map Generation/New Prop Settings")]
public class PropsSettings : UpdatableSettings
{
    [Header("The different prop settings")]
    public PropsStruct[] BasicTerrainProps;

    [System.Serializable]
    public class PropsStruct
    {
        public string name;
        public GameObject propPrefab;
        public float minSpawnHeight;
        public float maxSpawnHeight;
        public int poolSize;
        public float propsPerChunk;

        public float yOffset;
        public bool randomRotation;
        public Vector3 rotationClamps = new Vector3(360, 360, 30);
    }
}
