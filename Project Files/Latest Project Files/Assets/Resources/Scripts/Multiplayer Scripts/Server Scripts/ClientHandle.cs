using System.Net;
using System.Linq;
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

        int structuresCount = _packet.ReadInt();

        for (int i = 0; i < structuresCount; i++)
        {
            StructureDataStruct structure = new StructureDataStruct
            {
                structure = _packet.ReadString(),
                rot = _packet.ReadVector3()
            };
            chunkData.structures.Add(_packet.ReadVector3(), structure);
        }

        MultiplayerTerrainGenerator.instance.SpawnChunk(chunkData);
    }

    public static void PropChunkUpdate(Packet _packet)
    {
        Dictionary<Vector3, PropDataStruct> props = new Dictionary<Vector3, PropDataStruct>();

        Vector2 coord = _packet.ReadVector2();
        int length = _packet.ReadInt();

        MultiplayerTerrainGenerator.instance.terrainChunkDictionary[coord].chunkData.props = props;
        GameObject propHolder = MultiplayerTerrainGenerator.instance.terrainChunkDictionary[coord].props;

        Transform[] children = propHolder.GetComponentsInChildren<Transform>();
        for (int j = 0; j < children.Length; j++) { PropsGeneration.instance.RemoveProp(children[j]); }

        Dictionary<Vector3, GameObject> propsDict = new Dictionary<Vector3, GameObject>();

        for (int i = 0; i < length; i++)
        {
            PropDataStruct prop = new PropDataStruct
            {
                group = _packet.ReadInt(),
                prop = _packet.ReadInt(),
                rot = _packet.ReadVector3()
            };
            Vector3 pos = _packet.ReadVector3();
            props.Add(pos, prop);

            GameObject gi = PropsGeneration.instance.propsSettings.PropGroups[prop.group].Props[prop.prop].prefab;
            GameObject g = PropsGeneration.instance.pools[gi.transform.name + " Pool"].pool[0];
            PropsGeneration.instance.pools[gi.transform.name + " Pool"].pool.RemoveAt(0);

            g.transform.parent = MultiplayerTerrainGenerator.instance.terrainChunkDictionary[coord].props.transform;
            g.transform.localPosition = pos;
            g.transform.eulerAngles = prop.rot;
            g.SetActive(true);

            propsDict.Add(g.transform.position, g);
        }

        MultiplayerTerrainGenerator.instance.terrainChunkDictionary[coord].propDict = propsDict;
    }

    public static void StructureChunkUpdate(Packet _packet)
    {
        Dictionary<Vector3, StructureDataStruct> structures = new Dictionary<Vector3, StructureDataStruct>();

        Vector2 coord = _packet.ReadVector2();
        int length = _packet.ReadInt();

        MultiplayerTerrainGenerator.instance.terrainChunkDictionary[coord].chunkData.structures = structures;
        GameObject structuresHolder = MultiplayerTerrainGenerator.instance.terrainChunkDictionary[coord].structures;

        Transform[] children = structuresHolder.GetComponentsInChildren<Transform>();
        for (int j = 0; j < children.Length; j++) { PropsGeneration.instance.RemoveProp(children[j]); }

        Dictionary<Vector3, GameObject> structuresDict = new Dictionary<Vector3, GameObject>();

        for (int i = 0; i < length; i++)
        {
            StructureDataStruct structure = new StructureDataStruct()
            {
                structure = _packet.ReadString(),
                rot = _packet.ReadVector3()
            };
            Vector3 pos = _packet.ReadVector3();
            structures.Add(pos, structure);

            ItemSettings item = Resources.Load<ItemSettings>("Prefabs/Interactable Items/" + structure.structure);
            GameObject g = Instantiate(item.gameObject);

            g.transform.name = item.name;
            g.transform.parent = MultiplayerTerrainGenerator.instance.terrainChunkDictionary[coord].props.transform;
            g.transform.position = pos;
            g.transform.eulerAngles = structure.rot;
            g.SetActive(true);

            structuresDict.Add(g.transform.position, g);
        }

        MultiplayerTerrainGenerator.instance.terrainChunkDictionary[coord].structureDict = structuresDict;
    }

    public static void UpdateVitals(Packet _packet)
    {
        VitalsManager.instance.MultiplayerModifyVitalAmount(0, _packet.ReadFloat());
        VitalsManager.instance.MultiplayerModifyVitalAmount(1, _packet.ReadFloat());
    }
}
