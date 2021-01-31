using System;
using UnityEngine;

public class Present_InteractableItem : InteractableItem
{
    [Header("List of things the present could have in it")]
    public ItemSettings[] items;

    void Awake() { interactTxt = "Open"; }

    public override void OnInteract()
    {
        base.OnInteract();

        GameObject g = Instantiate(items[UnityEngine.Random.Range(0, items.Length)].gameObject, new Vector3(transform.position.x, transform.position.y + 10f, transform.position.z), Quaternion.identity);
        g.AddComponent<Rigidbody>();
        try { g.GetComponent<MeshCollider>().convex = true; } catch(Exception ex) { }

        Destroy(gameObject);
    }
}
