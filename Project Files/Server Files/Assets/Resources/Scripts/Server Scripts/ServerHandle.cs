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

        Server.clients[_fromClient].player.SetInput(horizontal, vertical, _rotation, jump);
    }

    public static void Interact(int _fromClient, Packet _packet) 
    {
        Debug.Log("tried to interact");
        Server.clients[_fromClient].player.inventory.Add(Server.clients[_fromClient].player.currentItem.name);
        ServerSend.UpdateInventory(Server.clients[_fromClient].player);
    }
}
