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
        Transform chunksParent = TerrainGenerator.instance.gameObject.transform;

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
        if (chunk.transform.childCount == 0 ||  TerrainGenerator.instance.generateType == TerrainGenerator.GenerateType.PreGen) { return false; }

        // Create a new chunkData
        SavingManager.ChunkPropData chunkData = new SavingManager.ChunkPropData();
        // The chunk coord
        Vector2 coord = new Vector2(chunk.transform.position.x, chunk.transform.position.z);

        // Get the props & structures holder
        GameObject propsHolder = null;
        GameObject structuresHolder = null;

        try { structuresHolder = chunk.transform.Find("Structures Holder").gameObject; } catch { }
        try { propsHolder = chunk.transform.Find("Props Holder").gameObject; } catch { }

        if (propsHolder == null)
            return false;

        // Loop through all the children in the props holder
        for (int i = 0; i < propsHolder.transform.childCount; i++)
        {
            //The prop
            Transform prop = propsHolder.transform.GetChild(i);

            // Add the names, positions and rotations
            chunkData.propName.Add(prop.name);
            chunkData.position.Add(prop.position);
            chunkData.rotation.Add(prop.rotation);
        }
        if (structuresHolder != null)
        {
            // Loop through all the children in the structures holder
            for (int i = 0; i < structuresHolder.transform.childCount; i++)
            {
                //The prop
                Transform prop = structuresHolder.transform.GetChild(i);

                // Add the names, positions and rotations
                chunkData.propName.Add(prop.name);
                chunkData.position.Add(prop.position);
                chunkData.rotation.Add(prop.rotation);
            }
        }

        SavingManager.SaveFile.chunkData[coord] = chunkData;

        // Add the chunk to the chunks list one way or the other
        if (!SavingManager.SaveFile.chunkData.ContainsKey(coord))
        {
            SavingManager.SaveFile.chunkData.Add(coord, chunkData);
        }
        else
        {
            SavingManager.SaveFile.chunkData[coord] = chunkData;
        }

        print("saved chunk: " + chunk.transform.position.x + "," + chunk.transform.position.z);

        // Return true
        return true;
    }

    // Here to apply prop data to a chunk
    public void ApplyChunkData(GameObject chunk)
    {
        SavingManager.ChunkPropData saveData = SavingManager.SaveFile.chunkData[new Vector2(chunk.transform.position.x, chunk.transform.position.z)];

        GameObject propsHolder = null;
        GameObject itemsHolder = null;
        GameObject spHolder = null;

        for (int i = 0; i < chunk.transform.childCount; i++)
        {
            Transform child = chunk.transform.GetChild(i);
            if (child.name == "Props Holder")
            {
                propsHolder = child.gameObject;
            }
            else if (child.name == "Items Holder")
            {
                itemsHolder = child.gameObject;
            }
            else if (child.name == "Structure Pieces Holder")
            {
                spHolder = child.gameObject;
            }
        }

        if (propsHolder == null)
        {
            propsHolder = new GameObject("Props Holder");
            propsHolder.transform.parent = chunk.transform;
            propsHolder.transform.localPosition = Vector3.zero;
        }
        if (itemsHolder == null)
        {
            itemsHolder = new GameObject("Items Holder");
            itemsHolder.transform.parent = chunk.transform;
            itemsHolder.transform.localPosition = Vector3.zero;
        }
        if (spHolder == null)
        {
            spHolder = new GameObject("Structure Pieces Holder");
            spHolder.transform.parent = chunk.transform;
            spHolder.transform.localPosition = Vector3.zero;
        }

        for (int i = 0; i < saveData.propName.Count; i++)
        {
            // Get the pool to be used
            PoolData pool = null;
            try { pool = propsGeneration.pools[saveData.propName[i] + " Pool"]; } catch { }

            if (pool != null)
            {
                // Store the prop we are going to use
                GameObject prop = pool.pool[0];
                // Remove the prop from the pool
                pool.pool.RemoveAt(0);

                // Set the parent and visibility of the prop
                prop.transform.parent = propsHolder.transform;
                prop.SetActive(true);

                // Set the position and rotation of the prop
                prop.transform.position = saveData.position[i];
                prop.transform.rotation = saveData.rotation[i];
            }
        }
    }
}
