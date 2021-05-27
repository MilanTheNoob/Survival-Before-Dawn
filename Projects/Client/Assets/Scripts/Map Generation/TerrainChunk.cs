using UnityEngine;

public class TerrainChunk
{
    public const float colliderGenerationDistanceThreshold = 5;
    public event System.Action<TerrainChunk, bool> onVisibilityChanged;
    public Vector2 coord;

    public GameObject meshObject;
    public Vector2 sampleCentre;
    public Bounds bounds;

    public MeshRenderer meshRenderer;
    public MeshFilter meshFilter;
    public MeshCollider meshCollider;

    public GameObject props;
    public GameObject items;
    public GameObject structures;
    public GameObject vehicles;

    public bool isVisible;
    public bool useProps;

    public int biome;
    public int lodIndex;

    public HeightMap heightData;
    bool heightMapRecieved;
    float maxViewDst;
    public bool isGenerated;

    HeightMapSettings heightMapSettings;
    MeshSettings meshSettings;

    Transform viewer;
    TerrainGenerator.GenerateType gType;

    LODMesh meshStruct;

    public TerrainChunk(Vector2 coord, HeightMapSettings heightMapSettings, MeshSettings meshSettings, float maxViewDst, Transform parent, Material material, TerrainGenerator.GenerateType gType, int biome)
    {
        this.coord = coord;
        this.heightMapSettings = heightMapSettings;
        this.meshSettings = meshSettings;
        this.viewer = SavingManager.player.transform;
        this.gType = gType;
        this.biome = biome;
        this.maxViewDst = maxViewDst;

        sampleCentre = coord * meshSettings.meshWorldSize / meshSettings.meshScale;
        Vector3 position = coord * meshSettings.meshWorldSize;
        bounds = new Bounds(position, Vector2.one * meshSettings.meshWorldSize);

        meshObject = new GameObject("Terrain Chunk " + position.x + "-" + position.y);
        meshRenderer = meshObject.AddComponent<MeshRenderer>();
        meshFilter = meshObject.AddComponent<MeshFilter>();
        meshCollider = meshObject.AddComponent<MeshCollider>();

        meshRenderer.material = material;

        meshObject.transform.position = new Vector3(position.x, 0, position.y);
        meshObject.transform.parent = parent;

        meshObject.layer = 8;
        meshObject.tag = "TerrainChunk";

        props = new GameObject("Props Holder");
        props.transform.parent = meshObject.transform;
        props.transform.localPosition = Vector3.zero;

        items = new GameObject("Items Holder");
        items.transform.parent = meshObject.transform;
        items.transform.localPosition = Vector3.zero;

        structures = new GameObject("Structure Pieces Holder");
        structures.transform.parent = meshObject.transform;
        structures.transform.localPosition = Vector3.zero;

        vehicles = new GameObject("Vehicles Holder");
        vehicles.transform.parent = meshObject.transform;
        vehicles.transform.localPosition = Vector3.zero;

        BuildingManager.instance.LoadData();
        SetVisible(false);

        meshStruct = new LODMesh(0);
        meshStruct.updateCallback += SetChunkMesh;
    }

    public void Load()
    {
        ThreadedDataRequester.RequestData(() => HeightMapGenerator.GenerateHeightMap(meshSettings.numVertsPerLine, meshSettings.numVertsPerLine, heightMapSettings, sampleCentre), OnHeightMapRecieved);
    }

    void OnHeightMapRecieved(object heightMapObject)
    {
        this.heightData = (HeightMap)heightMapObject;
        heightMapRecieved = true;

        UpdateTerrainChunk();
    }

    public void UpdateTerrainChunk()
    {
        if (heightMapRecieved)
        {
            float viewerDistanceFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));

            bool wasVisible = IsVisible();
            bool visible = viewerDistanceFromNearestEdge <= maxViewDst;

            if (visible)
            {
                if (!meshStruct.hasRequestedMesh)
                {
                    meshStruct.RequestMesh(heightData, meshSettings);
                }
            }

            if (wasVisible != visible)
            {
                SetVisible(visible);
                onVisibilityChanged?.Invoke(this, visible);

                if (!visible)
                {
                    ChunkSaving.SaveChunkData(this);
                    PropsGeneration.instance.RemoveFromChunk(this);
                    meshStruct.setMesh = false;
                }
                else
                {
                    PropsGeneration.instance.Generate(this);
                }
            }
        }
    }

    public void SetChunkMesh()
    {
        meshFilter.mesh = meshStruct.mesh;
        meshCollider.sharedMesh = meshFilter.mesh;

        PropsGeneration.instance.Generate(this);
        meshStruct.setMesh = true;
    }

    Vector2 viewerPosition { get { return new Vector2(viewer.position.x, viewer.position.z); } }
    public void RemoveChunk() { PropsGeneration.instance.RemoveFromChunk(this); Object.Destroy(meshObject); }
    public void UpdateViewDst(float _ViewDst) { maxViewDst = _ViewDst; }
    public void SetVisible(bool visible) { meshObject.SetActive(visible); }
    public bool IsVisible() { if (meshObject != null) { return meshObject.activeSelf; } else { return false; } }
}

class LODMesh
{

    public Mesh mesh;
    public bool hasRequestedMesh;
    public bool hasMesh;
    public bool setMesh;
    int lod;
    public event System.Action updateCallback;

    public LODMesh(int lod)
    {
        this.lod = lod;
    }

    public void OnMeshDataRecieved(object meshDataObject)
    {
        mesh = ((MeshData)meshDataObject).CreateMesh();
        hasMesh = true;

        updateCallback();
    }

    public void RequestMesh(HeightMap heightMap, MeshSettings meshSettings)
    {
        hasRequestedMesh = true;
        ThreadedDataRequester.RequestData(() => MeshGenerator.GenerateTerrainMesh(heightMap.values, meshSettings, lod), OnMeshDataRecieved);
    }
}