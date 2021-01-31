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

    public bool isVisible;
    public bool useProps;

    public int biome;
    public int lodIndex;

    LODInfo[] detailLevels;
    LODMesh[] lodMeshes;
    int colliderLODIndex;

    HeightMap heightData;
    bool heightMapRecieved;
    int previousLODIndex = -1;
    bool hasSetCollider;
    float maxViewDst;

    HeightMapSettings heightMapSettings;
    MeshSettings meshSettings;

    Transform viewer;

    TerrainGenerator.GenerateType gType;

    public TerrainChunk(Vector2 coord, HeightMapSettings heightMapSettings, MeshSettings meshSettings, LODInfo[] detailLevels, int colliderLODIndex, Transform parent, Transform viewer, Material material, TerrainGenerator.GenerateType gType, int biome)
    {
        this.coord = coord;
        this.detailLevels = detailLevels;
        this.colliderLODIndex = colliderLODIndex;
        this.heightMapSettings = heightMapSettings;
        this.meshSettings = meshSettings;
        this.viewer = viewer;
        this.gType = gType;
        this.biome = biome;

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

        BuildingManager.instance.LoadData();

        SetVisible(false);

        lodMeshes = new LODMesh[detailLevels.Length];

        for (int i = 0; i < detailLevels.Length; i++)
        {
            lodMeshes[i] = new LODMesh(detailLevels[i].lod);
            lodMeshes[i].updateCallback += UpdateTerrainChunk;
            if (i == colliderLODIndex)
            {
                lodMeshes[i].updateCallback += UpdateCollisionMesh;
            }
        }

        maxViewDst = detailLevels[detailLevels.Length - 1].visibleDstThreshold;
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

    Vector2 viewerPosition
    {
        get
        {
            return new Vector2(viewer.position.x, viewer.position.z);
        }
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
                lodIndex = 0;

                for (int i = 0; i < detailLevels.Length - 1; i++)
                {
                    if (viewerDistanceFromNearestEdge > detailLevels[i].visibleDstThreshold)
                    {
                        lodIndex = i + 1;
                    }
                    else { break; }
                }

                if (lodIndex != previousLODIndex)
                {
                    LODMesh lodMesh = lodMeshes[lodIndex];
                    if (lodMesh.hasMesh)
                    {
                        previousLODIndex = lodIndex;
                        if (meshFilter != null)
                            meshFilter.mesh = lodMesh.mesh;
                    }
                    else if (!lodMesh.hasRequestedMesh)
                    {
                        lodMesh.RequestMesh(heightData, meshSettings);
                    }
                }

                if (lodIndex == 0 && gType == TerrainGenerator.GenerateType.Standard)
                {
                    PropsGeneration.instance.Generate(this);
                }
                else if (lodIndex > 0)
                {
                    PropsGeneration.instance.RemoveFromChunk(this);
                }

                if (meshFilter != null)
                    meshCollider.sharedMesh = meshFilter.mesh;
            }
            else
            {
                PropsGeneration.instance.RemoveFromChunk(this);
            }

            if (wasVisible != visible)
            {
                SetVisible(visible);

                onVisibilityChanged?.Invoke(this, visible);
            }
        }
    }

    public void UpdateCollisionMesh()
    {
        if (!hasSetCollider)
        {
            float sqrDstFromViewerToEdge = bounds.SqrDistance(viewerPosition);

            if (sqrDstFromViewerToEdge < detailLevels[colliderLODIndex].sqrVisibleDstThreshold)
            {
                if (!lodMeshes[colliderLODIndex].hasRequestedMesh)
                {
                    lodMeshes[colliderLODIndex].RequestMesh(heightData, meshSettings);
                }
            }

            if (sqrDstFromViewerToEdge < colliderGenerationDistanceThreshold * colliderGenerationDistanceThreshold)
            {
                if (lodMeshes[colliderLODIndex].hasMesh)
                {
                    meshCollider.sharedMesh = lodMeshes[colliderLODIndex].mesh;
                    hasSetCollider = true;
                }
            }
        }
    }

    public void RemoveChunk()
    {
        PropsGeneration.instance.RemoveFromChunk(this);

        Object.Destroy(meshObject);

        //TerrainGenerator.instance.terrainChunkDictionary.Remove(coord);
        //TerrainGenerator.instance.visibleTerrainChunks.Remove(this);
    }

    public void SetVisible(bool visible)
    {
        meshObject.SetActive(visible);
    }

    public bool IsVisible()
    {
        if (meshObject != null) { return meshObject.activeSelf; } else { return false; }
    }
}

class LODMesh
{

    public Mesh mesh;
    public bool hasRequestedMesh;
    public bool hasMesh;
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