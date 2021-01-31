using System.Collections.Generic;
using UnityEngine;

public class PropsGenerator : MonoBehaviour
{
    [HideInInspector]
    public PropsGeneration propsGeneration;
    [HideInInspector]
    public PropsReferences propsReferences;
    [HideInInspector]
    public PropsSaving propsSaving;
    [HideInInspector]
    public PropsPooling propsPooling;
    
    // Called to generate props & structures
    public List<Vector3> GenerateProps(List<Vector3> vertices, GameObject chunk, int biome)
    {
        // Return if null
        if (chunk == null) { return null; }

        // Create a new structures holder
        Transform propsHolder = chunk.transform.Find("Props Holder");

        // If we couldn't find one then create one
        if (propsHolder == null)
            propsHolder = new GameObject("Props Holder").transform;

        // Set the structure holder vars
        propsHolder.parent = chunk.transform;
        propsHolder.localPosition = Vector3.zero;

        /*
        foreach (KeyValuePair<string, PoolData> pool in propsGeneration.pools)
        {
            if (pool.Value.pool.Count < 50)
                propsPooling.DuplicatePropPool(pool.Key);
        }

        */

        // Cycle through all the prop groups
        for (int j = 0; j < propsGeneration.propsSettings.Biomes[biome].props.Length; j++)
        {
            // Loop through all the props
            for (int k = 0; k < propsGeneration.propsSettings.Biomes[biome].props[j].Props.Length; k++)
            {
                // Get the prop id
                int propId = propsGeneration.propsSettings.Biomes[biome].props[j].Props[k].propId;
                int groupId = propsGeneration.propsSettings.Biomes[biome].props[j].Props[k].groupId;

                // Get the pool of the prop
                PoolData pool = propsGeneration.pools[propsGeneration.propsSettings.PropGroups[groupId].Props[propId].prefab.transform.name + " Pool"];

                for (int i = 0; i < propsGeneration.propsSettings.Biomes[biome].props[j].Props[k].propsPerChunk; i++)
                {
                    // Get a random postion
                    int t = Random.Range(0, vertices.Count);

                    // Place an object, if successful remove the vertice
                    propsReferences.PlaceObject(propsHolder.gameObject, pool, new Vector3(vertices[t].x, vertices[t].y + propsGeneration.propsSettings.PropGroups[groupId].Props[propId].yOffset, vertices[t].z), propsGeneration.propsSettings.PropGroups[groupId].Props[propId].rotationClamps);
                    vertices.RemoveAt(i);
                }
            }
        }

        // Create some empty holders for other vars
        if (chunk.transform.Find("Items Holder") == null)
        {
            GameObject itemsHolder = new GameObject("Items Holder");

            // Set the structure holder vars
            itemsHolder.transform.parent = chunk.transform;
            itemsHolder.transform.localPosition = Vector3.zero;
        }

        if (chunk.transform.Find("Structure Pieces Holder") == null)
        {
            GameObject piecesHolder = new GameObject("Structure Pieces Holder");

            // Set the structure holder vars
            piecesHolder.transform.parent = chunk.transform;
            piecesHolder.transform.localPosition = Vector3.zero;

            //BuildingManager.instance.LoadData();
        }

        // Store chunk
        propsSaving.StoreChunk(chunk);

        // Return true since this func is actually a bool
        return vertices;

    }
}
