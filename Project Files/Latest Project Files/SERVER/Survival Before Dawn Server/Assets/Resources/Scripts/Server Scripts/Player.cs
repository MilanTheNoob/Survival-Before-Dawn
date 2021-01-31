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
    public float jumpSpeed = 5f;

    public Vector3 oldPos;

    private float horizontal;
    private float vertical;
    private bool jump;

    private float yVelocity = 0;

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
    }

    public void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        Vector2 _iDirection = new Vector2();

        if (horizontal < -0.2f)
            _iDirection.x = -1;

        if (horizontal > 0.2f)
            _iDirection.x = 1;

        if (vertical > 0f)
            _iDirection.y = 1;

        if (vertical < 0f)
            _iDirection.y = -1;

        Vector3 _moveDirection = transform.right * _iDirection.x + transform.forward * _iDirection.y;
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
