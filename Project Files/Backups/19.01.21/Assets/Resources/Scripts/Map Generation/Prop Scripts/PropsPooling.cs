using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropsPooling : MonoBehaviour
{
    #region Singleton

    public static PropsPooling instance;

    private void Awake()
    {
        instance = this;
    }

    #endregion

    [HideInInspector]
    public PropsGeneration propsGeneration;
    [HideInInspector]
    public PropsDestroying propsDestroying;

    public enum PropType
    {
        Standard,
        Structure
    }

    // The pooling function
    public void StartPooling()
    {
        // Firstly remove everything
        propsDestroying.RemoveEverything();

        // Create new pool holders
        propsGeneration.propsPoolParent = new GameObject("Props Pool");
        propsGeneration.structuresPoolParent = new GameObject("Structures Pool");

        // Loop through all the different props
        for (int i = 0; i < propsGeneration.propsSettings.PropGroups.Length; i++)
        {
            for (int y = 0; y < propsGeneration.propsSettings.PropGroups[i].Props.Length; y++)
            {
                // Create a new pool holder for the prop pool and set its parent
                GameObject poolHolder = new GameObject(propsGeneration.propsSettings.PropGroups[i].Props[y].prefab.transform.name + " Pool");
                poolHolder.transform.parent = propsGeneration.propsPoolParent.transform;

                // Add a pool data script to it and store it
                PoolData poolData = poolHolder.AddComponent<PoolData>();

                // Add the pool to the pool holder
                propsGeneration.pools.Add(poolHolder.transform.name, poolData);

                for (int j = 0; j < propsGeneration.propsSettings.poolSizes; j++)
                {
                    // Instantiate a new prop
                    GameObject newObject = Instantiate(propsGeneration.propsSettings.PropGroups[i].Props[y].prefab);

                    // Set the name, position, etc of the prop correctly
                    newObject.transform.name = propsGeneration.propsSettings.PropGroups[i].Props[y].prefab.transform.name;
                    newObject.transform.parent = poolHolder.transform;
                    newObject.SetActive(false);

                    // Add the prop to a couple of the array
                    poolData.pool.Add(newObject);
                    propsGeneration.props.Add(newObject);
                }
            }
        }

        // Loop through all the different structures
        for (int x = 0; x < propsGeneration.structuresSettings.StandardBuildings.Length; x++)
        {
            // Create a new pool holder for the structures pool
            GameObject poolHolder = new GameObject(propsGeneration.structuresSettings.StandardBuildings[x].name + " Pool");
            poolHolder.transform.parent = propsGeneration.structuresPoolParent.transform;

            // Add a pool data script to it and store it
            PoolData poolData = poolHolder.AddComponent<PoolData>();

            propsGeneration.pools.Add(poolHolder.transform.name, poolData);

            for (int y = 0; y < propsGeneration.structuresSettings.pool; y++)
            {
                // Instantiate a new prop
                GameObject newObject = Instantiate(propsGeneration.structuresSettings.StandardBuildings[x].structure);

                // Set the position, etc of the prop correctly
                newObject.transform.parent = poolHolder.transform;
                newObject.SetActive(false);

                // Add the prop to a couple of the array
                poolData.pool.Add(newObject);
                propsGeneration.structures.Add(newObject);
            }
        }
    }

    // This also duplicates an object into the propsPool
    public void DuplicatePropPool(string i)
    {
        for (int j = 0; j < 100; j++)
        {
            // Instantaite a gameObject and set its properties
            GameObject prop = Instantiate(propsGeneration.pools[i].pool[0], Vector3.zero, Quaternion.identity);

            if (prop.GetComponent<Rigidbody>() != null) { Destroy(prop.GetComponent<Rigidbody>()); }

            prop.transform.parent = propsGeneration.pools[i].gameObject.transform;
            prop.transform.name = propsGeneration.pools[i].pool[0].transform.name;
            prop.SetActive(false);

            propsGeneration.pools[i].pool.Add(prop);
        }
    }
}
