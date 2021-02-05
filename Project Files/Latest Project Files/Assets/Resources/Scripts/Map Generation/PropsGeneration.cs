using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PropsGeneration : MonoBehaviour
{
    [Header("Scriptable Objects")]
    public PropsSettings propsSettings;
    public StructuresSettings structuresSettings;

    [HideInInspector]
    public Dictionary<string, PoolData> pools = new Dictionary<string, PoolData>();

    [HideInInspector]
    public List<GameObject> structures = new List<GameObject>();
    [HideInInspector]
    public List<GameObject> props = new List<GameObject>();

    [HideInInspector]
    public Dictionary<string, BoxCollider> triggerColliders = new Dictionary<string, BoxCollider>();

    [HideInInspector]
    public GameObject propsPoolParent;
    [HideInInspector]
    public GameObject structuresPoolParent;

    public static PropsGeneration instance;

    List<Vector2> generatedChunks = new List<Vector2>();

    #region Start Funcs

    void Awake() { instance = this; }
    void Start()
    {
        SavingManager.SaveGameCallback += StoreVisibleChunks;

        propsPoolParent = new GameObject("Props Pool");
        structuresPoolParent = new GameObject("Structures Pool");

        for (int i = 0; i < propsSettings.PropGroups.Length; i++)
        {
            for (int y = 0; y < propsSettings.PropGroups[i].Props.Length; y++)
            {
                GameObject poolHolder = new GameObject(propsSettings.PropGroups[i].Props[y].prefab.transform.name + " Pool");
                poolHolder.transform.parent = propsPoolParent.transform;

                PoolData poolData = poolHolder.AddComponent<PoolData>();
                pools.Add(poolHolder.transform.name, poolData);

                for (int j = 0; j < propsSettings.poolSizes; j++)
                {
                    GameObject newObject = Instantiate(propsSettings.PropGroups[i].Props[y].prefab);

                    newObject.transform.name = propsSettings.PropGroups[i].Props[y].prefab.transform.name;
                    newObject.transform.parent = poolHolder.transform;
                    newObject.SetActive(false);

                    poolData.pool.Add(newObject);
                    props.Add(newObject);
                }
            }
        }

        for (int x = 0; x < structuresSettings.StandardBuildings.Length; x++)
        {
            GameObject poolHolder = new GameObject(structuresSettings.StandardBuildings[x].name + " Pool");
            poolHolder.transform.parent = structuresPoolParent.transform;

            PoolData poolData = poolHolder.AddComponent<PoolData>();
            pools.Add(poolHolder.transform.name, poolData);

            for (int y = 0; y < structuresSettings.pool; y++)
            {
                GameObject newObject = Instantiate(structuresSettings.StandardBuildings[x].structure);

                newObject.transform.parent = poolHolder.transform;
                newObject.SetActive(false);

                poolData.pool.Add(newObject);
                structures.Add(newObject);
            }
        }
    }

    #endregion

    #region Generating

    public void Generate(TerrainChunk chunk)
    {
        if (generatedChunks.Contains(chunk.coord) || chunk.meshFilter.mesh.vertices.Length < 14000) { return; }
        generatedChunks.Add(chunk.coord);

        if (SavingManager.SaveFile.chunkData.ContainsKey(new Vector2(chunk.coord.x, chunk.coord.y)))
        {
            ChunkPropData saveData = SavingManager.SaveFile.chunkData[new Vector2(chunk.coord.x, chunk.coord.y)];

            for (int i = 0; i < saveData.propName.Count; i++)
            {
                if (pools.ContainsKey(saveData.propName[i] + " Pool"))
                {
                    PoolData pool = pools[saveData.propName[i] + " Pool"];
                    GameObject prop = pool.pool[0];
                    pool.pool.RemoveAt(0);

                    prop.transform.parent = chunk.props.transform;
                    prop.SetActive(true);

                    prop.transform.position = saveData.position[i];
                    prop.transform.rotation = saveData.rotation[i];
                }
            }
        }
        else
        {
            List<Vector3> vertices = chunk.meshFilter.mesh.vertices.ToList();
            Random.InitState(TerrainGenerator.instance.heightMapSettings.noiseSettings.seed + (int)chunk.coord.x + (int)chunk.coord.y);

            for (int j = 0; j < structuresSettings.perChunk; j++)
            {
                int rn = Random.Range(0, structuresSettings.StandardBuildings.Length);
                int t = Random.Range(0, vertices.Count);

                GameObject g = pools[structuresSettings.StandardBuildings[rn].name + " Pool"].pool[0];
                pools[structuresSettings.StandardBuildings[rn].name + " Pool"].pool.RemoveAt(0);

                g.transform.parent = chunk.props.transform;
                g.transform.localPosition = vertices[t];
                g.transform.eulerAngles = new Vector3(0, 360, 0);
                g.SetActive(true);

                Bounds structureBounds = GetGroupedBounds(g);
                for (int i = 0; i < vertices.Count; i++) { if (structureBounds.Contains(chunk.meshObject.transform.TransformPoint(vertices[i]))) { vertices.RemoveAt(i); } }
            }

            for (int j = 0; j < propsSettings.Biomes[chunk.biome].props.Length; j++)
            {
                for (int k = 0; k < propsSettings.Biomes[chunk.biome].props[j].Props.Length; k++)
                {
                    int propId = propsSettings.Biomes[chunk.biome].props[j].Props[k].propId;
                    int groupId = propsSettings.Biomes[chunk.biome].props[j].Props[k].groupId;

                    PoolData pool = pools[propsSettings.PropGroups[groupId].Props[propId].prefab.transform.name + " Pool"];

                    for (int i = 0; i < propsSettings.Biomes[chunk.biome].props[j].Props[k].propsPerChunk; i++)
                    {
                        int t = Random.Range(0, vertices.Count);

                        Vector3 euler = Vector3.zero;
                        euler.x = Random.Range(-10, 10);
                        euler.y = Random.Range(-180, 180);
                        euler.z = Random.Range(-10, 10);

                        GameObject g =  pool.pool[0];
                        pool.pool.RemoveAt(0);

                        g.transform.parent = chunk.props.transform;
                        g.transform.localPosition = new Vector3(vertices[t].x, vertices[t].y + propsSettings.PropGroups[groupId].Props[propId].yOffset, vertices[t].z);
                        g.transform.eulerAngles = euler;
                        g.SetActive(true);

                        vertices.RemoveAt(i);
                    }
                }
            }

            StoreChunk(chunk);
        }
    }

    public Dictionary<Vector3, GameObject> MultiplayerGenerate(MultiplayerTerrainChunk chunk)
    {
        if (generatedChunks.Contains(chunk.coord)) { return null; }
        generatedChunks.Add(chunk.coord);

        Dictionary<Vector3, GameObject> propG = new Dictionary<Vector3, GameObject>();

        for (int i = 0; i < chunk.chunkData.props.Count; i++)
        {
            GameObject gi = propsSettings.PropGroups[chunk.chunkData.props.ElementAt(i).Value.group].Props[chunk.chunkData.props.ElementAt(i).Value.prop].prefab;

            if (pools.ContainsKey(gi.transform.name + " Pool"))
            {
                GameObject g = pools[gi.transform.name + " Pool"].pool[0];
                pools[gi.transform.name + " Pool"].pool.RemoveAt(0);

                g.transform.parent = chunk.props.transform;
                g.transform.localPosition = chunk.chunkData.props.ElementAt(i).Key;
                g.transform.eulerAngles = chunk.chunkData.props.ElementAt(i).Value.rot;
                g.SetActive(true);

                propG.Add(g.transform.position, g);
            }
        }

        return propG;
    }

    #endregion

    #region References

    public Bounds GetGroupedBounds(GameObject go)
    {
        Renderer[] childs = go.GetComponentsInChildren<Renderer>();
        Bounds tempBounds = childs[0].bounds;

        for (int i = 0; i < childs.Length; i++) { tempBounds.Encapsulate(childs[i].bounds); }
        return tempBounds;
    }

    #endregion

    #region Misc

    public void RemoveFromChunk(TerrainChunk chunk)
    {
        if (chunk.props.transform.childCount == 0 || !generatedChunks.Contains(chunk.coord)) { return; }
        generatedChunks.Remove(chunk.coord);

        StoreChunk(chunk);
        for (int i = 0; i < chunk.props.transform.childCount; i++) { RemoveProp(chunk.props.transform.GetChild(i)); }
    }
    public void RemoveFromChunk(MultiplayerTerrainChunk chunk)
    {
        if (chunk.props.transform.childCount == 0 || !generatedChunks.Contains(chunk.coord)) { return; }
        generatedChunks.Remove(chunk.coord);

        chunk.propDict.Clear();
        for (int i = 0; i < chunk.props.transform.childCount; i++) { RemoveProp(chunk.props.transform.GetChild(i)); }
    }
    public void RemoveProp(Transform child)
    {
        if (!pools.ContainsKey(child.name + " Pool")) { return; }

        child.position = Vector3.zero;
        child.gameObject.SetActive(false);
        child.parent = pools[child.name + " Pool"].transform;

        pools[child.name + " Pool"].pool.Add(child.gameObject);
    }
    public void AddToPropPool(GameObject i)
    {
        if (!pools.ContainsKey(i.name + " Pool")) { return; }

        i.transform.parent = pools[i.name + " Pool"].gameObject.transform;
        i.transform.name = pools[i.name + " Pool"].pool[0].transform.name;
        i.transform.position = new Vector3(0f, -10f, 0f);
        i.transform.rotation = Quaternion.identity;
        //i.SetActive(false);

        pools[i.name + " Pool"].pool.Add(i);
    }

    public void StoreVisibleChunks() { for (int i = 0; i < TerrainGenerator.instance.visibleTerrainChunks.Count; i++) { StoreChunk(TerrainGenerator.instance.visibleTerrainChunks[i]); } }
    public void StoreChunk(TerrainChunk chunk)
    {
        if (!generatedChunks.Contains(chunk.coord)) { return; }

        ChunkPropData chunkData = new ChunkPropData();

        for (int i = 0; i < chunk.props.transform.childCount; i++)
        {
            Transform prop = chunk.props.transform.GetChild(i);

            chunkData.propName.Add(prop.name);
            chunkData.position.Add(prop.position);
            chunkData.rotation.Add(prop.rotation);
        }

        SavingManager.SaveFile.chunkData[chunk.coord] = chunkData;
        if (!SavingManager.SaveFile.chunkData.ContainsKey(chunk.coord)) { SavingManager.SaveFile.chunkData.Add(chunk.coord, chunkData); } else { SavingManager.SaveFile.chunkData[chunk.coord] = chunkData; }
    }

    public int GetRandomBiome() { return Random.Range(0, propsSettings.Biomes.Length); }

    #endregion
}