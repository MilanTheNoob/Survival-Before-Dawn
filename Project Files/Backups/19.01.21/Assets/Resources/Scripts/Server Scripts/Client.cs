using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;

public class Client : MonoBehaviour
{
    // The Singleton of this script
    #region Singleton

    public static Client instance;

    private void Awake()
    {
        instance = this;
    }

    #endregion

    public static int dataBufferSize = 4096;

    public string ip = "127.0.0.1";
    public int port = 26950;
    public int myId = 0;
    public TCP tcp;
    public UDP udp;

    private bool isConnected = false;
    private delegate void PacketHandler(Packet _packet);
    private static Dictionary<int, PacketHandler> packetHandlers;

    // Called at the beginning of the game
    private void Start()
    {
        // Store a new TCP & UDP instance
        tcp = new TCP();
        udp = new UDP();
    }

    // Disconnect when the game is quit
    private void OnApplicationQuit() { Disconnect(); }

    // Called to connect to a server
    public void ConnectToServer()
    {
        // Call the init client data
        InitializeClientData();

        // isConnected to true
        isConnected = true;
        // Connect to tcp
        tcp.Connect();
    }

    // TCP class
    public class TCP
    {
        // TCP Client socket
        public TcpClient socket;

        // Sum vars
        private NetworkStream stream;
        private Packet receivedData;
        private byte[] receiveBuffer;

        // Called to connect
        public void Connect()
        {
            // Init a new socket
            socket = new TcpClient
            {
                ReceiveBufferSize = dataBufferSize,
                SendBufferSize = dataBufferSize
            };

            // Set the recieve buffer and begin connecting
            receiveBuffer = new byte[dataBufferSize];
            socket.BeginConnect(instance.ip, instance.port, ConnectCallback, socket);
        }

        // Inits the newly connected client's TCP-related info
        private void ConnectCallback(IAsyncResult _result)
        {
            // End the socket connection
            socket.EndConnect(_result);

            if (!socket.Connected)
                return;

            // Get the stream from the socket
            stream = socket.GetStream();
            // Start receiving data from the socket
            stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);

            //Store a new empty packet
            receivedData = new Packet();
        }

