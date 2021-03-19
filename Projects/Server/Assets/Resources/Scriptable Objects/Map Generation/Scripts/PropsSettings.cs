using UnityEngine;

[CreateAssetMenu(menuName = "Custom Scriptable Objects/Map Generation/New Prop Settings")]
public class PropsSettings : UpdatableSettings
{
    public PropsGroupStruct[] PropGroups;
    public BiomesStruct[] Biomes;

    [System.Serializable]
    public class PropsGroupStruct
    {
        public string groupName;
        public PropsStruct[] Props;
    }

    [System.Serializable]
    public class PropsStruct
    {
        public GameObject prop;

        public float yOffset = -0.2f;
        public Vector3 rotationClamps = new Vector3(180, 180, 30);
    }

    [System.Serializable]
    public class BiomePropsStruct
    {
        public string name;
        public int groupId;
        public int propId;

        public int propsPerChunk;
    }

    [System.Serializable]
    public class BiomeGroupsStruct
    {
        public string groupName;
        public BiomePropsStruct[] Props;
    }

    [System.Serializable]
    public class BiomesStruct
    {
        public string name;
        public BiomeGroupsStruct[] props;
    }
}
