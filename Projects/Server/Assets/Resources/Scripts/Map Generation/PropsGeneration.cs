using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PropsGeneration : MonoBehaviour
{
    public static PropsGeneration instance;
    void Awake() { instance = this; }

    public PropsSettings propsSettings;

    List<Vector2> generatedChunks = new List<Vector2>();

    public void Generate(TerrainChunk chunk)
    {
        if (generatedChunks.Contains(chunk.coord)) { return; }
        generatedChunks.Add(chunk.coord);

        List<Vector3> vertices = chunk.mesh.vertices.ToList();

        if (NetworkManager.instance.chunkDatas.ContainsKey(chunk.coord))
        {
            chunk.chunkData = NetworkManager.instance.chunkDatas[chunk.coord];

            for (int i = 0; i < chunk.chunkData.props.Count; i++)
            {
                if (!chunk.propsDict.ContainsKey(chunk.chunkData.props.ElementAt(i).Key))
                {
                    GameObject g = Instantiate(propsSettings.PropGroups[chunk.chunkData.props.ElementAt(i).Value.group].Props[chunk.chunkData.props.ElementAt(i).Value.prop].prop);
                    g.transform.name = propsSettings.PropGroups[chunk.chunkData.props.ElementAt(i).Value.group].Props[chunk.chunkData.props.ElementAt(i).Value.prop].prop.name;
                    g.transform.parent = chunk.props.transform;
                    g.transform.localPosition = chunk.chunkData.props.ElementAt(i).Key;
                    g.transform.eulerAngles = chunk.chunkData.props.ElementAt(i).Value.rot;

                    chunk.propsDict.Add(g.transform.position, g);
                }
            }

            for (int i = 0; i < chunk.chunkData.structures.Count; i++)
            {
                GameObject prefab = Resources.Load<GameObject>("Prefabs/Non Interactables/" + chunk.chunkData.structures.ElementAt(i).Value.structure);

                GameObject g = Instantiate(prefab);
                g.transform.name = prefab.name;
                g.transform.parent = chunk.structures.transform;
                g.transform.localPosition = chunk.chunkData.structures.ElementAt(i).Key;
                g.transform.eulerAngles = chunk.chunkData.structures.ElementAt(i).Value.rot;

                chunk.structuresDict.Add(g.transform.position, g);
            }
        }
        else
        {
            chunk.chunkData = new ChunkDataStruct
            {
                coord = chunk.coord,
                heightMap = chunk.heightData
            };

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
                        if (!chunk.propsDict.ContainsKey(chunk.props.transform.TransformPoint(pos)))
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
                            g.transform.parent = chunk.props.transform;
                            g.transform.localPosition = pos;
                            g.transform.eulerAngles = euler;

                            chunk.propsDict.Add(g.transform.position, g);
                            chunk.chunkData.props.Add(pos, prop);
                        }
                    }
                }
            }
        }
    }
}