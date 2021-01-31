using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public List<Vector3> GenerateProps(List<Vector3> vertices, GameObject chunk, BoxCollider triggerCollider)
    {
        // Cycle through all the props
        for (int j = 0; j < propsGeneration.propsSettings.BasicTerrainProps.Length; j++)
        {
            // Get the pool of the prop
            PoolData pool = propsGeneration.pools[propsGeneration.propsSettings.BasicTerrainProps[j].name + " Pool"];

            for (int i = 0; i < propsGeneration.propsSettings.BasicTerrainProps[j].propsPerChunk; i++)
            {
                // Get a random postion
                int t = (int)Random.Range(0, vertices.Count);

                // Place an object, if successful remove the vertice
                if (propsReferences.PlaceObject(chunk, pool, vertices[t], propsGeneration.propsSettings.BasicTerrainProps[j].rotationClamps) != null)
                {
                    vertices.RemoveAt(i);
                }
            }
        }

        // Store chunk
        propsSaving.StoreChunk(chunk);

        // Return true since this func is actually a bool
        return vertices;

    }
}
