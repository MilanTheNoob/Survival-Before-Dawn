using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom Scriptable Objects/Map Generation/New Prop Settings")]
public class PropsSettings : UpdatableSettings
{
    public PropsGroupStruct[] PropGroups;
    public BiomesStruct[] Biomes;

    [System.Serializable]
    public class PropsGroupStruct
    {
        public string Name;
        public PropsStruct[] Props;
    }

    [System.Serializable]
    public class PropsStruct
    {
        public GameObject[] PrefabVariants;
        
        [Space]

        public float YOffset = -0.2f;
        public Vector3 RotationClamp = new Vector3(5, 180, 5);
    }

    [System.Serializable]
    public class BiomePropStruct
    {
        public string Name;
        public int Group;
        public int Prop;

        [Space]

        public int PerChunk;

        [Space]

        public float SizeMax = 2.3f;
        public float SizeMin = 0.8f;
    }

    [System.Serializable]
    public class BiomesStruct
    {
        public string Name;
        public BiomePropStruct[] props;
    }
}
