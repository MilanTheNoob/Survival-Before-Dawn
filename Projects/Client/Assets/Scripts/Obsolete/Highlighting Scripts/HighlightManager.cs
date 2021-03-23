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

    static Dictionary<int, Outline> outlines = new Dictionary<int, Outline>();

    public static void Highlight(GameObject g)
    {
        if (outlines.ContainsKey(g.GetInstanceID())) { return; }

        Outline outline = g.AddComponent<Outline>();
        outlines.Add(g.GetInstanceID(), outline);

        instance.StartCoroutine(FadeInOutline(outline));
    }

    static IEnumerator FadeInOutline(Outline o)
    {
        float _timeStartedLerping = Time.time;
        float timeSinceStarted = Time.time - _timeStartedLerping;
        float percentageComplete = timeSinceStarted / 0.3f;

        while (true)
        {
            timeSinceStarted = Time.time - _timeStartedLerping;
            percentageComplete = timeSinceStarted / 0.3f;

            float currentValue = Mathf.Lerp(0f, 15f, percentageComplete);
            o.OutlineWidth = currentValue;

            if (percentageComplete >= 1) { break; }
            yield return new WaitForFixedUpdate();
        }
    }

    public static void Restore(GameObject g)
    {
        if (g == null) { return; }
        if (!outlines.ContainsKey(g.GetInstanceID())) { return; }

        instance.StartCoroutine(FadeOutOutline(outlines[g.GetInstanceID()]));
    }

    static IEnumerator FadeOutOutline(Outline o)
    {
        float _timeStartedLerping = Time.time;
        float timeSinceStarted = Time.time - _timeStartedLerping;
        float percentageComplete = timeSinceStarted / 0.3f;

        while (true)
        {
            if (o == null) { break; }

            timeSinceStarted = Time.time - _timeStartedLerping;
            percentageComplete = timeSinceStarted / 0.3f;

            float currentValue = Mathf.Lerp(15f, 0f, percentageComplete);
            o.OutlineWidth = currentValue;

            if (percentageComplete >= 1) { outlines.Remove(o.gameObject.GetInstanceID()); Destroy(o); break; }

            yield return new WaitForFixedUpdate();
        }
    }
    public static void Remove(GameObject g)
    {
        if (g == null) { return; }
        if (!outlines.ContainsKey(g.GetInstanceID())) { return; }

        Destroy(outlines[g.GetInstanceID()]);
        outlines.Remove(g.GetInstanceID());
    }
}
