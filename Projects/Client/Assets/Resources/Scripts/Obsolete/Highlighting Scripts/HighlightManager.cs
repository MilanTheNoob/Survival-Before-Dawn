using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightManager : MonoBehaviour
{
    #region Singleton

    public static HighlightManager instance;
    void Awake() { instance = this; }

    #endregion

    public Material outlineMat;
    public float outlineScale;

    static MeshRenderer currentRend;

    public static void Highlight(GameObject g)
    {
        /*
        if (currentRend != null || g.GetComponent<MeshFilter>() == null || g == null) { return; }

        GameObject outlineG = new GameObject();
        outlineG.transform.parent = g.transform;
        outlineG.transform.localPosition = Vector3.zero;
        outlineG.transform.localRotation = Quaternion.identity;
        outlineG.AddComponent<MeshFilter>().sharedMesh = g.GetComponent<MeshFilter>().sharedMesh;
        currentRend = outlineG.AddComponent<MeshRenderer>();

        currentRend.material = instance.outlineMat;
        currentRend.material.SetColor("_OutlineColor", new Color(1, 1, 1));
        currentRend.material.SetFloat("_Scale", instance.outlineScale);

        /*
        for (int i = 0; i < currentRend.sharedMaterials.Length; i++)  
        {
            print("boo");

            currentRend.sharedMaterials[i] = instance.outlineMat;
            currentRend.sharedMaterials[i].SetColor("_OutlineColor", new Color(1, 1, 1));
            currentRend.sharedMaterials[i].SetFloat("_Scale", instance.outlineScale);
        }
        currentRend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        */
    }

    public static void Restore(GameObject g)
    {
        /*
        if (currentRend == null || g == null) { return; }

        Destroy(currentRend.gameObject);
        currentRend = null;
        */
    }
}
