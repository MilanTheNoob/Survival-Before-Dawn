using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class TweeningLibrary : MonoBehaviour
{
    // The singleton
    #region Singleton

    // The static reference
    public static TweeningLibrary instance;

    // Set the instance to ourselves
    void Awake()
    {
        instance = this;
    }

    #endregion

    static Dictionary<int, CanvasGroup> cachedCG = new Dictionary<int, CanvasGroup>();
    static Dictionary<int, RectTransform> cachedRT = new Dictionary<int, RectTransform>();

    static List<int> tweening = new List<int>();

    #region Resetting Funcs

    // Set the Alpha Instantly
    public static void SetAlphaInstant(GameObject g, float a)
    {
        // Get the GameObject ID
        int gId = g.GetInstanceID();
        // Create a new CanvasGroup var
        CanvasGroup cg;
        // Get the CanvasGroup
        if (cachedCG.ContainsKey(gId)) { cg = cachedCG[gId]; } else { cg = g.AddComponent<CanvasGroup>(); cachedCG.Add(gId, cg); }

        // Set the alpha
        cg.alpha = a;
    }

    // Set the scale 
    public static void SetScaleInstant(GameObject g, float a)
    {
        // Get the GameObject ID
        int gId = g.GetInstanceID();
        // Create a new RectTransform var
        RectTransform rt;
        // Get the RectTransform
        if (cachedCG.ContainsKey(gId)) { rt = cachedRT[gId]; } else { rt = g.AddComponent<RectTransform>(); cachedRT.Add(gId, rt); }

        // Set the localScale
        rt.localScale = new Vector3(a, a, 1);
    }

    #endregion

    #region Fading Funcs

    // Called to fade in/out UI
    public static void FadeOut(GameObject g, float d) { if (tweening.Contains(g.GetInstanceID()) || instance == null || g == null) { return; } instance.StartCoroutine(FadeCanvasGroup(g, 1, 0, false, d)); }
    public static void FadeIn(GameObject g, float d) { if (tweening.Contains(g.GetInstanceID()) || instance == null || g == null) { return; } g.SetActive(true); instance.StartCoroutine(FadeCanvasGroup(g, 0, 1, true, d)); }

    // The fading coroutine used to affect a UI's alpha
    static IEnumerator FadeCanvasGroup(GameObject g, float start, float end, bool s, float lerpTime = 1)
    {
        // Get the id of the GameObject
        int gId = g.GetInstanceID();
        // Create a new empty comp var
        CanvasGroup cg;
        // Get the CanvasGroup needed
        if (cachedCG.ContainsKey(gId)) { cg = cachedCG[gId]; } else { cg = g.AddComponent<CanvasGroup>(); cachedCG.Add(gId, cg); }

        // Set some variables
        float _timeStartedLerping = Time.time;
        float timeSinceStarted = Time.time - _timeStartedLerping;
        float percentageComplete = timeSinceStarted / lerpTime;

        while (true)
        {
            // Figure out how far along the fading is
            timeSinceStarted = Time.time - _timeStartedLerping;
            percentageComplete = timeSinceStarted / lerpTime;

            // Figure the current value of what the alpha should be
            float currentValue = Mathf.Lerp(start, end, percentageComplete);

            // Set the alpha
            cg.alpha = currentValue;

            // Break away if it has finished fading
            if (percentageComplete >= 1)
            {
                // Set some values of the gO & cg
                g.SetActive(s);
                cg.alpha = 1;

                // Break away
                break;
            }

            // Wait until next frame
            yield return new WaitForFixedUpdate();
        }
    }

    #endregion

    #region Scaling Funcs

    // Scale in/out RectTransforms
    public static void ScaleOut(GameObject g, float d) { instance.StartCoroutine(ScaleRectTransform(g, 1, 0, false, d)); }
    public static void ScaleIn(GameObject g, float d) { g.SetActive(true); instance.StartCoroutine(ScaleRectTransform(g, 0, 1, true)); }

    // Scale Funcs
    public static void ScaleRight(GameObject g, float d, float start, float end) { g.SetActive(true); instance.StartCoroutine(ScaleRectSpecific(g, start, end, 0, d)); }
    public static void ScaleLeft(GameObject g, float d, float start, float end) { g.SetActive(true); instance.StartCoroutine(ScaleRectSpecific(g, start, end, 1, d)); }
    public static void ScaleTop(GameObject g, float d, float start, float end) { g.SetActive(true); instance.StartCoroutine(ScaleRectSpecific(g, start, end, 2, d)); }
    public static void ScaleBottom(GameObject g, float d, float start, float end, bool r) { g.SetActive(true); instance.StartCoroutine(ScaleRectSpecific(g, start, end, 3, d)); }

    // Called to scale a RectTransform
    static IEnumerator ScaleRectTransform(GameObject g, float start, float end, bool s, float lerpTime = 1)
    {
        // Get the id of the gameObject
        int gId = g.GetInstanceID();
        // Create a new empty RectTransform var
        RectTransform rt;
        // Get the RectTransform needed
        if (cachedRT.ContainsKey(gId)) { rt = cachedRT[gId]; } else { rt = g.GetComponent<RectTransform>(); cachedRT.Add(gId, rt); }

        // Set some variables
        float _timeStartedLerping = Time.time;
        float timeSinceStarted = Time.time - _timeStartedLerping;
        float percentageComplete = timeSinceStarted / lerpTime;

        while (true)
        {
            // Figure out how far along the fading is
            timeSinceStarted = Time.time - _timeStartedLerping;
            percentageComplete = timeSinceStarted / lerpTime;

            // Figure the current value of what the alpha should be
            float currentValue = Mathf.Lerp(start, end, percentageComplete);

            // Set the alpha
            rt.localScale = new Vector3(currentValue, currentValue, 1);

            // Break away if it has finished fading
            if (percentageComplete >= 1)
            {
                // Set some vars
                g.SetActive(s);
                rt.localScale = new Vector3(1, 1, 1);

                // Break away
                break;
            }

            // Wait until next frame
            yield return new WaitForFixedUpdate();
        }
    }

    // Specific Scale Enumerators
    static IEnumerator ScaleRectSpecific(GameObject g, float start, float end, int t, float lerpTime = 1)
    {
        // Get the id of the gameObject
        int gId = g.GetInstanceID();
        // Create a new empty RectTransform var
        RectTransform rt;
        // Get the RectTransform needed
        if (cachedRT.ContainsKey(gId)) { rt = cachedRT[gId]; } else { rt = g.GetComponent<RectTransform>(); cachedRT.Add(gId, rt); }

        // Set some variables
        float _timeStartedLerping = Time.time;
        float timeSinceStarted = Time.time - _timeStartedLerping;
        float percentageComplete = timeSinceStarted / lerpTime;

        while (true)
        {
            // Figure out how far along the fading is
            timeSinceStarted = Time.time - _timeStartedLerping;
            percentageComplete = timeSinceStarted / lerpTime;

            // Figure the current value of what the alpha should be
            float currentValue = Mathf.Lerp(start, end, percentageComplete);

            // Set the alpha
            if (t == 0) { rt.SetRight(currentValue); } else if (t == 1) { rt.SetLeft(currentValue); } else if (t == 2) { rt.SetTop(currentValue); } else if (t == 3) { rt.SetBottom(currentValue); }
            rt.right = new Vector3(currentValue, currentValue, 1);

            // Break away if it has finished fading
            if (percentageComplete >= 1)
            {
                rt.localScale = new Vector3(1, 1, 1);

                // Break away
                break;
            }

            // Wait until next frame
            yield return new WaitForFixedUpdate();
        }
    }

    #endregion

    #region Color Funcs

    public static void LerpColor(Image img, Color startColor, Color endColor, float duration) { instance.StartCoroutine(LerpColor(img, startColor.r, endColor.r, duration)); }
    public static void LerpColor(Text txt, Color startColor, Color endColor, float duration) { instance.StartCoroutine(LerpColor(txt, startColor.r, endColor.r, duration)); }

    static IEnumerator LerpColor(Image img, float start, float end, float lerpTime = 1)
    {
        // Set some variables
        float _timeStartedLerping = Time.time;
        float timeSinceStarted = Time.time - _timeStartedLerping;
        float percentageComplete = timeSinceStarted / lerpTime;

        while (true)
        {
            // Figure out how far along the fading is
            timeSinceStarted = Time.time - _timeStartedLerping;
            percentageComplete = timeSinceStarted / lerpTime;

            // Figure the current value of what the alpha should be
            float currentValue = Mathf.Lerp(start, end, percentageComplete);

            // Set the alpha
            Color newColor = new Color(currentValue, currentValue, currentValue);
            img.color = newColor;

            // Break away if it has finished fading
            if (percentageComplete >= 1)
            {
                Color endColor = new Color(currentValue, currentValue, currentValue);
                img.color = endColor;

                // Break away
                break;
            }

            // Wait until next frame
            yield return new WaitForFixedUpdate();
        }
    }
    static IEnumerator LerpColor(Text txt, float start, float end, float lerpTime = 1)
    {
        // Set some variables
        float _timeStartedLerping = Time.time;
        float timeSinceStarted = Time.time - _timeStartedLerping;
        float percentageComplete = timeSinceStarted / lerpTime;

        while (true)
        {
            // Figure out how far along the fading is
            timeSinceStarted = Time.time - _timeStartedLerping;
            percentageComplete = timeSinceStarted / lerpTime;

            // Figure the current value of what the alpha should be
            float currentValue = Mathf.Lerp(start, end, percentageComplete);

            // Set the alpha
            Color newColor = new Color(currentValue, currentValue, currentValue);
            txt.color = newColor;

            // Break away if it has finished fading
            if (percentageComplete >= 1)
            {
                Color endColor = new Color(currentValue, currentValue, currentValue);
                txt.color = endColor;

                // Break away
                break;
            }

            // Wait until next frame
            yield return new WaitForFixedUpdate();
        }
    }

    #endregion
}

public static class RectTransformExtensions
{
    public static void SetLeft(this RectTransform rt, float left)
    {
        rt.offsetMin = new Vector2(left, rt.offsetMin.y);
    }

    public static void SetRight(this RectTransform rt, float right)
    {
        rt.offsetMax = new Vector2(-right, rt.offsetMax.y);
    }

    public static void SetTop(this RectTransform rt, float top)
    {
        rt.offsetMax = new Vector2(rt.offsetMax.x, -top);
    }

    public static void SetBottom(this RectTransform rt, float bottom)
    {
        rt.offsetMin = new Vector2(rt.offsetMin.x, bottom);
    }
}
