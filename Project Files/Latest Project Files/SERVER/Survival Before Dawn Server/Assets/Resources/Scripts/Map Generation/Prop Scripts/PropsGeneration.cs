using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class ChunkPropData
{
    public string chunkLocation;
    public List<string> propName = new List<string>();
    public List<Vector3> position = new List<Vector3>();
    public List<Quaternion> rotation = new List<Quaternion>();
}

[System.Serializable]
public class WorldPropData
{
    public List<string> chunkLocations = new List<string>();

    public List<Vector3> locatedStructures = new List<Vector3>();
}

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

    [Header("Percent of vertices per chunk that should be empty")]
    public float emptyVertsPercent = 30f;

    [HideInInspector]
    public Dictionary<string, PoolData> pools = new Dictionary<string, PoolData>();

    [HideInInspector]
    public List<GameObject> structures = new List<GameObject>();
    [HideInInspector]
    public List<GameObject> props = new List<GameObject>();

    [HideInInspector]
    public Dictionary<string, BoxCollider> triggerColliders = new Dictionary<string, BoxCollider>();

    [HideInInspector]
    public WorldPropData worldPropData = new WorldPropData();
    //[HideInInspector]
    public List<ChunkPropData> chunkPropData = new List<ChunkPropData>();

    [HideInInspector]
    public GameObject propsPoolParent;
    [HideInInspector]
    public GameObject structuresPoolParent;

    [HideInInspector]
    public List<Vector3> currentVertices;
    [HideInInspector]
    public List<Vector3> locatedStructures;

    PropsGenerator propsGenerator = new PropsGenerator();
    PropsReferences propsReferences = new PropsReferences();
    PropsPooling propsPooling = new PropsPooling();
    PropsSaving propsSaving = new PropsSaving();
    PropsDestroying propsDestroying = new PropsDestroying();

    StructuresGenerator structuresGenerator = new StructuresGenerator();

    List<string> generatedChunks = new List<string>();

    // Called before start
    void Awake()
    {
        // Set the instances of all the scripts correctly
        instance = this;
        
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
        //InputManager.instance.SaveGameCallback += StoreVisibleChunks;
        //InputManager.instance.SaveGameCallback += SaveData;

        // Load all the data
        //propsSaving.LoadWorldPropData();
        // Start Pooling all the props needed
        //propsPooling.StartPooling();
    }

    // Called to generate props & structures on a chunk
    public void Generate(TerrainChunk chunk)
    {
        /*
        // Check if props has already been generated
        if (generatedChunks.Contains(chunk.meshObject.transform.name) || freezeGenerating)
            return;

        // Check if props has already been saved
        if (propsSaving.ApplyChunkData(chunk.meshObject))
            return;

        // Call the StructuresGenerator and store the vertices it returns
        currentVertices = structuresGenerator.GenerateStructures(chunk.meshObject, chunk.meshFilter, chunk.triggerCollider);

        // Call the PropsGenerator
        propsGenerator.GenerateProps(currentVertices, chunk.meshObject, chunk.triggerCollider);

        // Add to the generated chunks
        generatedChunks.Add(chunk.meshObject.transform.name);
        */
    }

    // Remove functions
    public void RemoveFromChunk(TerrainChunk chunk) 
    {
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

    // Load functions
    public void LoadData() { propsSaving.LoadWorldPropData(); }
    public void SaveData() { propsSaving.SaveWorldPropData(); }

}