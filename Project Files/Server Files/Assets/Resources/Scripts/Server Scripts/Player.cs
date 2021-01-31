using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int id;
    public string username;

    public CharacterController controller;
    public float gravity = -9.81f;
    public float moveSpeed = 5f;
    public float jumpSpeed = 10f;
    public Vector3 oldPos;

    public List<string> inventory = new List<string>();
    public GameObject currentItem;

    public List<Vector2> chunks;

    float horizontal;
    float vertical;
    bool jump;
    float yVelocity = 0;

    GameObject cameraPos;

    bool interacting;

    private void Start()
    {
        gravity *= Time.fixedDeltaTime * Time.fixedDeltaTime;
        moveSpeed *= Time.fixedDeltaTime;
        jumpSpeed *= Time.fixedDeltaTime;
    }

    public void Initialize(int _id, string _username)
    {
        id = _id;
        username = _username;

        cameraPos = new GameObject();
        cameraPos.transform.parent = controller.transform;
        cameraPos.transform.localPosition = new Vector3(0f, 1.6f, 0.4f);
    }

    public void FixedUpdate()
    {
        #region Interaction

        Ray ray = new Ray(cameraPos.transform.position, cameraPos.transform.forward);
        RaycastHit hitInfo;

        bool hitInteractableObject = Physics.SphereCast(ray, 2, out hitInfo, 2, NetworkManager.instance.itemsLayer);
        if (hitInteractableObject && !interacting)
        {
            interacting = true;
            currentItem = hitInfo.transform.gameObject;
            ServerSend.InteractItem("Interact with", currentItem.transform.name, this);
        }
        else if (interacting && currentItem != null && !hitInteractableObject) 
        { 
            ServerSend.StopInteractItem(this); 
            currentItem = null; 
            interacting = false; 
        }

        #endregion

        Vector3 _moveDirection = transform.right * horizontal + transform.forward * vertical;
        _moveDirection *= moveSpeed;

        if (controller.isGrounded)
        {
            yVelocity = 0f;
            if (jump)
                yVelocity = jumpSpeed;
        }
        yVelocity += gravity;

        _moveDirection.y = yVelocity;
        controller.Move(_moveDirection);

        ServerSend.PlayerPosition(this);
        ServerSend.PlayerRotation(this);
    }

    public void SetInput(float _horizontal, float _vertical, Quaternion _rotation, bool _jump)
    {
        horizontal = _horizontal;
        vertical = _vertical;
        jump = _jump;

        transform.rotation = _rotation;
    }
}
