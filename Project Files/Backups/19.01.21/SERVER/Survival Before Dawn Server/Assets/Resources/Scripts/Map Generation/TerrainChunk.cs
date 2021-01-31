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
    public GameObject triggerColParent;
    public Vector2 sampleCentre;
    public Bounds bounds;

    public MeshRenderer meshRenderer;
    public MeshFilter meshFilter;
    public MeshCollider meshCollider;
    public BoxCollider triggerCollider;

    public Dictionary<string, Vector3> props = new Dictionary<string, Vector3>();
    public TerrainGenerator.TerrainDataStruct terrainData;

    Mesh mesh = new Mesh();
    HeightMap heightData;

    HeightMapSettings heightMapSettings;
    MeshSettings meshSettings;

    Player calledPlayer;

    public TerrainChunk(Vector2 coord, HeightMapSettings heightMapSettings, MeshSettings meshSettings, Transform parent, Player calledPlayer)
    {
        // Apply sum data
        this.coord = coord;
        this.heightMapSettings = heightMapSettings;
        this.meshSettings = meshSettings;
        this.calledPlayer = calledPlayer;

        // Get the position of the chunk given & set the bounds
        sampleCentre = coord * meshSettings.meshWorldSize / meshSettings.meshScale;
        Vector3 position = coord * meshSettings.meshWorldSize;
        bounds = new Bounds(position, Vector2.one * meshSettings.meshWorldSize);

        // Create a new chunk
        meshObject = new GameObject("Terrain Chunk " + position.x + "-" + position.y);
        // Add a new Mssh Collider to the chunk
        meshCollider = meshObject.AddComponent<MeshCollider>();

        // Set the position of the chunk
        meshObject.transform.position = new Vector3(position.x, 0, position.y);
        // Set the parent of the chunk
        meshObject.transform.parent = parent;

        // Set the layer and tag of the chunk
        meshObject.layer = 8;
        meshObject.tag = "TerrainChunk";
    }

    // Called to load the mesh for the chunk
    public TerrainGenerator.TerrainDataStruct Load()
    {
        // Request a height map
        ThreadedDataRequester.RequestData(() => HeightMapGenerator.GenerateHeightMap(meshSettings.numVertsPerLine, meshSettings.numVertsPerLine, heightMapSettings, sampleCentre), OnHeightMapRecieved);

        TerrainGenerator.TerrainDataStruct data = new TerrainGenerator.TerrainDataStruct();
        data.coord = coord;
        data.props = props;
        return data;
    }

    public void UnLoad()
    {

    }

    // Called when we get the data for the Chunk
    void OnHeightMapRecieved(object heightMapObject)
    {
        // Assign the mapData
        this.heightData = (HeightMap)heightMapObject;

        ThreadedDataRequester.RequestData(() => MeshGenerator.GenerateTerrainMesh(heightData.values, meshSettings, 1), OnMeshDataRecieved);
    }

    // Called once we have a mesh object
    public void OnMeshDataRecieved(object meshDataObject)
    {
        // Create a new mesh
        mesh = ((MeshData)meshDataObject).CreateMesh();
        // Set the mesh to the mesh collider
        meshCollider.sharedMesh = mesh;

        // Generate Props for the chunk
        PropsGeneration.instance.Generate(this);

        ServerSend.ChunkData(GetTerrainData(), calledPlayer);
    }

    public void RemoveChunk()
    {
        PropsGeneration.instance.RemoveFromChunk(this);

        Object.Destroy(triggerColParent);
        Object.Destroy(meshObject);

        //TerrainGenerator.instance.terrainChunkDictionary.Remove(coord);
        //TerrainGenerator.instance.visibleTerrainChunks.Remove(this);
    }

    public TerrainGenerator.TerrainDataStruct GetTerrainData()
    {
        TerrainGenerator.TerrainDataStruct terrainData = new TerrainGenerator.TerrainDataStruct();

        terrainData.coord = coord;

        return terrainData;
    }

}