using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace GameServer
{
    class Player
    {
        public int id;
        public string username;

        public Vector3 position;
        public Quaternion rotation;

        private float moveSpeed = 5f / Constants.TICKS_PER_SEC;

        private float horizontal;
        private float vertical;

        public Player(int _id, string _username, Vector3 _spawnPosition)
        {
            id = _id;
            username = _username;
            position = _spawnPosition;
            rotation = Quaternion.Identity;
        }

        public void Update()
        {
            Move();
        }

        private void Move()
        {
            Vector3 _forward = Vector3.Transform(new Vector3(0, 0, 1), rotation);
            Vector3 _right = Vector3.Normalize(Vector3.Cross(_forward, new Vector3(0, 1, 0)));
            Vector2 _iDirection = new Vector2();

            if (horizontal < -0.2f)
                _iDirection.X = 1;

            if (horizontal > 0.2f)
                _iDirection.X = -1;

            if (vertical > 0f)
                _iDirection.Y = 1;

            if (vertical < 0f)
                _iDirection.Y = -1;

            Vector3 _moveDirection = _right * _iDirection.X + _forward * _iDirection.Y;
            position += _moveDirection * moveSpeed;

            ServerSend.PlayerPosition(this);
            ServerSend.PlayerRotation(this);
        }

        public void SetInput(float _horizontal, float _vertical, Quaternion _rotation)
        {
            horizontal = _horizontal;
            vertical = _vertical;

            rotation = _rotation;
        }
    }
}
