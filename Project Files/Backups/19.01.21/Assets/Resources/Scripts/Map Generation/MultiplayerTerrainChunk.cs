using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerTerrainChunk
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

    Mesh mesh;
    bool hasMesh;

    HeightMap heightData;
    float maxViewDst;

    HeightMapSettings heightMapSettings;
    MeshSettings meshSettings;

    Transform viewer;

    public MultiplayerTerrainChunk(Vector2 coord, HeightMapSettings heightMapSettings, MeshSettings meshSettings, Transform parent, Transform viewer, Material material)
    {
        // Apply sum data
        this.coord = coord;
        this.heightMapSettings = heightMapSettings;
        this.meshSettings = meshSettings;
        this.viewer = viewer;

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
    }

    // Called to load the mesh for the chunk
    public void Load()
    {
        Debug.Log("FUCK");
        // Request a height map
        ThreadedDataRequester.RequestData(() => HeightMapGenerator.GenerateHeightMap(meshSettings.numVertsPerLine, meshSettings.numVertsPerLine, heightMapSettings, sampleCentre), OnHeightMapRecieved);
    }

    // Called when we get the data for the Chunk
    void OnHeightMapRecieved(object heightMapObject)
    {
        // Assign the mapData
        this.heightData = (HeightMap)heightMapObject;

        // Request the mesh data
        ThreadedDataRequester.RequestData(() => MeshGenerator.GenerateTerrainMesh(heightData.values, meshSettings, 1), UpdateTerrainChunk);
    }

    // This returns the viewer position
    Vector2 viewerPosition
    {
        get
        {
            try
            {
                // Return the x & y axis of the player position
                return new Vector2(viewer.position.x, viewer.position.z);
            }
            catch (Exception)
            {
                return new Vector2(0, 0);
            }
        }
    }

    // This updates the chunk, the lod, etc
    public void UpdateTerrainChunk(object meshDataObject)
    {
        // Get the distance from the player to the nearest chunk edge
        float viewerDistanceFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));

        // Was the chunk visible
        bool wasVisible = IsVisible();
        // Is the chunk edge visible
        bool visible = viewerDistanceFromNearestEdge <= maxViewDst;

        Debug.Log(visible);

        if (!hasMesh)
        {
            mesh = ((MeshData)meshDataObject).CreateMesh();
            meshFilter.mesh = mesh;
            meshCollider.sharedMesh = mesh;
            hasMesh = true;
        }

        // Toggle the visibility
        SetVisible(visible);
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