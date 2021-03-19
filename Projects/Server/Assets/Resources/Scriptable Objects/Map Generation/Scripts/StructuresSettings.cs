using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom Scriptable Objects/Map Generation/New Structures Settings")]
public class StructuresSettings : UpdatableSettings
{
    [Header("Basic structure settings")]
    public int poolSizes;
    public int structuresPerChunk;
    public float minSpawnHeight;
    public float maxSpawnHeight;

    [Header("The different structures settings")]
    public StructuresStruct[] StandardBuildings;

    [System.Serializable]
    public class StructuresStruct
    {
        public string name; 
        public StructureSettings structureSettings;
        
        public float yOffset;

        // Returns the structure
        public GameObject getStructure()
        {
            // Create a new empty parent
            GameObject structure = new GameObject(name);

            // Loop through all the objects
            for (int i = 0; i < structureSettings.structureObjects.Count; i++)
            {
                // Create a new gameObject for the object
                GameObject structureObject = new GameObject(structureSettings.structureObjects[i].objectName);

                // Add all the required components
                structureObject.AddComponent<MeshFilter>();
                structureObject.AddComponent<MeshRenderer>();
                structureObject.AddComponent<MeshCollider>();

                // Set the meshes of the filter & collider
                structureObject.GetComponent<MeshFilter>().sharedMesh = structureSettings.structureObjects[i].objectMesh;
                structureObject.GetComponent<MeshCollider>().sharedMesh = structureSettings.structureObjects[i].objectMesh;


                // Set all the basic properties correctly (pos, rot, etc)
                structureObject.transform.parent = structure.transform;
                structureObject.transform.localPosition = structureSettings.structureObjects[i].objectPosition;
                structureObject.transform.localRotation = structureSettings.structureObjects[i].objectRotation;
                structureObject.transform.localScale = structureSettings.structureObjects[i].objectScale;

                // Set the material of the object
                structureObject.GetComponent<Renderer>().sharedMaterial = structureSettings.structureObjects[i].objectMaterial;
            }

            // Returns the structure we created
            return structure;
        }
    }
}
