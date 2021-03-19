using System.Collections.Generic;
using UnityEngine;

public class ServerHandle
{
    public static void WelcomeReceived(int _fromClient, Packet _packet)
    {
        int _clientIdCheck = _packet.ReadInt();
        string _username = _packet.ReadString();

        Server.clients[_fromClient].SendIntoGame(_username);
    }

    public static void PlayerMovement(int _fromClient, Packet _packet)
    {
        float horizontal = _packet.ReadFloat();
        float vertical = _packet.ReadFloat();
        bool jump = _packet.ReadBool();

        Quaternion _rotation = _packet.ReadQuaternion();
        Quaternion _camera = _packet.ReadQuaternion();

        bool _moving = _packet.ReadBool();

        Server.clients[_fromClient].player.SetInput(horizontal, vertical, _rotation, _camera, jump, _moving);
    }

    public static void Interact(int _fromClient, Packet _packet) 
    {
        if (Server.clients[_fromClient].player.currentItem != null)
        {
            TerrainGenerator.instance.UpdateChunk(Server.clients[_fromClient].player.currentItem.transform.position, null);

            Server.clients[_fromClient].player.inventory.Add(Server.clients[_fromClient].player.currentItem.name);
            Object.Destroy(Server.clients[_fromClient].player.currentItem);
            Server.clients[_fromClient].player.interacting = false;
            Server.clients[_fromClient].player.currentItem = null;

            ServerSend.UpdateInventory(Server.clients[_fromClient].player);
        }
    }

    public static void ModifyVital(int _fromClient, Packet _packet)
    {
        Server.clients[_fromClient].player.health += _packet.ReadFloat();
        Server.clients[_fromClient].player.hunger += _packet.ReadFloat();
    }

    public static void DestroyProp(int _fromClient, Packet _packet)
    {
        TerrainGenerator.instance.UpdateChunk(_packet.ReadVector3(), null);
    }

    public static void AddProp(int _fromClient, Packet _packet)
    {
        Vector3 pos = _packet.ReadVector3();
        PropDataStruct propData = new PropDataStruct
        {
            rot = _packet.ReadVector3(),
            group = _packet.ReadInt(),
            prop = _packet.ReadInt()
        };

        TerrainGenerator.instance.UpdateChunk(pos, propData);
    }

    public static void AddStructure(int _fromClient, Packet _packet)
    {
        Vector3 pos = _packet.ReadVector3();

        StructureDataStruct propData = new StructureDataStruct
        {
            structure = _packet.ReadString(),
            rot = _packet.ReadVector3()
        };

        TerrainGenerator.instance.StructureUpdateChunk(pos, propData);

        if (Server.clients[_fromClient].player.inventory.Contains(propData.structure))
        {
            Server.clients[_fromClient].player.inventory.Remove(propData.structure);
            ServerSend.UpdateInventory(Server.clients[_fromClient].player);
        }
    }
    public static void Craft(int _fromClient, Packet _packet)
    {
        List<string> output = new List<string>();
        List<string> tools = new List<string>();
        List<string> input = new List<string>();

        int outputCount = _packet.ReadInt();
        for (int i = 0; i < outputCount; i++) { output.Add(_packet.ReadString()); }
        int toolsCount = _packet.ReadInt();
        for (int i = 0; i < toolsCount; i++) { tools.Add(_packet.ReadString()); }
        int inputCount = _packet.ReadInt();
        for (int i = 0; i < inputCount; i++) { input.Add(_packet.ReadString()); }

        for (int i = 0; i < input.Count; i++) { Server.clients[_fromClient].player.inventory.Remove(input[i]); }
        for (int i = 0; i < output.Count; i++) { Server.clients[_fromClient].player.inventory.Add(output[i]); }

        ServerSend.UpdateInventory(Server.clients[_fromClient].player);
    }

    public static void EndStorage(int _fromClient, Packet _packet)
    {
        int inventoryCount = _packet.ReadInt();
        int storageCount = _packet.ReadInt();
        Vector3 storagePos = _packet.ReadVector3();

        NetworkManager.instance.storageData[storagePos].Clear();
        Server.clients[_fromClient].player.inventory.Clear();

        for (int i = 0; i < inventoryCount; i++) { Server.clients[_fromClient].player.inventory.Add(_packet.ReadString()); }
        for (int i = 0; i < storageCount; i++) { NetworkManager.instance.storageData[storagePos].Add(_packet.ReadString()); }

        ServerSend.UpdateInventory(Server.clients[_fromClient].player);
    }

    public static void RemoveFromInventory(int _fromClient, Packet _packet)
    {
        int itemCount = _packet.ReadInt();
        for (int i = 0; i < itemCount; i++) { Server.clients[_fromClient].player.inventory.Remove(_packet.ReadString()); }
        ServerSend.UpdateInventory(Server.clients[_fromClient].player);
    }
}
