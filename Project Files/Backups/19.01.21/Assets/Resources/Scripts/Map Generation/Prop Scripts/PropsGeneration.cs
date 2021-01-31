using System.Collections.Generic;
using UnityEngine;

public class PropsGeneration : MonoBehaviour
{
    // All the singleton code
    #region Singleton

    // The Singleton instance
    public static PropsGeneration instance;

    #endregion

    [Header("The scriptable objects we get data from")]
    public PropsSettings propsSettings;
    public StructuresSettings structuresSettings;

    [Header("Freeze the different systems for debugging?")]
    public bool freezeSaving = false;
    public bool freezeGenerating = false;

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

    [HideInInspector]
    public List<Vector3> currentVertices;
    [HideInInspector]
    public List<Vector3> locatedStructures;

    PropsGenerator propsGenerator;
    PropsReferences propsReferences;
    PropsPooling propsPooling;
    PropsSaving propsSaving;
    PropsDestroying propsDestroying;

    StructuresGenerator structuresGenerator;

    List<string> generatedChunks = new List<string>();

    // Called before start
    void Awake()
    {
        // Set the instances of all the scripts correctly
        instance = this;

        // Add all the new prop & structure scripts to our gameObject
        propsGenerator = gameObject.AddComponent<PropsGenerator>();
        propsReferences = gameObject.AddComponent<PropsReferences>();
        propsPooling = gameObject.AddComponent<PropsPooling>();
        propsSaving = gameObject.AddComponent<PropsSaving>();
        propsDestroying = gameObject.AddComponent<PropsDestroying>();
        structuresGenerator = gameObject.AddComponent<StructuresGenerator>();

        // The PropsDestroying instances
        propsDestroying.propsGeneration = this;
        propsDestroying.propsSaving = propsSaving;

        // The PropsGenerator instances
        propsGenerator.propsGeneration = this;
        propsGenerator.propsReferences = propsReferences;
        propsGenerator.propsSaving = propsSaving;
        propsGenerator.propsPooling = propsPooling;

        // The PropsPooling instances
        propsPooling.propsGeneration = this;
        propsPooling.propsDestroying = propsDestroying;

        // The PropsSaving instances
        propsSaving.propsGeneration = this;
        propsSaving.propsPooling = propsPooling;

        // The PropsReferences instances
        propsReferences.propsPooling = propsPooling;
        propsReferences.propsGeneration = this;

        // The StructuresGenerator instances
        structuresGenerator.propsGeneration = this;
        structuresGenerator.propsReferences = propsReferences;
        structuresGenerator.propsSaving = propsSaving;
        structuresGenerator.propsPooling = propsPooling;
    }

    // Called at the beginning of the game
    void Start()
    {
        // Add some funcs to the SaveGameCallback
        SavingManager.SaveGameCallback += StoreVisibleChunks;

        // Start Pooling all the props needed
        propsPooling.StartPooling();
    }

    // Called to generate props & structures on a chunk
    public void Generate(TerrainChunk chunk)
    {
        // Check if the meshObject is null
        if (chunk.meshObject == null)
            return;

        // Check if props has already been generated
        if (generatedChunks.Contains(chunk.meshObject.transform.name) || freezeGenerating || chunk.meshFilter.mesh.vertices.Length < 14000)
            return;

        if (SavingManager.SaveFile.chunkData.ContainsKey(new Vector2(chunk.meshObject.transform.position.x, chunk.meshObject.transform.position.z)))
        {
            propsSaving.ApplyChunkData(chunk.meshObject);
            return;
        }

        // Call the StructuresGenerator and store the vertices it returns
        currentVertices = structuresGenerator.GenerateStructures(chunk.meshObject, chunk.meshFilter);

        // Call the PropsGenerator
        propsGenerator.GenerateProps(currentVertices, chunk.meshObject, chunk.biome);

        // Add to the generated chunks
        generatedChunks.Add(chunk.meshObject.transform.name);

        StoreChunk(chunk.meshObject);
    }

    // Remove functions
    public void RemoveFromChunk(TerrainChunk chunk)
    {
        if (chunk.meshObject == null)
            return;

        // Remove the chunk from the generated chunks
        generatedChunks.Remove(chunk.meshObject.transform.name);
        // Remove everything from the chunk
        propsDestroying.RemoveEverythingFromChunk(chunk.meshObject);
    }
    public void RemoveAll()
    {
        // Clear the generated chunks list
        generatedChunks.Clear();
        // Remove everything
        propsDestroying.RemoveEverything();
    }

    // Store functions
    public void StoreVisibleChunks() { propsSaving.StoreUsedChunks(); }
    public void StoreChunk(GameObject chunk) { propsSaving.StoreChunk(chunk); }

    // Get a random biome
    public int GetRandomBiome() { return Random.Range(0, propsSettings.Biomes.Length); }
}