using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class ClientHandle : MonoBehaviour
{
    // Called once client joins server
    public static void Welcome(Packet _packet)
    {
        // Read all necessary data
        string _msg = _packet.ReadString();
        int _myId = _packet.ReadInt();
        int _seed = _packet.ReadInt();

        // Reset terrain
        TerrainGenerator.instance.ResetChunks();

        // Log server msg, set our id & call a ClientSend func
        Debug.Log($"Message from server: {_msg}");
        Client.instance.myId = _myId;
        ClientSend.WelcomeReceived();

        // Toggle the UI & set the terrain seed
        InputManager.instance.ToggleUISectionsInt(0);
        TerrainGenerator.instance.heightMapSettings.noiseSettings.seed = _seed;

        // Connect the UDP sock
        Client.instance.udp.Connect(((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port);
    }

    // Called to spawn a player (both local & non-local)
    public static void SpawnPlayer(Packet _packet)
    {
        // Get all basic vars about the player
        int _id = _packet.ReadInt();
        string _username = _packet.ReadString();
        Vector3 _position = _packet.ReadVector3();
        Quaternion _rotation = _packet.ReadQuaternion();

        // Call the GameManager to spawn the player
        GameManager.instance.SpawnPlayer(_id, _username, _position, _rotation);
    }

    // Called to set the position of a player
    public static void PlayerPosition(Packet _packet)
    {
        // Read the id & position from the packet
        int _id = _packet.ReadInt();
        Vector3 _position = _packet.ReadVector3();

        // Set the position thru the GameManager
        GameManager.players[_id].transform.position = _position;
    }

    // Sets the rotation of a player
    public static void PlayerRotation(Packet _packet)
    {
        // Read data
        int _id = _packet.ReadInt();
        Quaternion _rotation = _packet.ReadQuaternion();

        // Set data
        GameManager.players[_id].transform.rotation = _rotation;
    }

    // Tells the client a player has disconnected
    public static void PlayerDisconnected(Packet _packet)
    {
        // Get the id of the dipped player
        int _id = _packet.ReadInt();

        // Update the client
        Destroy(GameManager.players[_id].gameObject);
        GameManager.players.Remove(_id);
    }

    // Called to spawn a chunk based in server info
    public static void ChunkData(Packet _packet)
    {
        // Call the TerrainGenerator
        TerrainGenerator.instance.SpawnChunk(_packet.ReadVector2(), _packet.ReadStringVector3Dict());
    }
}
