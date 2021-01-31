using System.Linq;
using System.Collections;
using System.Collections.Generic;
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
        // Apply sum data
        this.coord = coord;
        this.detailLevels = detailLevels;
        this.colliderLODIndex = colliderLODIndex;
        this.heightMapSettings = heightMapSettings;
        this.meshSettings = meshSettings;
        this.viewer = viewer;
        this.gType = gType;
        this.biome = biome;

        // Get the position of the chunk given & set the bounds
        sampleCentre = coord * meshSettings.meshWorldSize / meshSettings.meshScale;
        Vector3 position = coord * meshSettings.meshWorldSize;
        bounds = new Bounds(position, Vector2.one * meshSettings.meshWorldSize);

        // Create a new chunk
        meshObject = new GameObject("Terrain Chunk " + position.x + "-" + position.y);
        // Add a new Mesh Renderer to the chunk
        meshRenderer = meshObject.AddComponent<MeshRenderer>();
        // Add a new Mesh Filter to the chunk
        meshFilter = meshObject.AddComponent<MeshFilter>();
        // Add a new Mssh Collider to the chunk
        meshCollider = meshObject.AddComponent<MeshCollider>();

        // Assign the material
        meshRenderer.material = material;

        // Set the position of the chunk
        meshObject.transform.position = new Vector3(position.x, 0, position.y);
        // Set the parent of the chunk
        meshObject.transform.parent = parent;

        // Set the layer and tag of the chunk
        meshObject.layer = 8;
        meshObject.tag = "TerrainChunk";

        // Make the chunk invisible to the player
        SetVisible(false);

        // Create a new LOD Mesh array
        lodMeshes = new LODMesh[detailLevels.Length];

        for (int i = 0; i < detailLevels.Length; i++)
        {
            // Insert the LOD meshes
            lodMeshes[i] = new LODMesh(detailLevels[i].lod);
            lodMeshes[i].updateCallback += UpdateTerrainChunk;
            if (i == colliderLODIndex)
            {
                // Update Collision Mesh
                lodMeshes[i].updateCallback += UpdateCollisionMesh;
            }
        }

        // Get the max view distance
        maxViewDst = detailLevels[detailLevels.Length - 1].visibleDstThreshold;
    }

    // Called to load the mesh for the chunk
    public void Load()
    {
        // Request a height map
        ThreadedDataRequester.RequestData(() => HeightMapGenerator.GenerateHeightMap(meshSettings.numVertsPerLine, meshSettings.numVertsPerLine, heightMapSettings, sampleCentre), OnHeightMapRecieved);
    }

    // Called when we get the data for the Chunk
    void OnHeightMapRecieved(object heightMapObject)
    {
        // Assign the mapData
        this.heightData = (HeightMap)heightMapObject;
        // Set mapDataRecieved to true
        heightMapRecieved = true;

        // Call the UpdateTerrainChunk
        UpdateTerrainChunk();
    }

    // This returns the viewer position
    Vector2 viewerPosition
    {
        get
        {
            // Return the x & y axis of the player position
            return new Vector2(viewer.position.x, viewer.position.z);
        }
    }

    // This updates the chunk, the lod, etc
    public void UpdateTerrainChunk()
    {
        if (heightMapRecieved)
        {
            // Get the distance from the player to the nearest chunk edge
            float viewerDistanceFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));

            // Was the chunk visible
            bool wasVisible = IsVisible();
            // Is the chunk edge visible
            bool visible = viewerDistanceFromNearestEdge <= maxViewDst;

            if (visible)
            {
                lodIndex = 0;

                for (int i = 0; i < detailLevels.Length - 1; i++)
                {
                    if (viewerDistanceFromNearestEdge > detailLevels[i].visibleDstThreshold)
                    {
                        // Set the LOD Index to i plus 1
                        lodIndex = i + 1;
                    }
                    else { break; }
                }

                if (lodIndex != previousLODIndex)
                {
                    // Get the lod to be used
                    LODMesh lodMesh = lodMeshes[lodIndex];
                    if (lodMesh.hasMesh)
                    {
                        // Set the previous lod index correctly
                        previousLODIndex = lodIndex;
                        // Set the mesh to the mesh filter
                        if (meshFilter != null)
                            meshFilter.mesh = lodMesh.mesh;
                    }
                    else if (!lodMesh.hasRequestedMesh)
                    {
                        // Get the mesh
                        lodMesh.RequestMesh(heightData, meshSettings);
                    }
                }

                if (lodIndex == 0 && gType == TerrainGenerator.GenerateType.Standard)
                {
                    // Generate Props for the chunk
                    PropsGeneration.instance.Generate(this);
                }
                else if (lodIndex > 0)
                {
                    // Remove the props from the chunk
                    PropsGeneration.instance.RemoveFromChunk(this);
                }

                // Set the shared mesh of the collider to the mesh filter's mesh
                if (meshFilter != null)
                    meshCollider.sharedMesh = meshFilter.mesh;
            }
            else
            {
                // Remove everything from the chunk if it isn't visible
                PropsGeneration.instance.RemoveFromChunk(this);
            }

            if (wasVisible != visible)
            {
                // Toggle the visibility
                SetVisible(visible);

                // Call onVisibilityChanged if it isn't null
                onVisibilityChanged?.Invoke(this, visible);
            }
        }
    }

    // Called to update the collision mesh
    public void UpdateCollisionMesh()
    {
        if (!hasSetCollider)
        {
            // Get the squared distance from the player to the edge
            float sqrDstFromViewerToEdge = bounds.SqrDistance(viewerPosition);

            if (sqrDstFromViewerToEdge < detailLevels[colliderLODIndex].sqrVisibleDstThreshold)
            {
                if (!lodMeshes[colliderLODIndex].hasRequestedMesh)
                {
                    // Request mesh
                    lodMeshes[colliderLODIndex].RequestMesh(heightData, meshSettings);
                }
            }

            if (sqrDstFromViewerToEdge < colliderGenerationDistanceThreshold * colliderGenerationDistanceThreshold)
            {
                if (lodMeshes[colliderLODIndex].hasMesh)
                {
                    // Set the mesh collider's mesh
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

    // Used to set the visibility of the chunk
    public void SetVisible(bool visible)
    {
        // Toggle the visibility of the chunk
        meshObject.SetActive(visible);
    }

    // Returns if the chunk is visible
    public bool IsVisible()
    {
        // Return if the chunk is visible
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
        // Input data into the variables
        this.lod = lod;
    }

    // Called once we have a mesh object
    public void OnMeshDataRecieved(object meshDataObject)
    {
        // Create a new mesh
        mesh = ((MeshData)meshDataObject).CreateMesh();
        // Set has mesh to true
        hasMesh = true;

        // Call the updateCallback
        updateCallback();
    }

    // Called to get a mesh
    public void RequestMesh(HeightMap heightMap, MeshSettings meshSettings)
    {
        // Set Has Requested Mesh to true
        hasRequestedMesh = true;
        // Request the mesh data
        ThreadedDataRequester.RequestData(() => MeshGenerator.GenerateTerrainMesh(heightMap.values, meshSettings, lod), OnMeshDataRecieved);
    }
}