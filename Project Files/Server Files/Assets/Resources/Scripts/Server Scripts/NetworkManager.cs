using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance;

    public GameObject playerPrefab;
    public LayerMask itemsLayer;

    public Dictionary<int, Player> players = new Dictionary<int, Player>();
    public Dictionary<string, PlayerData> playerData = new Dictionary<string, PlayerData>();
    public Dictionary<Vector2, ChunkDataStruct> chunkDatas = new Dictionary<Vector2, ChunkDataStruct>();
    public Dictionary<Vector3, List<string>> storageData = new Dictionary<Vector3, List<string>>();

    #region Saving

    void Awake()
    {
        if (BinarySerializer.HasSaved("ServerData.SBDSaveData"))
        {
            SaveData saveData = BinarySerializer.Load<SaveData>("ServerData.SBDSaveData");

            for (int i = 0; i < saveData.storage.Count; i++) { storageData.Add(saveData.storage[i].pos, saveData.storage[i].items); }
            for (int i = 0; i < saveData.playerIps.Count; i++) { playerData.Add(saveData.playerIps[i], saveData.playerDatas[i]); }

            for (int i = 0; i < saveData.chunkData.Count; i++)
            {
                ChunkDataStruct chunkData = new ChunkDataStruct
                {
                    coord = saveData.chunkData[i].coord,
                    heightMap = saveData.chunkData[i].heightMap
                };

                for (int j = 0; j < saveData.chunkData[i].props.Count; j++)
                {
                    PropDataStruct propData = new PropDataStruct
                    {
                        group = saveData.chunkData[i].props[j].group,
                        prop = saveData.chunkData[i].props[j].prop,
                        rot = saveData.chunkData[i].props[j].rot
                    };

                    chunkData.props.Add(saveData.chunkData[i].props[j].pos, propData);
                }
                for (int j = 0; j < saveData.chunkData[i].structures.Count; j++)
                {
                    StructureDataStruct structureData = new StructureDataStruct
                    {
                        structure = saveData.chunkData[i].structures[j].structure,
                        rot = saveData.chunkData[i].structures[j].rot
                    };
                    chunkData.structures.Add(saveData.chunkData[i].structures[j].pos, structureData);
                }

                chunkDatas.Add(chunkData.coord, chunkData);
            }
        }

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 30;

        instance = this;
        Server.Start(50, 26950);
    }

    void OnApplicationQuit()
    {
        SaveData saveData = new SaveData();

        for (int i = 0; i < players.Count; i++)
        {
            PlayerData data = new PlayerData
            {
                health = players.ElementAt(i).Value.health,
                hunger = players.ElementAt(i).Value.hunger,

                pos = players.ElementAt(i).Value.transform.position,
                inventory = players.ElementAt(i).Value.inventory
            };

            char div = ':';
            string[] mess = Server.clients[players.ElementAt(i).Value.id].ip.Split(div);

            if (playerData.ContainsKey(mess[0])) { playerData[mess[0]] = data; } else { playerData.Add(mess[0], data); }
        }

        for (int i = 0; i < playerData.Count; i++) { saveData.playerDatas.Add(playerData.ElementAt(i).Value); saveData.playerIps.Add(playerData.ElementAt(i).Key); }

        for (int i = 0; i < TerrainGenerator.instance.chunkDictionary.Count; i++)
        {
            ChunkDataStruct chunkData = TerrainGenerator.instance.chunkDictionary.ElementAt(i).Value.chunkData;
            SerializableChunkDataStruct newChunkData = new SerializableChunkDataStruct
            {
                coord = chunkData.coord,
                heightMap = chunkData.heightMap
            };

            for (int j = 0; j < chunkData.props.Count; j++)
            {
                SerializablePropDataStruct prop = new SerializablePropDataStruct
                {
                    group = chunkData.props.ElementAt(j).Value.group,
                    prop = chunkData.props.ElementAt(j).Value.prop,
                    pos = chunkData.props.ElementAt(j).Key,
                    rot = chunkData.props.ElementAt(j).Value.rot
                };

                newChunkData.props.Add(prop);
            }
            for (int j = 0; j < chunkData.structures.Count; j++)
            {
                SerializableStructureDataStruct structure = new SerializableStructureDataStruct
                {
                    structure = chunkData.structures.ElementAt(j).Value.structure,
                    pos = chunkData.structures.ElementAt(j).Key,
                    rot = chunkData.structures.ElementAt(j).Value.rot
                };
                newChunkData.structures.Add(structure);
            }

            saveData.chunkData.Add(newChunkData);
        }

        for (int i = 0; i < storageData.Count; i++)
        {
            StorageData sd = new StorageData
            {
                items = storageData.ElementAt(i).Value,
                pos = storageData.ElementAt(i).Key
            };

            saveData.storage.Add(sd);
        }

        BinarySerializer.Save(saveData, "ServerData.SBDSaveData");
        Server.Stop();
    }

    #endregion

    public Player InstantiatePlayer(int id, string ip)
    {
        string newIp = ConvertIP(ip);
        Player player = Instantiate(playerPrefab, new Vector3(0f, 25f, 0f), Quaternion.identity).GetComponent<Player>();

        if (playerData.ContainsKey(newIp))
        {
            player.transform.position = playerData[newIp].pos;
            player.inventory = playerData[newIp].inventory;

            player.health = playerData[newIp].health;
            player.hunger = playerData[newIp].hunger;
        }

        players.Add(id, player);
        return player;
    }

    string ConvertIP(string _ip)
    {
        char div = ':';
        string[] mess = _ip.Split(div);
        return mess[0];
    }
}

[System.Serializable]
public class StorageData
{
    public Vector3 pos;
    public List<string> items = new List<string>();
}

[System.Serializable]
public class SaveData
{
    public List<string> playerIps = new List<string>();
    public List<PlayerData> playerDatas = new List<PlayerData>();

    public List<SerializableChunkDataStruct> chunkData = new List<SerializableChunkDataStruct>();
    public List<StorageData> storage = new List<StorageData>();
}

[System.Serializable]
public class PlayerData
{
    public List<string> inventory = new List<string>();

    public float health;
    public float hunger;

    public Vector3 pos;
}

[System.Serializable]
public class SerializablePropDataStruct
{
    public Vector3 pos;
    public Vector3 rot;
    public int group;
    public int prop;
}

[System.Serializable]
public class SerializableStructureDataStruct
{
    public Vector3 pos;
    public Vector3 rot;
    public string structure;
}

[System.Serializable]
public class SerializableChunkDataStruct
{
    public Vector2 coord;
    public HeightMap heightMap;

    public List<SerializablePropDataStruct> props = new List<SerializablePropDataStruct>();
    public List<SerializableStructureDataStruct> structures = new List<SerializableStructureDataStruct>();
}