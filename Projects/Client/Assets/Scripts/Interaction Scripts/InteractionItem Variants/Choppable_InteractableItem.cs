using System.Collections;
using UnityEngine;

public class Choppable_InteractableItem : InteractableItem
{
    [Header("The items that are dropped from the tree when chopped")]
    public ItemSettings[] droppedItemSettings;

    void Awake() { interactTxt = "Chop"; }

    public override void OnInteract()
    {
        base.OnInteract();

        AudioManager.PlayChop();
        ToolsManager.instance.SwingAnim();
        isInteractable = false;

        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        if (rb == null) { rb = gameObject.AddComponent<Rigidbody>(); }

        rb.useGravity = true;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.AddForce(Vector3.forward, ForceMode.Impulse);

        StartCoroutine(ReplaceTreeI());
    }

    IEnumerator ReplaceTreeI()
    {
        yield return new WaitForSeconds(2f);

        for (int i = 0; i < droppedItemSettings.Length; i++)
        {
            GameObject newProp = Instantiate(droppedItemSettings[i].gameObject);

            newProp.transform.parent = gameObject.transform.parent;
            newProp.transform.localPosition = gameObject.transform.localPosition;
            newProp.transform.name = droppedItemSettings[i].gameObject.name;

            if (newProp.GetComponent<MeshCollider>() == null) { newProp.AddComponent<MeshCollider>(); }
            newProp.GetComponent<MeshCollider>().convex = true;

            Rigidbody tmpRb = newProp.AddComponent<Rigidbody>();
            tmpRb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            tmpRb.useGravity = true;
        }

        DestroyImmediate(gameObject);
    }
}
