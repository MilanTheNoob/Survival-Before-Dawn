using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropsDestroying : MonoBehaviour
{
    [HideInInspector]
    public PropsGeneration propsGeneration;
    [HideInInspector]
    public PropsSaving propsSaving;

    // Called to remove all the props and pools
    public void RemoveEverything()
    {
        // Loop through all the props & structures
        for (int i = 0; i < propsGeneration.props.Count; i++)
        {
            // Destroy all the props
            Destroy(propsGeneration.props[i]);
        }
        for (int i = 0; i < propsGeneration.structures.Count; i++)
        {
            // Destroy all the structures
            Destroy(propsGeneration.structures[i]);
        }

        // And clear the props & structures list
        propsGeneration.props.Clear();
        propsGeneration.structures.Clear();

        // Clear the pool dictionary
        propsGeneration.pools.Clear();

        // Destroy the pool holders
        Destroy(propsGeneration.propsPoolParent);
        Destroy(propsGeneration.structuresPoolParent);
    }

    // Called to remove all the props and structures from a chunk
    public void RemoveEverythingFromChunk(GameObject chunk)
    {
        // Check if the chunk has any props
        if (chunk.transform.childCount <= 0)
            return;

        // Store the chunk
        propsSaving.StoreChunk(chunk);

        // Loop through all the chunk's children
        foreach (Transform child in chunk.transform)
        {
            // Remove the prop
            RemoveProp(child);
        }
    }

    // Called to remove a prop
    public void RemoveProp(Transform child)
    {
        // Get the pool the object belongs to
        PoolData pool = propsGeneration.pools[child.name + " Pool"];

        // Set all the stuff about the prop correctly
        child.position = Vector3.zero;
        child.gameObject.SetActive(false);
        child.parent = pool.transform;

        // Add the prop to to the pool Array
        pool.pool.Add(child.gameObject);
    }
}
