using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewTerrainChunk
{
    public Vector2 coord;

    public GameObject meshObject;
    public Vector2 sampleCentre;

    public MeshRenderer meshRenderer;
    public MeshFilter meshFilter;
    public MeshCollider meshCollider;

    HeightMap heightData;
    HeightMapSettings heightMapSettings;
    MeshSettings meshSettings;

    PreviewLODMesh l;

    public PreviewTerrainChunk(Vector2 coord, HeightMapSettings heightMapSettings, MeshSettings meshSettings, Transform parent, Material material)
    {
        // Apply sum data
        this.coord = coord;
        this.heightMapSettings = heightMapSettings;
        this.meshSettings = meshSettings;

        // Get the position of the chunk given & set the bounds
        sampleCentre = coord * meshSettings.meshWorldSize / meshSettings.meshScale;
        Vector3 position = coord * meshSettings.meshWorldSize;

        // Create a new chunk
        meshObject = new GameObject("Terrain Chunk " + position.x + "-" + position.y);
        // Add a new Mesh Renderer to the chunk
        meshRenderer = meshObject.AddComponent<MeshRenderer>();
        // Add a new Mesh Filter to the chunk
        meshFilter = meshObject.AddComponent<MeshFilter>();
        // Add a new Mesh Collider
        meshCollider = meshObject.AddComponent<MeshCollider>();

        // Assign the material
        meshRenderer.material = material;

        // Set the position of the chunk
        meshObject.transform.position = new Vector3(position.x, 0, position.y);
        // Set the parent of the chunk
        meshObject.transform.parent = parent;
    }

    // Called to load the mesh for the chunk
    public void Load()
    {
        ThreadedDataRequester.RequestData(() => HeightMapGenerator.GenerateHeightMap(meshSettings.numVertsPerLine, meshSettings.numVertsPerLine, heightMapSettings, sampleCentre), OnHeightMapRecieved);
    }

    // Called when we get the data for the Chunk
    void OnHeightMapRecieved(object heightMapObject)
    {
        this.heightData = (HeightMap)heightMapObject;

        l = new PreviewLODMesh();
        l.updateCallback += SetMesh;

        l.RequestMesh(heightData, meshSettings);
    }

    void SetMesh() { meshFilter.mesh = l.mesh; meshCollider.sharedMesh = l.mesh; }
}

class PreviewLODMesh
{
    public Mesh mesh;
    public event System.Action updateCallback;

    // Called once we have a mesh object
    public void OnMeshDataRecieved(object meshDataObject)
    {
        // Create a new mesh
        mesh = ((MeshData)meshDataObject).CreateMesh();

        // Call the updateCallback
        updateCallback();
    }

    // Called to get a mesh
    public void RequestMesh(HeightMap heightMap, MeshSettings meshSettings)
    {
        // Request the mesh data
        ThreadedDataRequester.RequestData(() => MeshGenerator.GenerateTerrainMesh(heightMap.values, meshSettings, 0), OnMeshDataRecieved);
    }
}