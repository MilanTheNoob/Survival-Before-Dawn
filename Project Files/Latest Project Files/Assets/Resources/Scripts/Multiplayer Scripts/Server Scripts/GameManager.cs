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

    public void SpawnPlayer(int _id, string _username, Vector3 _position, Quaternion _rotation)
    {
        GameObject _player;

        if (_id == Client.instance.myId)
        {
            _player = Instantiate(localPlayerPrefab, _position, _rotation);
            _player.transform.name = "Player";

            MultiplayerInputManager.player = _player;
            MultiplayerTerrainGenerator.instance.viewer = _player.transform;
        }
        else { _player = Instantiate(playerPrefab, _position, _rotation); }

        PlayerManager _pm = _player.GetComponent<PlayerManager>();

        _pm.id = _id;
        _pm.username = _username;
        players.Add(_id, _pm);
    }
}
