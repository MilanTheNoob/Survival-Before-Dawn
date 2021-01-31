using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    #region Singleton

    public static MouseLook instance;

    void Awake()
    {
        instance = this;
    }

    #endregion

    public Transform player;

    float xRotation = 0f;

    // Update is called once per frame
    void Update()
    {
        // Manipluate and clamp the y axis for rotation
        xRotation -= InputManager.MouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Rotate the player and camera depending on the Input
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        player.Rotate(Vector3.up * InputManager.MouseX);
    }
}
