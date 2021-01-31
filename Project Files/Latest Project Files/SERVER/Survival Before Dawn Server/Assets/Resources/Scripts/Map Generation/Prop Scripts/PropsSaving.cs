using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropsSaving : MonoBehaviour
{
    [HideInInspector]
    public PropsGeneration propsGeneration;
    [HideInInspector]
    public PropsPooling propsPooling;

    // Called to store all the used chunks
    public void StoreUsedChunks()
    {
        // Has the freezing been saved
        if (propsGeneration.freezeSaving)
            return;

        // Get the chunk parent
        Transform chunksParent = TerrainGenerator.instance.transform;

        // Loop through the chunks
        for (int i = 0; i < chunksParent.childCount; i++)
        { 
            // Get child
            Transform child = chunksParent.GetChild(i);

            // Get the child count of the chunks
            if (child.childCount > 0)
                StoreChunk(child.gameObject);
        }
    }

    // Called to save a specific chunk
    public bool StoreChunk(GameObject chunk)
    {
        // Check if we should store the provided chunk
        if (chunk.transform.childCount == 0 || propsGeneration.freezeSaving) { return false; }
        
        // Create a new chunkData
        ChunkPropData chunkData = new ChunkPropData();
        bool alreadyExists = false;
        int existingLoc = 0;

        // Set the chunk location
        chunkData.chunkLocation = chunk.transform.position.x + "x" + chunk.transform.position.z;

        // Loop through all the children in the chunk
        foreach (Transform prop in chunk.transform)
        {
            // Add the names, positions and rotations
            chunkData.propName.Add(prop.name);
            chunkData.position.Add(prop.position);
            chunkData.rotation.Add(prop.rotation);
        }

        // Loop through all the chunk strings
        for (int i = 0; i < propsGeneration.chunkPropData.Count; i++)
        {
            if (propsGeneration.chunkPropData[i].chunkLocation == chunkData.chunkLocation) 
            { 
                // If a duplicate exists replace it
                propsGeneration.chunkPropData[i] = chunkData; 
                alreadyExists = true;
                existingLoc = i;
            }
        }

        // Add the chunk to the chunks list one way or the other
        if (propsGeneration.chunkPropData.Count < 1 || !alreadyExists)
        {
            propsGeneration.chunkPropData.Add(chunkData);
            propsGeneration.worldPropData.chunkLocations.Add(chunkData.chunkLocation);
        }
        else
        {
            propsGeneration.chunkPropData[existingLoc] = chunkData;
            propsGeneration.worldPropData.chunkLocations[existingLoc] = chunkData.chunkLocation;
        }

        // Return true
        return true;
    }

    // Used to load all of the prop data
    public void LoadWorldPropData()
    {
        // Check if the saving has been freezed
        if (propsGeneration.freezeSaving)
            return;
        
        // Load & get the PropData struct
        propsGeneration.worldPropData = BinarySerializer.Load<WorldPropData>("PropData");

        if (propsGeneration.worldPropData.chunkLocations.Count > 0)
        {
            for (int i = 0; i < propsGeneration.worldPropData.chunkLocations.Count; i++)
            {
                // Add all the chunk datas
                propsGeneration.chunkPropData.Add(BinarySerializer.Load<ChunkPropData>(propsGeneration.worldPropData.chunkLocations[i]));
            }
        }
    }

    // Called to save all the prop data
    public void SaveWorldPropData()
    {
        // Check if the saving has been freezed
        if (propsGeneration.freezeSaving)
            return;
        
        for (int i = 0; i < propsGeneration.chunkPropData.Count; i++)
        {
            // Save all the chunk datas
            BinarySerializer.Save(propsGeneration.chunkPropData[i], propsGeneration.chunkPropData[i].chunkLocation);
        }

        // Save the PropData
        BinarySerializer.Save(propsGeneration.worldPropData, "PropData");
    }

    // Used to get prop data for a chunk
    public ChunkPropData GetChunkData(GameObject chunk)
    {
        // Check if the saving has been freezed
        if (propsGeneration.freezeSaving || chunk.transform.childCount <= 0)
            return null;
        
        // Set some variables
        ChunkPropData chunkData = new ChunkPropData();
        string chunkLocationStr = chunk.transform.position.x + "x" + chunk.transform.position.z;

        // Loop through all the chunks and find the one we need
        for (int i = 0; i < propsGeneration.chunkPropData.Count; i++)
        {
            if (propsGeneration.chunkPropData[i].chunkLocation == chunkLocationStr) 
            { 
                // Store the chunk data
                chunkData = propsGeneration.chunkPropData[i];
            }
        }

        // Return the chunk data
        return chunkData;
    }

    // Here to apply prop data to a chunk
    public bool ApplyChunkData(GameObject chunk)
    {
        // Check if the saving has been freezed
        if (propsGeneration.freezeSaving)
            return false;
        
        // Get the data
        ChunkPropData saveData = GetChunkData(chunk);

        if (saveData == null)
            return false;

        // Make sure the save data and chunk adheres to the parameters otherwise return false
        if (saveData.propName.Count > 0)
        {
            for (int i = 0; i < saveData.propName.Count; i++)
            {
                // Get the pool to be used
                PoolData pool = propsGeneration.pools[saveData.propName[i] + " Pool"];

                // Store the prop we are going to use
                GameObject prop = pool.pool[0];
                // Remove the prop from the pool
                pool.pool.RemoveAt(0);

                // Set the parent and visibility of the prop
                prop.transform.parent = chunk.transform;
                prop.SetActive(true);

                // Set the position and rotation of the prop
                prop.transform.position = saveData.position[i];
                prop.transform.rotation = saveData.rotation[i];
            }

            // Return true
            return true;
        }
        
        // Return false
        return false;
    }
}
