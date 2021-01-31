using System.Linq;
using UnityEngine;

public class TerrainChunk
{
    public Vector2 coord;

    public GameObject meshObject;
    public Vector2 sampleCentre;
    public Bounds bounds;

    public MeshCollider meshCollider;
    public ChunkDataStruct chunkData;

    public bool hasChunkData;
    public Mesh mesh = new Mesh();

    public GameObject props;
    public GameObject items;
    public GameObject structures;

    HeightMap heightData;

    HeightMapSettings heightMapSettings;
    MeshSettings meshSettings;

    Player player;

    public TerrainChunk(Vector2 coord, HeightMapSettings heightMapSettings, MeshSettings meshSettings, Transform parent, Player player)
    {
        this.coord = coord;
        this.heightMapSettings = heightMapSettings;
        this.meshSettings = meshSettings;
        this.player = player;

        sampleCentre = coord * meshSettings.meshWorldSize / meshSettings.meshScale;
        Vector3 position = coord * meshSettings.meshWorldSize;
        bounds = new Bounds(position, Vector2.one * meshSettings.meshWorldSize);

        meshObject = new GameObject("Terrain Chunk " + position.x + "-" + position.y);
        meshCollider = meshObject.AddComponent<MeshCollider>();

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
        ThreadedDataRequester.RequestData(() => HeightMapGenerator.GenerateHeightMap(meshSettings.numVertsPerLine, meshSettings.numVertsPerLine, heightMapSettings, sampleCentre), OnHeightMapRecieved);
    }

    void OnHeightMapRecieved(object heightMapObject)
    {
        this.heightData = (HeightMap)heightMapObject;
        ThreadedDataRequester.RequestData(() => MeshGenerator.GenerateTerrainMesh(heightData.values, meshSettings, 0), OnMeshDataRecieved);
    }

    public void OnMeshDataRecieved(object meshDataObject)
    {
        mesh = ((MeshData)meshDataObject).CreateMesh();
        meshCollider.sharedMesh = mesh;

        chunkData = new ChunkDataStruct
        {
            coord = coord,
            heightMap = heightData,
            props = PropsGeneration.instance.Generate(this)
        };

        hasChunkData = true;

        ServerSend.ChunkData(chunkData, player, meshSettings.numVertsPerLine, meshSettings.numVertsPerLine);
    }

}