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
    public GameObject cameraPos;

    public List<Vector2> chunks;

    public bool interacting;
    public bool moving;

    public float health;
    public float hunger;

    float horizontal;
    float vertical;
    bool jump;
    float yVelocity = 0;

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

        health = 1f;
        hunger = 1f;
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
            ServerSend.InteractItem(currentItem, this);
        }
        if (interacting && !hitInteractableObject) 
        {
            ServerSend.StopInteractItem(this); 
            currentItem = null; 
            interacting = false; 
        }

        #endregion

        #region Vitals

        if (health <= 0f)
        {
            health = 1f;
            hunger = 1f;

            controller.enabled = false;
            controller.transform.position = new Vector3(0f, 25f, 0f);
            controller.enabled = true;
        }
        else if (health > 1f) { health = 1f; }

        if (hunger <= 0f)
        {
            if (health > 0f) { health -= 0.0003f; }
        }
        else if (hunger > 1f)
        {
            hunger = 1f;
        }
        else { hunger -= 0.00006f; }

        ServerSend.UpdateVitals(this);

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

    public void SetInput(float _horizontal, float _vertical, Quaternion _rotation, Quaternion _camera, bool _jump, bool _moving)
    {
        horizontal = _horizontal;
        vertical = _vertical;
        jump = _jump;

        transform.rotation = _rotation;
        cameraPos.transform.rotation = _camera;

        moving = _moving;
    }
}
