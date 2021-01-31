using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    #region Singleton

    public static NetworkManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    #endregion

    public GameObject playerPrefab;
    public List<Player> players;

    TerrainGenerator terrainGenerator;

    private void Start()
    {
        terrainGenerator = TerrainGenerator.instance;

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 30;

        Server.Start(50, 26950);
    }

    private void FixedUpdate()
    {
        //terrainGenerator.UpdateChunks(players);
    }

    private void OnApplicationQuit()
    {
        Server.Stop();
    }

    public Player InstantiatePlayer()
    {
        Player player = Instantiate(playerPrefab, new Vector3(0f, 25f, 0f), Quaternion.identity).GetComponent<Player>();
        players.Add(player);

        terrainGenerator.SpawnChunksOnPlayer(player);

        return player;
    }
}