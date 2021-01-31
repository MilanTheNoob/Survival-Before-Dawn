using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightManager : MonoBehaviour
{
    #region Singleton

    // The instance of ourselves
    public static HighlightManager instance;

    // Called before Start
    private void Awake()
    {
        // Set the instance to ourselves
        instance = this;
    }

    #endregion

    static List<int> cachedMats = new List<int>();

    // Called to highlight a gameObject
    public static void Highlight(GameObject g)
    {
        // Check the gameObject isn't already highlighted
        if (cachedMats.Contains(g.GetInstanceID()))
            return;

        // Get the g's MeshRenderer
        MeshRenderer mr;
        try { mr = g.GetComponent<MeshRenderer>(); } catch (Exception ex) { return; }

        // Store the g's ID and change the material
        Color32 color = new Color(mr.material.color.r + 0.2f, mr.material.color.g + 0.2f, mr.material.color.b + 0.2f, mr.material.color.a);
        mr.material.color = color;

        // Add the gameObject to the cached list
        cachedMats.Add(g.GetInstanceID());
    }

    // Restores a highlighted gameObject
    public static void Restore(GameObject g)
    {
        // Check it isn't null
        if (g == null)
            return;

        // Check if we have stored the gameObject
        if (!cachedMats.Contains(g.GetInstanceID()))
            return;

        // Get the g's MeshRenderer
        MeshRenderer mr = g.GetComponent<MeshRenderer>();

        // Store the g's ID and change the material
        Color32 color = new Color(mr.material.color.r - 0.2f, mr.material.color.g - 0.2f, mr.material.color.b - 0.2f, mr.material.color.a);
        mr.material.color = color;


        // Remve the gameObject from the cache list
        cachedMats.Remove(g.GetInstanceID());
    }
}
