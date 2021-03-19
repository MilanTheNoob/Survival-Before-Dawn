using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DOFManager : MonoBehaviour
{
    public VolumeProfile volume;

    DepthOfField dof;
    Camera cam;

    void Start()
    {
        volume.TryGet(out dof);
        cam = SavingManager.player.GetComponentInChildren<Camera>();
    }

    void FixedUpdate()
    {
        RaycastHit hitInfo;
        Ray rayDOF = new Ray(cam.transform.position, cam.transform.forward * 10);

        if (Physics.Raycast(rayDOF, out hitInfo))
        {
            dof.focusDistance.value = Mathf.Lerp(dof.focusDistance.value, Mathf.Clamp(Vector3.Distance(cam.transform.position, hitInfo.point), 1f, 200f), Time.deltaTime * 9);
        }
        else
        {
            if (Vector3.Distance(cam.transform.position, hitInfo.point) < 5)
            {
                dof.focusDistance.value++;
            }
        }
    }
}
