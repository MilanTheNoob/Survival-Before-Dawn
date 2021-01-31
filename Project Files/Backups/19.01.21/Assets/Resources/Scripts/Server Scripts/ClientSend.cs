using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSend : MonoBehaviour
{
    // The TerrainGenerator instance
    static TerrainGenerator terrainGenerator;

    // Called at the beginning of the game
    private void Start() { terrainGenerator = TerrainGenerator.instance; }

    // Called to send TCP data to the server
    private static void SendTCPData(Packet _packet)
    {
        // Write the length of the packer
        _packet.WriteLength();
        // Send the data
        Client.instance.tcp.SendData(_packet);
    }

    // Sends data thru UDP (better for large amounts of non-critical data)
    private static void SendUDPData(Packet _packet)
    {
        // Write the lengtth
        _packet.WriteLength();
        // Send the data
        Client.instance.udp.SendData(_packet);
    }

    #region Packets

    // Called to tell the server we joined successfully 
    public static void WelcomeReceived()
    {
        // Create a new packet
        using(Packet _packet = new Packet((int)ClientPackets.welcomeReceived))
        {
            // Write our player id
            _packet.Write(Client.instance.myId);
            // Write our username
            _packet.Write(UIManager.instance.usernameField.text);

            // Send the data thru TCP
            SendTCPData(_packet);
        }
    }

    public static void PlayerMovement(float horizontal, float vertical, bool jump)
    {
        // Create a new packet
        using(Packet _packet = new Packet((int)ClientPackets.playerMovement))
        {
            // Write our input data
            _packet.Write(horizontal);
            _packet.Write(vertical);
            _packet.Write(jump);

            // Write our rotational data
            _packet.Write(TerrainGenerator.instance.viewer.rotation);

            // Send data thru UDP
            SendUDPData(_packet);
        }
    }

    #endregion
}
