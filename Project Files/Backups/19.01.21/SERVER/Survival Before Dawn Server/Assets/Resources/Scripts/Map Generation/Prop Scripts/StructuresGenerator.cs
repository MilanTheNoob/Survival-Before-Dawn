using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StructuresGenerator : MonoBehaviour
{
    [HideInInspector]
    public PropsGeneration propsGeneration;
    [HideInInspector]
    public PropsReferences propsReferences;
    [HideInInspector]
    public PropsSaving propsSaving;
    [HideInInspector]
    public PropsPooling propsPooling;

    // Called to generate structures
    public List<Vector3> GenerateStructures(GameObject chunk, MeshFilter mf, BoxCollider triggerCollider)
    {
        //Get all the vertices of the chunk
        List<Vector3> vertices = mf.mesh.vertices.ToList();

        // Initialise the seed for generating randomly
        int seed = TerrainGenerator.instance.heightMapSettings.noiseSettings.seed;
        Random.InitState(seed + (int)chunk.transform.position.x + (int)chunk.transform.position.z);

        // Cycle through all the structures
        for (int j = 0; j < propsGeneration.structuresSettings.structuresPerChunk; j++)
        {
            // Get a random structure
            int rn = Random.Range(0, propsGeneration.structuresSettings.StandardBuildings.Length);
            string n = propsGeneration.structuresSettings.StandardBuildings[rn].name + " Pool";
            PoolData pool = propsGeneration.pools[n];

            // Set all the variables including the pos of the corners position
            List<Vector3> cornerPositions = propsReferences.getCornersFromGBounds(pool.pool[0]);
            bool isInChunk = false;
            int attempts = 0;
            int t = 0;

            // Keep looping until we get a good pos or until we run out of atte
            while (!isInChunk && attempts < 10)
            {
                // Get a random position & init a variable
                t = (int)Random.Range(0, vertices.Count);
                bool cornerIsOutside = false;

                // Add to the attempts
                attempts++;

                // Loop through the corner positions
                for (int x = 0; x < cornerPositions.Count; x++)
                {
                    Vector3 point = chunk.transform.TransformPoint(cornerPositions[x] + vertices[t]);

                    // Check if the corner pos is inside the chunk
                    if (!triggerCollider.bounds.Contains(point))
                        cornerIsOutside = true;
                }

                // If none of the corners
                if (!cornerIsOutside)
                    isInChunk = true;
            }

            // Place the object
            GameObject structure = propsReferences.PlaceStructure(chunk, pool, chunk.transform.TransformPoint(vertices[t]), rn);

            if (structure != null)
            {
                Bounds structureBounds = propsReferences.getGroupedBounds(structure);

                for (int x = 0; x < vertices.Count; x++)
                {
                    // Remove all the vertices that collide with the structure
                    if (structureBounds.Contains(chunk.transform.TransformPoint(vertices[x])))
                        vertices.RemoveAt(x);
                }
            }

        }

        if (vertices.Count <= 0)
            vertices.Add(Vector3.zero);

        // Return the vertices
        return vertices;
    }
}
