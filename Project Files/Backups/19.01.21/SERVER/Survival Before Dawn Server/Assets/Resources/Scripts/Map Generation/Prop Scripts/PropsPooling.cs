using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropsPooling : MonoBehaviour
{
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
        for (int i = 0; i < propsGeneration.propsSettings.BasicTerrainProps.Length; i++)
        {
            // Create a new pool holder for the prop pool and set its parent
            GameObject poolHolder = new GameObject(propsGeneration.propsSettings.BasicTerrainProps[i].name + " Pool");
            poolHolder.transform.parent = propsGeneration.propsPoolParent.transform;

            // Add a pool data script to it and store it
            PoolData poolData = poolHolder.AddComponent<PoolData>();

            // Add the pool to the pool holder
            propsGeneration.pools.Add(poolHolder.transform.name, poolData);

            for (int j = 0; j < propsGeneration.propsSettings.BasicTerrainProps[i].poolSize; j++)
            {
                // Instantiate a new prop
                GameObject newObject = Instantiate(propsGeneration.propsSettings.BasicTerrainProps[i].propPrefab);

                // Check if the prop has a mesh collider
                if (newObject.GetComponent<MeshCollider>() == null)
                    newObject.AddComponent<MeshCollider>();

                // Set the name, position, etc of the prop correctly
                newObject.transform.name = propsGeneration.propsSettings.BasicTerrainProps[i].name;
                newObject.transform.parent = poolHolder.transform;
                newObject.SetActive(false);

                // Add the prop to a couple of the array
                poolData.pool.Add(newObject);
                propsGeneration.props.Add(newObject);
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

            for (int y = 0; y < propsGeneration.structuresSettings.poolSizes; y++)
            {
                // Instantiate a new prop
                GameObject newObject = propsGeneration.structuresSettings.StandardBuildings[x].getStructure();

                // Set the position, etc of the prop correctly
                newObject.transform.parent = poolHolder.transform;
                newObject.SetActive(false);

                // Add the prop to a couple of the array
                poolData.pool.Add(newObject);
                propsGeneration.structures.Add(newObject);
            }
        }
    }
}