        // Send data to the client
        public void SendData(Packet _packet)
        {
            try
            {
                // Send data to server if possible
                if (socket != null)
                    stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null);
            }
            catch (Exception _ex)
            {
                // Log the error
                Debug.Log($"Error sending data to server via TCP: {_ex}");
            }
        }

        // Reads incoming data from the stream
        private void ReceiveCallback(IAsyncResult _result)
        {
            try
            {
                // Get the length of the data
                int _byteLength = stream.EndRead(_result);
                if (_byteLength <= 0)
                {
                    // Disconnect & then returb
                    instance.Disconnect();
                    return;
                }

                // Store the data as a byte array
                byte[] _data = new byte[_byteLength];
                // Copy the received data to the array above
                Array.Copy(receiveBuffer, _data, _byteLength);

                // Reset receivedData if all data was handled
                receivedData.Reset(HandleData(_data));
                // Start reading the stream
                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
            }
            catch
            {
                // Disconnect if the above code failed
                Disconnect();
            }
        }

        //Prepares received data to be used by the appropriate packet handler methods
        private bool HandleData(byte[] _data)
        {
            // The length of the packet
            int _packetLength = 0;
            // Set the data 
            receivedData.SetBytes(_data);

            if (receivedData.UnreadLength() >= 4)
            {
                // If client's received data contains a packet
                _packetLength = receivedData.ReadInt();
                if (_packetLength <= 0)
                {
                    // If packet contains no data return true
                    return true;
                }
            }

            while (_packetLength > 0 && _packetLength <= receivedData.UnreadLength())
            {
                // While packet contains data AND packet data length doesn't exceed the length of the packet we're reading
                byte[] _packetBytes = receivedData.ReadBytes(_packetLength);
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet _packet = new Packet(_packetBytes))
                    {
                        // Get the type of packet being received
                        int _packetId = _packet.ReadInt();
                        // Call appropriate method to handle the packet
                        packetHandlers[_packetId](_packet);
                    }
                });

                // Reset packet length
                _packetLength = 0;
                if (receivedData.UnreadLength() >= 4)
                {
                    // If client's received data contains another packet
                    _packetLength = receivedData.ReadInt();
                    if (_packetLength <= 0)
                    {
                        // If packet contains no data
                        return true;
                    }
                }
            }

            if (_packetLength <= 1)
                return true; // Reset receivedData instance to allow it to be reused

            return false;
        }

        // Called to disconnect from the server and clean up the TCP connection
        private void Disconnect()
        {
            // Call the instance's Disconnect func
            instance.Disconnect();

            // Set all vars to null
            stream = null;
            receivedData = null;
            receiveBuffer = null;
            socket = null;
        }
    }

    // The TCP struct
    public class UDP
    {
        // Basic vars
        public UdpClient socket;
        public IPEndPoint endPoint;

        // The init class/func
        public UDP() { endPoint = new IPEndPoint(IPAddress.Parse(instance.ip), instance.port); }

        //Attempts to connect to the server via UDP
        public void Connect(int _localPort)
        {
            // Create a new socket
            socket = new UdpClient(_localPort);

            // Connect and start receiving data
            socket.Connect(endPoint);
            socket.BeginReceive(ReceiveCallback, null);

            using (Packet _packet = new Packet()) { SendData(_packet); }
        }

        //Sends data to the client via UDP
        public void SendData(Packet _packet)
        {
            try
            {
                // Insert the client's ID at the start of the packet
                _packet.InsertInt(instance.myId);

                // Send the packet if possible
                if (socket != null)
                    socket.BeginSend(_packet.ToArray(), _packet.Length(), null, null);
            }
            catch (Exception _ex)
            {
                // Log the exception of the code above
                Debug.Log($"Error sending data to server via UDP: {_ex}");
            }
        }

        // Receives incoming UDP data
        private void ReceiveCallback(IAsyncResult _result)
        {
            try
            {
                // Store all data in the bytes array
                byte[] _data = socket.EndReceive(_result, ref endPoint);
                // Start receiving again
                socket.BeginReceive(ReceiveCallback, null);

                if (_data.Length < 4)
                {
                    // Disconnect and return
                    instance.Disconnect();
                    return;
                }

                // Call the handle func
                HandleData(_data);
            }
            catch
            {
                // Disconnect if the code above fails
                Disconnect();
            }
        }

        // Prepares received data to be used by the appropriate packet handler methods
        private void HandleData(byte[] _data)
        {
            // Create a new packet from the data to read
            using (Packet _packet = new Packet(_data))
            {
                // Get the length of the packet
                int _packetLength = _packet.ReadInt();
                // Get the data based on the length
                _data = _packet.ReadBytes(_packetLength);
            }

            // Handle the data on another thread
            ThreadManager.ExecuteOnMainThread(() =>
            {
                // A new packet once more
                using (Packet _packet = new Packet(_data))
                {
                    // Get the packet length
                    int _packetId = _packet.ReadInt();
                    // Call appropriate method to handle the packet
                    packetHandlers[_packetId](_packet);
                }
            });
        }

        //Disconnects from the server and cleans up the UDP connection
        private void Disconnect()
        {
            // Call the instance's disconnect func
            instance.Disconnect();

            // Set vars to null
            endPoint = null;
            socket = null;
        }
    }

    // Initializes all necessary client data
    private void InitializeClientData()
    {
        // Init a new dict storing all the funcs along with id's
        packetHandlers = new Dictionary<int, PacketHandler>()
        {
            { (int)ServerPackets.welcome, ClientHandle.Welcome },
            { (int)ServerPackets.spawnPlayer, ClientHandle.SpawnPlayer },
            { (int)ServerPackets.playerPosition, ClientHandle.PlayerPosition },
            { (int)ServerPackets.playerRotation, ClientHandle.PlayerRotation },
            { (int)ServerPackets.playerDisconnected, ClientHandle.PlayerDisconnected },
            { (int)ServerPackets.chunkData, ClientHandle.ChunkData }
        };
        // Log
        Debug.Log("Initialized packets.");
    }

    // Disconnects from the server and stops all network traffic
    private void Disconnect()
    {
        // Check if we are connected in the first place
        if (isConnected)
        {
            // Close socks and set vars accordingly
            isConnected = false;
            tcp.socket.Close();
            udp.socket.Close();

            //Log in console
            Debug.Log("Disconnected from server.");
        }
    }
}