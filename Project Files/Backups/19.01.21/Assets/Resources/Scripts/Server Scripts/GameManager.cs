using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton

    public static GameManager instance;

    private void Awake()
    {
        instance = this;
    }

    #endregion

    public static Dictionary<int, PlayerManager> players = new Dictionary<int, PlayerManager>();

    public GameObject localPlayerPrefab;
    public GameObject playerPrefab;

    // Called to spawn a player
    public void SpawnPlayer(int _id, string _username, Vector3 _position, Quaternion _rotation)
    {
        // Create a new var to store the player
        GameObject _player;

        if (_id == Client.instance.myId)
        {
            // Instantiate the localPlayer
            _player = Instantiate(localPlayerPrefab, _position, _rotation);
            _player.transform.name = "Player";

            // Destroy the old one and replace it
            Destroy(InputManager.instance.player);
            InputManager.instance.player = _player;
            TerrainGenerator.instance.viewer = _player.transform;
        }
        else
        {
            // Instantiate a non-local player
            _player = Instantiate(playerPrefab, _position, _rotation);
        }

        // Get the PlayerManager
        PlayerManager _pm = _player.GetComponent<PlayerManager>();

        // Set the values of the player
        _pm.id = _id;
        _pm.username = _username;
        players.Add(_id, _pm);
    }
}
