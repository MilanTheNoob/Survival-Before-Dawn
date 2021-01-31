using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PropsGeneration : MonoBehaviour
{
    public static PropsGeneration instance;
    void Awake() { instance = this; }

    public PropsSettings propsSettings;

    public Dictionary<Vector3, PropDataStruct> Generate(TerrainChunk chunk)
    {
        Dictionary<Vector3, PropDataStruct> props = new Dictionary<Vector3, PropDataStruct>();
        List<Vector3> vertices = chunk.mesh.vertices.ToList();

        for (int j = 0; j < propsSettings.Biomes[0].props.Length; j++)
        {
            for (int k = 0; k < propsSettings.Biomes[0].props[j].Props.Length; k++)
            {
                int propId = propsSettings.Biomes[0].props[j].Props[k].propId;
                int groupId = propsSettings.Biomes[0].props[j].Props[k].groupId;

                for (int i = 0; i < propsSettings.Biomes[0].props[j].Props[k].propsPerChunk; i++)
                {
                    int t = Random.Range(0, vertices.Count);

                    Vector3 pos = new Vector3(vertices[t].x, vertices[t].y + propsSettings.PropGroups[groupId].Props[propId].yOffset, vertices[t].z);
                    if (!props.ContainsKey(pos)) 
                    {
                        vertices.RemoveAt(i);

                        PropDataStruct prop = new PropDataStruct
                        {
                            prop = propId,
                            group = groupId
                        };

                        Vector3 euler = Vector3.zero;
                        euler.x = Random.Range(-5, 5);
                        euler.y = Random.Range(-180, 180);
                        euler.z = Random.Range(-5, 5);
                        prop.rot = euler;

                        GameObject g = Instantiate(propsSettings.PropGroups[groupId].Props[propId].prop);
                        g.transform.name = propsSettings.PropGroups[groupId].Props[propId].prop.name;
                        g.transform.parent = chunk.items.transform;
                        g.transform.localPosition = new Vector3(vertices[t].x, vertices[t].y + propsSettings.PropGroups[groupId].Props[propId].yOffset, vertices[t].z);
                        g.transform.eulerAngles = euler;

                        props.Add(pos, prop);
                    }
                }
            }
        }

        return props;
    }
}