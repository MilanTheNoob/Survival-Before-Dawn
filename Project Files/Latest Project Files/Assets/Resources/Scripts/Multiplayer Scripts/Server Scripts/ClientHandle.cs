using System.Net;
using System.Collections.Generic;
using UnityEngine;

public class ClientHandle : MonoBehaviour
{
    public static void Welcome(Packet _packet)
    {
        int _myId = _packet.ReadInt();

        Client.instance.myId = _myId;
        ClientSend.WelcomeReceived();

        MultiplayerInputManager.instance.ToggleUISectionsInt(0);

        Client.instance.udp.Connect(((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port);
    }

    public static void SpawnPlayer(Packet _packet)
    {
        int _id = _packet.ReadInt();
        string _username = _packet.ReadString();
        Vector3 _position = _packet.ReadVector3();
        Quaternion _rotation = _packet.ReadQuaternion();

        GameManager.instance.SpawnPlayer(_id, _username, _position, _rotation);
    }

    public static void PlayerPosition(Packet _packet)
    {
        int _id = _packet.ReadInt();
        Vector3 _position = _packet.ReadVector3();

        GameManager.players[_id].transform.position = _position;
    }

    public static void PlayerRotation(Packet _packet)
    {
        int _id = _packet.ReadInt();
        Quaternion _rotation = _packet.ReadQuaternion();

        GameManager.players[_id].transform.rotation = _rotation;
    }

    public static void PlayerDisconnected(Packet _packet)
    {
        int _id = _packet.ReadInt();

        Destroy(GameManager.players[_id].gameObject);
        GameManager.players.Remove(_id);
    }

    public static void ChunkData(Packet _packet)
    {
        ChunkDataStruct chunkData = new ChunkDataStruct();

        chunkData.coord = _packet.ReadVector2();
        chunkData.heightMap = _packet.ReadHeightMap();

        int propsCount = _packet.ReadInt();

        for (int i = 0; i < propsCount; i++)
        {
            PropDataStruct prop = new PropDataStruct
            {
                group = _packet.ReadInt(),
                prop = _packet.ReadInt(),
                rot = _packet.ReadVector3()
            };
            chunkData.props.Add(_packet.ReadVector3(), prop);
        }

        MultiplayerTerrainGenerator.instance.SpawnChunk(chunkData);
    }

    public static void PropChunkUpdate(Packet _packet)
    {
        Dictionary<Vector3, PropDataStruct> props = new Dictionary<Vector3, PropDataStruct>(); 

        Vector2 coord = _packet.ReadVector2();
        int length = _packet.ReadInt();

        if (!MultiplayerTerrainGenerator.instance.terrainChunkDictionary.ContainsKey(coord)) { return; }

        for (int i = 0; i < length; i++)
        {
            PropDataStruct prop = new PropDataStruct
            {
                group = _packet.ReadInt(),
                prop = _packet.ReadInt(),
                rot = _packet.ReadVector3()
            };

            props.Add(_packet.ReadVector3(), prop);
        }

        MultiplayerTerrainGenerator.instance.terrainChunkDictionary[coord].chunkData.props = props;

        PropsGeneration.instance.RemoveFromChunk(MultiplayerTerrainGenerator.instance.terrainChunkDictionary[coord]);
        PropsGeneration.instance.MultiplayerGenerate(MultiplayerTerrainGenerator.instance.terrainChunkDictionary[coord]);
    }
}
