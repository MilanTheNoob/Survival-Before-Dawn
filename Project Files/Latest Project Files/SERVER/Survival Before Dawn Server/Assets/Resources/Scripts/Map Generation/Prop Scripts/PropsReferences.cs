using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropsReferences : MonoBehaviour
{
    [HideInInspector]
    public PropsPooling propsPooling;
    [HideInInspector]
    public PropsGeneration propsGeneration;

    readonly int[] possibleStructRots = new int[]{0, 90, 180, 270, 360};

    // Called to place an object
    public GameObject PlaceObject(GameObject chunk, PoolData objectPool, Vector3 objectPos, Vector3 objectRotClamp)
    {
        // Return null if the pool has been emptied
        if (objectPool.pool.Count < 1)
            return null;

        // Get a random rotation
        Vector3 euler = Vector3.zero;
        euler.x = Random.Range(-objectRotClamp.x, objectRotClamp.x);
        euler.y = Random.Range(-objectRotClamp.y, objectRotClamp.y);
        euler.z = Random.Range(-objectRotClamp.z, objectRotClamp.z);

        // Get an object from the pool and remove it from the pool
        GameObject g = objectPool.pool[0];
        objectPool.pool.RemoveAt(0);


        // Set the basic properties
        g.transform.parent = chunk.transform;
        g.transform.localPosition = objectPos;
        g.transform.eulerAngles = euler;
        g.SetActive(true);

        // Return the object
        return g;
    }

    // Called to place an object in global space
    public GameObject PlaceStructure(GameObject chunk, PoolData objectPool, Vector3 objectPos, int objId)
    {
        // Return null if the pool has been emptied
        if (objectPool.pool.Count < 1)
            return null;

        // Get an object from the pool and remove it from the pool
        GameObject g = objectPool.pool[0];
        objectPool.pool.RemoveAt(0);

        // Set the basic properties
        g.transform.parent = chunk.transform;
        g.transform.position = objectPos;
        g.transform.eulerAngles = new Vector3(0, 360, 0);
        g.SetActive(true);

        // Return the object
        return g;
    }

    // Returns a list of the corener positions of the given Bounds
    public List<Vector3> getCorners(Bounds goBounds)
    {
        // Get the bound's width & height
        float width = goBounds.size.x;
        float height = goBounds.size.y;
 
        // Init all the corner vars
        Vector3 topRight = new Vector3(), topLeft = new Vector3(), bottomRight = new Vector3(), bottomLeft = new Vector3();
 
        // Get top right pos
        topRight.x += width / 2;
        topRight.y += height / 2;
 
        // Get top left pos
        topLeft.x -= width / 2;
        topLeft.y += height / 2;
 
        // Get bottom right pos
        bottomRight.x += width / 2;
        bottomRight.y -= height / 2;
 
        // Get bottom left pos
        bottomLeft.x -= width / 2;
        bottomLeft.y -= height / 2;

        // Create a list 
        List<Vector3> cor_temp = new List<Vector3>();
        // Add all the corner positions
        cor_temp.Add(topRight);
        cor_temp.Add(topLeft);
        cor_temp.Add(bottomRight);
        cor_temp.Add(bottomLeft);
     
        // Return the list
        return cor_temp;
    }

    // Returns the bounds
    public Bounds getGroupedBounds(GameObject go)
    {
        // Get all the children
        Renderer[] childs = go.GetComponentsInChildren<Renderer>();
        Bounds tempBounds = childs[0].bounds;

        // Loop through all the children
        for (int i = 0; i < childs.Length; i++) 
        {
            // Encapsulate the child
            if (i != 0)
                tempBounds.Encapsulate(childs[i].bounds);
        }

        // Return the bounds
        return tempBounds;
    }

    // Combines two of the above funcs
    public List<Vector3> getCornersFromGBounds(GameObject go) { return getCorners(getGroupedBounds(go)); }

    // Called to get the structure id using a name
    public int GetStructureId(string name)
    {
        // Loop through the structures list
        for (int i = 0; i < propsGeneration.structuresSettings.StandardBuildings.Length; i++)
        {
            // Return the structure id if the structure exists
            if (propsGeneration.structuresSettings.StandardBuildings[i].name == name)
                return i;
        }

        // Return 0 if we find nothing
        return 0;
    }
}
