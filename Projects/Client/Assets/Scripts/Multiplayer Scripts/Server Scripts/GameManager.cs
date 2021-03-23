using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;

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

    public GameObject playerPrefab;

    void Start()
    {
        players.Clear();
        StartCoroutine(WaitExitI());
    }

    IEnumerator WaitExitI()
    {
        yield return new WaitForSeconds(2f);
        if (players.Count == 0) { SceneManager.LoadScene(0, LoadSceneMode.Single); }
    }

    public void SpawnPlayer(int _id, string _username, Vector3 _position, Quaternion _rotation)
    {
        GameObject _player;

        if (_id == Client.instance.myId)
        {
            _player = Instantiate(playerPrefab, _position, _rotation);
            _player.transform.name = "Player";

            MultiplayerInputManager.player = _player;
            MultiplayerTerrainGenerator.instance.viewer = _player.transform;

            Destroy(_player.GetComponentInChildren<Camera>().gameObject);
        }
        else { _player = Instantiate(playerPrefab, _position, _rotation); }

        PlayerManager _pm = _player.GetComponent<PlayerManager>();

        _pm.id = _id;
        _pm.username = _username;
        players.Add(_id, _pm);
    }
}
