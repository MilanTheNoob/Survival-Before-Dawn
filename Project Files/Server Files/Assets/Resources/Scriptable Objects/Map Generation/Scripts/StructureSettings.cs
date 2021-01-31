using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom Scriptable Objects/Map Generation/New Structure Settings")]
public class StructureSettings : UpdatableSettings
{
    [Header("The different structures settings")]
    public List<StructureStruct> structureObjects = new List<StructureStruct>();

    // Called to add an object to the structure
    public void AddObject(string objName, Mesh objMesh, Vector3 objPos, Quaternion objRot, Vector3 objScale, Material objMat)
    {
        // Create a new struct
        StructureStruct structureStruct = new StructureStruct();

        // Add all the data to the struct
        structureStruct.objectName = objName;
        structureStruct.objectMesh = objMesh;
        structureStruct.objectPosition = objPos;
        structureStruct.objectRotation = objRot;
        structureStruct.objectScale = objScale;
        structureStruct.objectMaterial = objMat;

        // Add the struct to the list
        structureObjects.Add(structureStruct);
    }

    [System.Serializable]
    public class StructureStruct
    {
        public string objectName;
        public Mesh objectMesh;
        public Vector3 objectPosition;
        public Quaternion objectRotation;
        public Vector3 objectScale;
        public Material objectMaterial;
    }
}
