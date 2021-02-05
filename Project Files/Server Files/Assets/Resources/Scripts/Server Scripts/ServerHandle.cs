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
        Debug.Log(_packet.ReadFloat(false));
        Server.clients[_fromClient].player.health += _packet.ReadFloat();
        Server.clients[_fromClient].player.hunger += _packet.ReadFloat();
    }

    public static void DestroyProp(int _fromClient, Packet _packet)
    {
        TerrainGenerator.instance.UpdateChunk(_packet.ReadVector3(), null);
    }

    public static void AddProp(int _fromClient, Packet _packet)
    {
        Debug.Log("AAAAAAAAAAAAAAAAADDDDDD");
        Vector3 pos = _packet.ReadVector3();

        PropDataStruct propData = new PropDataStruct
        {
            rot = _packet.ReadVector3(),
            group = _packet.ReadInt(),
            prop = _packet.ReadInt()
        };

        TerrainGenerator.instance.UpdateChunk(pos, propData);
    }
}
