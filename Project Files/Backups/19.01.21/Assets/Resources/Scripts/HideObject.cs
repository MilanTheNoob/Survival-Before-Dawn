using UnityEngine;
using System.Collections;

public class HideObject : MonoBehaviour
{
    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 dist = (InputManager.instance.player.transform.position - transform.position).normalized;
        if (Vector3.Dot(dist, transform.forward) > InputManager.instance.cullDist) { gameObject.SetActive(false); } else { gameObject.SetActive(true); }
    }
}
