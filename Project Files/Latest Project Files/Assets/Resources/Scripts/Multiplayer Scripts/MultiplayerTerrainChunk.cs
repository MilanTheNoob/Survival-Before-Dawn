using System.Linq;
using UnityEngine;

public class MultiplayerTerrainChunk
{
    public Vector2 coord;

    public GameObject meshObject;
    public Vector2 sampleCentre;
    public Bounds bounds;

    public MeshRenderer meshRenderer;
    public MeshFilter meshFilter;

    public bool isVisible;
    public ChunkDataStruct chunkData;

    public GameObject props;
    public GameObject items;
    public GameObject structures;

    Mesh mesh;
    MeshSettings meshSettings;
    Transform viewer;

    public MultiplayerTerrainChunk(MeshSettings meshSettings, Transform parent, Transform viewer, Material material, ChunkDataStruct chunkData)
    {
        this.coord = chunkData.coord;
        this.meshSettings = meshSettings;
        this.viewer = viewer;
        this.chunkData = chunkData;

        sampleCentre = coord * meshSettings.meshWorldSize / meshSettings.meshScale;
        Vector3 position = coord * meshSettings.meshWorldSize;
        bounds = new Bounds(position, Vector2.one * meshSettings.meshWorldSize);

        meshObject = new GameObject("Terrain Chunk " + position.x + "-" + position.y);
        meshRenderer = meshObject.AddComponent<MeshRenderer>();
        meshFilter = meshObject.AddComponent<MeshFilter>();

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
    }

    public void Load()
    {
        ThreadedDataRequester.RequestData(() => MeshGenerator.GenerateTerrainMesh(chunkData.heightMap.values, meshSettings, 0), OnMeshDataRecieved);
    }

    public void OnMeshDataRecieved(object meshDataObject)
    {
        mesh = ((MeshData)meshDataObject).CreateMesh();
        meshFilter.mesh = mesh;

        PropsGeneration.instance.MultiplayerGenerate(this);
    }


    public void UpdateTerrainChunk()
    {
        float viewerDistanceFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(new Vector2(viewer.position.x, viewer.position.z)));

        bool wasVisible = IsVisible();
        bool visible = viewerDistanceFromNearestEdge <= 100;


        if (wasVisible != visible)
        {
            if (visible) { PropsGeneration.instance.MultiplayerGenerate(this); } else { PropsGeneration.instance.RemoveFromChunk(this); }

            SetVisible(visible);
        }
    }

    public void SetVisible(bool visible) { meshObject.SetActive(visible); }
    public bool IsVisible() { if (meshObject != null) { return meshObject.activeSelf; } else { return false; } }
}