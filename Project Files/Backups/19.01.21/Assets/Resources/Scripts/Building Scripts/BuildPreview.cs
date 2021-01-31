using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildPreview : MonoBehaviour
{
    [Header("The type of structure")]
    public StructureType structureType;

    List<SphereCollider> foundationSnaps = new List<SphereCollider>();
    List<SphereCollider> wallSnaps = new List<SphereCollider>();
    List<SphereCollider> ceilingSnaps = new List<SphereCollider>();

    bool isSnapped;

    public enum StructureType
    {
        Foundation,
        Wall,
        Furniture,
        Storage
    }

    // Start is called before the first frame update
    void Start()
    {
        #region Snap Points

        // Add all the snap points depending on the structure type (don't expect me to comment it all)
        if (structureType == StructureType.Foundation)
        {
            foundationSnaps.Add(new GameObject("Snap Point 0, 0, 1").AddComponent<SphereCollider>());
            foundationSnaps[0].isTrigger = true;
            foundationSnaps[0].radius = 0.25f;
            foundationSnaps[0].transform.parent = gameObject.transform;
            foundationSnaps[0].transform.localPosition = new Vector3(0f, 0f, 1f);
            foundationSnaps[0].tag = "Foundation_SP";
            foundationSnaps[0].gameObject.AddComponent<Rigidbody>().isKinematic = true;
            foundationSnaps.Add(new GameObject("Snap Point 1, 0, 0").AddComponent<SphereCollider>());
            foundationSnaps[1].isTrigger = true;
            foundationSnaps[1].radius = 0.25f;
            foundationSnaps[1].transform.parent = gameObject.transform;
            foundationSnaps[1].transform.localPosition = new Vector3(1f, 0f, 0f);
            foundationSnaps[1].tag = "Foundation_SP";
            foundationSnaps[1].gameObject.AddComponent<Rigidbody>().isKinematic = true;
            foundationSnaps.Add(new GameObject("Snap Point 0, 0, -1").AddComponent<SphereCollider>());
            foundationSnaps[2].isTrigger = true;
            foundationSnaps[2].radius = 0.25f;
            foundationSnaps[2].transform.parent = gameObject.transform;
            foundationSnaps[2].transform.localPosition = new Vector3(0f, 0f, -1f);
            foundationSnaps[2].tag = "Foundation_SP";
            foundationSnaps[2].gameObject.AddComponent<Rigidbody>().isKinematic = true;
            foundationSnaps.Add(new GameObject("Snap Point -1, 0, 0").AddComponent<SphereCollider>());
            foundationSnaps[3].isTrigger = true;
            foundationSnaps[3].radius = 0.25f;
            foundationSnaps[3].transform.parent = gameObject.transform;
            foundationSnaps[3].transform.localPosition = new Vector3(-1f, 0f, 0f);
            foundationSnaps[3].tag = "Foundation_SP";
            foundationSnaps[3].gameObject.AddComponent<Rigidbody>().isKinematic = true;

            wallSnaps.Add(new GameObject("Snap Point 0, 2.5, 0.5").AddComponent<SphereCollider>());
            wallSnaps[0].isTrigger = true;
            wallSnaps[0].radius = 0.5f;
            wallSnaps[0].transform.parent = gameObject.transform;
            wallSnaps[0].transform.localPosition = new Vector3(0f, 2.5f, 0.5f);
            wallSnaps[0].tag = "Wall_SP";
            wallSnaps[0].gameObject.AddComponent<Rigidbody>().isKinematic = true;
            wallSnaps.Add(new GameObject("Snap Point 0.5, 2.5, 0").AddComponent<SphereCollider>());
            wallSnaps[1].isTrigger = true;
            wallSnaps[1].radius = 0.5f;
            wallSnaps[1].transform.parent = gameObject.transform;
            wallSnaps[1].transform.localPosition = new Vector3(0.5f, 2.5f, 0f);
            wallSnaps[1].tag = "Wall_SP";
            wallSnaps[1].gameObject.AddComponent<Rigidbody>().isKinematic = true;
            wallSnaps.Add(new GameObject("Snap Point 0, 2.5, -0.5").AddComponent<SphereCollider>());
            wallSnaps[2].isTrigger = true;
            wallSnaps[2].radius = 0.5f;
            wallSnaps[2].transform.parent = gameObject.transform;
            wallSnaps[2].transform.localPosition = new Vector3(0f, 2.5f, -0.5f);
            wallSnaps[2].tag = "Wall_SP";
            wallSnaps[2].gameObject.AddComponent<Rigidbody>().isKinematic = true;
            wallSnaps.Add(new GameObject("Snap Point -0.5, 2.5, 0").AddComponent<SphereCollider>());
            wallSnaps[3].isTrigger = true;
            wallSnaps[3].radius = 0.5f;
            wallSnaps[3].transform.parent = gameObject.transform;
            wallSnaps[3].transform.localPosition = new Vector3(-0.5f, 2.5f, -0f);
            wallSnaps[3].tag = "Wall_SP";
            wallSnaps[3].gameObject.AddComponent<Rigidbody>().isKinematic = true;

            wallSnaps.Add(new GameObject("Snap Point 0, -2.5, 0.5").AddComponent<SphereCollider>());
            wallSnaps[4].isTrigger = true;
            wallSnaps[4].radius = 0.5f;
            wallSnaps[4].transform.parent = gameObject.transform;
            wallSnaps[4].transform.localPosition = new Vector3(0f, -2.5f, 0.5f);
            wallSnaps[4].tag = "Wall_SP";
            wallSnaps[4].gameObject.AddComponent<Rigidbody>().isKinematic = true;
            wallSnaps.Add(new GameObject("Snap Point 0.5, -2.5, 0").AddComponent<SphereCollider>());
            wallSnaps[5].isTrigger = true;
            wallSnaps[5].radius = 0.5f;
            wallSnaps[5].transform.parent = gameObject.transform;
            wallSnaps[5].transform.localPosition = new Vector3(0.5f, -2.5f, 0f);
            wallSnaps[5].tag = "Wall_SP";
            wallSnaps[5].gameObject.AddComponent<Rigidbody>().isKinematic = true;
            wallSnaps.Add(new GameObject("Snap Point 0, -2.5, -0.5").AddComponent<SphereCollider>());
            wallSnaps[6].isTrigger = true;
            wallSnaps[6].radius = 0.5f;
            wallSnaps[6].transform.parent = gameObject.transform;
            wallSnaps[6].transform.localPosition = new Vector3(0f, -2.5f, -0.5f);
            wallSnaps[6].tag = "Wall_SP";
            wallSnaps[6].gameObject.AddComponent<Rigidbody>().isKinematic = true;
            wallSnaps.Add(new GameObject("Snap Point -0.5, -2.5, 0").AddComponent<SphereCollider>());
            wallSnaps[7].isTrigger = true;
            wallSnaps[7].radius = 0.5f;
            wallSnaps[7].transform.parent = gameObject.transform;
            wallSnaps[7].transform.localPosition = new Vector3(-0.5f, -2.5f, -0f);
            wallSnaps[7].tag = "Wall_SP";
            wallSnaps[7].gameObject.AddComponent<Rigidbody>().isKinematic = true;

            isSnapped = true;
        }
        else if (structureType == StructureType.Wall)
        {
            wallSnaps.Add(new GameObject("Snap Point 0, 0, 1").AddComponent<SphereCollider>());
            wallSnaps[0].isTrigger = true;
            wallSnaps[0].radius = 0.25f;
            wallSnaps[0].transform.parent = gameObject.transform;
            wallSnaps[0].transform.localPosition = new Vector3(0f, 2.5f, 0.5f);
            wallSnaps[0].tag = "Wall_SP";
            wallSnaps[0].gameObject.AddComponent<Rigidbody>().isKinematic = true;
            wallSnaps.Add(new GameObject("Snap Point 0, 0, -1").AddComponent<SphereCollider>());
            wallSnaps[1].isTrigger = true;
            wallSnaps[1].radius = 0.25f;
            wallSnaps[1].transform.parent = gameObject.transform;
            wallSnaps[1].transform.localPosition = new Vector3(0.5f, 2.5f, 0f);
            wallSnaps[1].tag = "Wall_SP";
            wallSnaps[1].gameObject.AddComponent<Rigidbody>().isKinematic = true;
            wallSnaps.Add(new GameObject("Snap Point 0, 1, 0").AddComponent<SphereCollider>());
            wallSnaps[2].isTrigger = true;
            wallSnaps[2].radius = 0.25f;
            wallSnaps[2].transform.parent = gameObject.transform;
            wallSnaps[2].transform.localPosition = new Vector3(0f, 2.5f, -0.5f);
            wallSnaps[2].tag = "Wall_SP";
            wallSnaps[2].gameObject.AddComponent<Rigidbody>().isKinematic = true;

            ceilingSnaps.Add(new GameObject("Snap Point 2.5, 0.5, 0").AddComponent<SphereCollider>());
            ceilingSnaps[0].isTrigger = true;
            ceilingSnaps[0].radius = 0.25f;
            ceilingSnaps[0].transform.parent = gameObject.transform;
            ceilingSnaps[0].transform.localPosition = new Vector3(2.5f, 0.5f, 0f);
            ceilingSnaps[0].tag = "Foundation_SP";
            ceilingSnaps[0].gameObject.AddComponent<Rigidbody>().isKinematic = true;
            ceilingSnaps.Add(new GameObject("Snap Point -2.5, 0.5, 0").AddComponent<SphereCollider>());
            ceilingSnaps[1].isTrigger = true;
            ceilingSnaps[1].radius = 0.25f;
            ceilingSnaps[1].transform.parent = gameObject.transform;
            ceilingSnaps[1].transform.localPosition = new Vector3(-2.5f, 0.5f, 0f);
            ceilingSnaps[1].tag = "Foundation_SP";
            ceilingSnaps[1].gameObject.AddComponent<Rigidbody>().isKinematic = true;
        }
        else if (structureType == StructureType.Furniture || structureType == StructureType.Storage) { isSnapped = true; }

        #endregion

        // Add a boxcollider if there is none
        if (gameObject.GetComponent<BoxCollider>() != null)
            gameObject.AddComponent<BoxCollider>();

        // Update the color of the structure
        try { HighlightManager.Highlight(gameObject); } catch(Exception ex) { }

        // Change the layer of all children
        Transform[] children = transform.GetComponentsInChildren<Transform>();
        for (int i = 0; i < children.Length; i++) { children[i].gameObject.layer = 10; }
    }

    // Called when colliding with something
    void OnTriggerEnter(Collider other)
    {
        // What structure type are we?
        if (structureType == StructureType.Foundation && other.CompareTag("Foundation_SP") || structureType == StructureType.Wall && other.CompareTag("Wall_SP"))
        {
            // Snap
            Snapped(other.transform.position);
        }
    }

    // It snaps to da pos
    void Snapped(Vector3 pos)
    {
        // Set the pause build of buildingmanager to true
        BuildingManager.PauseBuild(true);

        // Set the position and snap
        transform.position = pos;
        isSnapped = true;
    }

    // Places the structure
    public bool Place()
    {
        // Restore the highlighting of the gameObject
        HighlightManager.Restore(gameObject);
        // Add an interactable script to the gameObject
        if (structureType == StructureType.Storage) { gameObject.AddComponent<Storage_InteractableItem>(); } else { gameObject.AddComponent<Structure_InteractableItem>(); }

        // Set the parent
        TerrainGenerator.AddToNearestChunk(gameObject, TerrainGenerator.ChildType.StructurePiece);

        // Destroy this component
        Destroy(this);

        // Return true
        return true;
    }

    // Returns if we are snapped or not
    public bool GetSnapped() { return isSnapped; }
}
