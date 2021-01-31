using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolData : MonoBehaviour
{
    public List<GameObject> pool = new List<GameObject>();

    // Get all the objects inside the pool parent and store it
    public void GetObjects()
    {
        // Clear the pool
        pool.Clear();

        foreach(Transform child in gameObject.transform)
        {
            // Add each object to the list
            pool.Add(child.gameObject);
        }
    }

    // Called to clear everything from the pool
    public void ClearObjects()
    {
        // Clear everything
        pool.Clear();
    }
}