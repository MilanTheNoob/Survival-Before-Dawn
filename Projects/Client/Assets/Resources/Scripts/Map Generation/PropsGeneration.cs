using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PropsGeneration : MonoBehaviour
{
    public static PropsGeneration instance;

    public PropsSettings DefaultPropSettings;
    public PropsSettings LQPropSettings;

    [HideInInspector]
    public Dictionary<string, PoolData> Pools = new Dictionary<string, PoolData>();
    [HideInInspector]
    public GameObject PropPools;
    [HideInInspector]
    public PropsSettings PropSettings;

    List<Vector2> generatedChunks = new List<Vector2>();

    #region Start Funcs

    void Awake() 
    { 
        instance = this;
        EnableLQGeneration(SavingManager.SaveFile.LQGeneration);
    }
    void Start()
    {
        SavingManager.SaveGameCallback += StoreVisibleChunks;
        PropPools = new GameObject("Pools");

        for (int i = 0; i < PropSettings.PropGroups.Length; i++)
        {
            for (int y = 0; y < PropSettings.PropGroups[i].Props.Length; y++)
            {
                PoolData Pool = new PoolData()
                {
                    PoolHolder = new GameObject(PropSettings.PropGroups[i].Props[y].PrefabVariants[0].transform.name + " Pool")
                };
                Pool.PoolHolder.transform.parent = PropPools.transform;

                for (int l = 0; l < PropSettings.PropGroups[i].Props[y].PrefabVariants.Length; l++)
                {
                    PropPoolData PropPool = new PropPoolData
                    {
                        Holder = new GameObject(PropSettings.PropGroups[i].Props[y].PrefabVariants[l].transform.name + " Prop Pool")
                    };
                    PropPool.Holder.transform.parent = Pool.PoolHolder.transform;

                    for (int j = 0; j < PropSettings.PropGroups[i].Props[y].PoolSizes; j++)
                    {
                        GameObject newObject = Instantiate(PropSettings.PropGroups[i].Props[y].PrefabVariants[l]);

                        newObject.transform.name = PropSettings.PropGroups[i].Props[y].PrefabVariants[l].transform.name;
                        newObject.transform.parent = PropPool.Holder.transform;
                        newObject.SetActive(false);

                        PropPool.Props.Add(newObject);
                    }

                    Pool.PropVariants.Add(PropPool);
                }

                Pools.Add(PropSettings.PropGroups[i].Props[y].PrefabVariants[0].transform.name + " Pool", Pool);
            }
        }
    }

    #endregion

    #region Generating

    public void Generate(TerrainChunk chunk)
    {
        if (generatedChunks.Contains(chunk.coord) || chunk.meshFilter.mesh.vertices.Length < 10) { return; }
        generatedChunks.Add(chunk.coord);

        if (SavingManager.SaveFile.Chunks.ContainsKey(new Vector2(chunk.coord.x, chunk.coord.y)))
        {
            List<PropData> Props = SavingManager.SaveFile.Chunks[chunk.coord].Props;

            for (int i = 0; i < Props.Count; i++)
            {
                if (Pools.ContainsKey(Props[i].Name + " Pool"))
                {
                    PoolData pool = Pools[Props[i].Name + " Pool"];
                    int rand = Random.Range(0, pool.PropVariants.Count);
                    GameObject prop = pool.PropVariants[rand].Props[0];
                    pool.PropVariants[rand].Props.RemoveAt(0);

                    prop.transform.parent = chunk.props.transform;
                    prop.SetActive(true);

                    prop.transform.position = Props[i].Position;
                    prop.transform.rotation = Props[i].Rotation;
                    prop.transform.localScale = Props[i].Scale;
                }
            }
        }
        else
        {
            GPlayManager.instance.AddExploredChunks();

            List<Vector3> vertices = chunk.meshFilter.mesh.vertices.ToList();
            List<Vector3> exclusions = new List<Vector3>();

            Random.InitState(TerrainGenerator.instance.heightMapSettings.noiseSettings.seed + (int)chunk.coord.x + (int)chunk.coord.y);
            int structureRand = Random.Range(1, 20);

            if (structureRand < 3 && !SavingManager.SaveFile.LQGeneration)
            {
                StructureChunk structureData = PropSettings.Structures[Random.Range(0, PropSettings.Structures.Length)];
                GameObject g = Instantiate(structureData.Structure);

                Renderer[] childs = g.GetComponentsInChildren<Renderer>();
                Bounds tempBounds = childs[0].bounds;
                for (int i = 0; i < childs.Length; i++) { if (i != 0) tempBounds.Encapsulate(childs[i].bounds); }

                g.transform.parent = chunk.structures.transform;
                g.transform.localPosition = new Vector3(0, vertices[vertices.Count / 2].y + 0.5f, 0);
                g.transform.eulerAngles = new Vector3(0F, Random.Range(0, 360f), 0f);
                g.SetActive(true);

                for (int i = 0; i < vertices.Count; i++)
                {
                    if (tempBounds.Contains(vertices[i]))
                    {
                        exclusions.Add(vertices[i]);
                        vertices.RemoveAt(i);
                    }
                }

                for (int j = 0; j < structureData.Props.Length; j++)
                {
                    int propId = structureData.Props[j].Prop;
                    int groupId = structureData.Props[j].Group;

                    PoolData pool = Pools[PropSettings.PropGroups[groupId].Props[propId].PrefabVariants[0].transform.name + " Pool"];

                    for (int i = 0; i < structureData.Props[j].PerChunk; i++)
                    {
                        int f = Random.Range(0, pool.PropVariants.Count);
                        int t = Random.Range(0, vertices.Count);

                        if (pool.PropVariants[f].Props.Count > 0 && !exclusions.Contains(vertices[t]))
                        {
                            float s = Random.Range(structureData.Props[j].SizeMin, structureData.Props[j].SizeMax);

                            Vector3 euler = Vector3.zero;
                            euler.x = Random.Range(-PropSettings.PropGroups[groupId].Props[propId].RotationClamp.x, PropSettings.PropGroups[groupId].Props[propId].RotationClamp.x);
                            euler.y = Random.Range(-PropSettings.PropGroups[groupId].Props[propId].RotationClamp.y, PropSettings.PropGroups[groupId].Props[propId].RotationClamp.y);
                            euler.z = Random.Range(-PropSettings.PropGroups[groupId].Props[propId].RotationClamp.z, PropSettings.PropGroups[groupId].Props[propId].RotationClamp.z);

                            GameObject gp = pool.PropVariants[f].Props[0];
                            pool.PropVariants[f].Props.RemoveAt(0);

                            gp.transform.parent = chunk.props.transform;
                            gp.transform.localPosition = new Vector3(vertices[t].x, vertices[t].y + PropSettings.PropGroups[groupId].Props[propId].YOffset, vertices[t].z);
                            gp.transform.eulerAngles = euler;
                            gp.transform.localScale = new Vector3(s, s, s);
                            gp.SetActive(true);

                            exclusions.Add(vertices[t]);
                            vertices.RemoveAt(t);
                        }
                    }
                }
            }
            else
            {
                for (int j = 0; j < PropSettings.Biomes[chunk.biome].props.Length; j++)
                {
                    int propId = PropSettings.Biomes[chunk.biome].props[j].Prop;
                    int groupId = PropSettings.Biomes[chunk.biome].props[j].Group;

                    PoolData pool = Pools[PropSettings.PropGroups[groupId].Props[propId].PrefabVariants[0].transform.name + " Pool"];

                    for (int i = 0; i < PropSettings.Biomes[chunk.biome].props[j].PerChunk; i++)
                    {
                        int f = Random.Range(0, pool.PropVariants.Count);
                        int t = Random.Range(0, vertices.Count);

                        if (pool.PropVariants[f].Props.Count > 0 && !exclusions.Contains(vertices[t]))
                        {
                            float s = Random.Range(PropSettings.Biomes[chunk.biome].props[j].SizeMin, PropSettings.Biomes[chunk.biome].props[j].SizeMax);

                            Vector3 euler = Vector3.zero;
                            euler.x = Random.Range(-PropSettings.PropGroups[groupId].Props[propId].RotationClamp.x, PropSettings.PropGroups[groupId].Props[propId].RotationClamp.x);
                            euler.y = Random.Range(-PropSettings.PropGroups[groupId].Props[propId].RotationClamp.y, PropSettings.PropGroups[groupId].Props[propId].RotationClamp.y);
                            euler.z = Random.Range(-PropSettings.PropGroups[groupId].Props[propId].RotationClamp.z, PropSettings.PropGroups[groupId].Props[propId].RotationClamp.z);

                            GameObject g = pool.PropVariants[f].Props[0];
                            pool.PropVariants[f].Props.RemoveAt(0);

                            g.transform.parent = chunk.props.transform;
                            g.transform.localPosition = new Vector3(vertices[t].x, vertices[t].y + PropSettings.PropGroups[groupId].Props[propId].YOffset, vertices[t].z);
                            g.transform.eulerAngles = euler;
                            g.transform.localScale = new Vector3(s, s, s);
                            g.SetActive(true);

                            exclusions.Add(vertices[t]);
                            vertices.RemoveAt(t);
                        }
                    }
                }
            }
        }
    }

    public void MultiplayerGenerate(MultiplayerTerrainChunk chunk)
    {
        if (generatedChunks.Contains(chunk.coord)) { return; }
        generatedChunks.Add(chunk.coord);

        chunk.propDict.Clear();
        chunk.structureDict.Clear();

        for (int i = 0; i < chunk.chunkData.props.Count; i++)
        {
            GameObject gi = PropSettings.PropGroups[chunk.chunkData.props.ElementAt(i).Value.group].Props[chunk.chunkData.props.ElementAt(i).Value.prop].PrefabVariants[0];

            if (Pools.ContainsKey(gi.transform.name + " Pool"))
            {
                GameObject g = Pools[gi.transform.name + " Pool"].PropVariants[0].Props[0];
                Pools[gi.transform.name + " Pool"].PropVariants[0].Props.RemoveAt(0);

                g.transform.parent = chunk.props.transform;
                g.transform.localPosition = chunk.chunkData.props.ElementAt(i).Key;
                g.transform.eulerAngles = chunk.chunkData.props.ElementAt(i).Value.rot;
                g.SetActive(true);

                chunk.propDict.Add(g.transform.position, g);
            }
        }
        for (int i = 0; i < chunk.chunkData.structures.Count; i++)
        {
            ItemSettings item = Resources.Load<ItemSettings>("Prefabs/Interactable Items/" + chunk.chunkData.structures.ElementAt(i).Value.structure);
            GameObject g = Instantiate(item.gameObject);

            g.transform.name = item.name;
            g.transform.parent = chunk.structures.transform;
            g.transform.position = chunk.chunkData.structures.ElementAt(i).Key;
            g.transform.eulerAngles = chunk.chunkData.structures.ElementAt(i).Value.rot;
            g.SetActive(true);

            chunk.structureDict.Add(g.transform.position, g);
        }
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
        if (!Pools.ContainsKey(child.name + " Pool")) { return; }

        child.position = Vector3.zero;
        child.gameObject.SetActive(false);
        child.parent = Pools[child.name + " Pool"].PropVariants[0].Holder.transform;
        child.transform.localScale = Vector3.one;

        Pools[child.name + " Pool"].PropVariants[0].Props.Add(child.gameObject);
    }
    public void AddToPropPool(GameObject i)
    {
        if (!Pools.ContainsKey(i.name + " Pool")) { return; }

        i.transform.parent = Pools[i.name + " Pool"].PropVariants[0].Holder.transform;
        i.transform.position = new Vector3(0f, -10f, 0f);
        i.transform.rotation = Quaternion.identity;
        //i.SetActive(false);

        Pools[i.name + " Pool"].PropVariants[0].Props.Add(i);
    }

    public void StoreVisibleChunks() 
    {
        for (int i = 0; i < TerrainGenerator.instance.visibleTerrainChunks.Count; i++) 
        { 
            ChunkSaving.SaveChunkData(TerrainGenerator.instance.visibleTerrainChunks[i]); 
        } 
    }

    public int GetRandomBiome() { return Random.Range(0, PropSettings.Biomes.Length); }
    public void EnableLQGeneration(bool e) { if (e) { PropSettings = LQPropSettings; } else { PropSettings = DefaultPropSettings; } }

    #endregion

    public class PoolData
    {
        public GameObject PoolHolder;
        public List<PropPoolData> PropVariants = new List<PropPoolData>();
    }

    public class PropPoolData
    {
        public GameObject Holder;
        public List<GameObject> Props = new List<GameObject>();
    }

}