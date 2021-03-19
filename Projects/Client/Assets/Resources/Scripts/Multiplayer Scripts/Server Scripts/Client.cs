using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;
using UnityEngine.SceneManagement;

public class Client : MonoBehaviour
{
    #region Singleton

    public static Client instance;

    private void Awake()
    {
        instance = this;
    }

    #endregion

    public static int dataBufferSize = 4096;

    public int myId = 0;
    public TCP tcp;
    public UDP udp;

    private bool isConnected = false;
    private delegate void PacketHandler(Packet _packet);
    private static Dictionary<int, PacketHandler> packetHandlers;

    void Start()
    {
        tcp = new TCP(); 
        udp = new UDP();

        InitializeClientData();

        isConnected = true;
        tcp.Connect();
    }

    void OnApplicationQuit() { Disconnect(); }

    public class TCP
    {
        public TcpClient socket;

        NetworkStream stream;
        Packet receivedData;
        byte[] receiveBuffer;

        public void Connect()
        {
            socket = new TcpClient
            {
                ReceiveBufferSize = dataBufferSize,
                SendBufferSize = dataBufferSize
            };

            receiveBuffer = new byte[dataBufferSize];
            socket.BeginConnect(SavingManager.ip, SavingManager.port, ConnectCallback, socket);
        }

        void ConnectCallback(IAsyncResult _result)
        {
            socket.EndConnect(_result);

            if (!socket.Connected)
                return;

            stream = socket.GetStream();
            stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);

            receivedData = new Packet();
        }

        public void SendData(Packet _packet)
        {
            try
            {
                if (socket != null)
                    stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null);
            }
            catch { MultiplayerInputManager.instance.errorParent.SetActive(true); }
        }

        void ReceiveCallback(IAsyncResult _result)
        {
            try
            {
                int _byteLength = stream.EndRead(_result);
                if (_byteLength <= 0) { instance.Disconnect(); return; }

                byte[] _data = new byte[_byteLength];
                Array.Copy(receiveBuffer, _data, _byteLength);

                receivedData.Reset(HandleData(_data));
                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
            }
            catch
            {
                Disconnect();
            }
        }

        private bool HandleData(byte[] _data)
        {
            int _packetLength = 0;
            receivedData.SetBytes(_data);

            if (receivedData.UnreadLength() >= 4)
            {
                _packetLength = receivedData.ReadInt();
                if (_packetLength <= 0) { return true; }
            }

            while (_packetLength > 0 && _packetLength <= receivedData.UnreadLength())
            {
                byte[] _packetBytes = receivedData.ReadBytes(_packetLength);
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet _packet = new Packet(_packetBytes))
                    {
                        int _packetId = _packet.ReadInt();
                        packetHandlers[_packetId](_packet);
                    }
                });

                _packetLength = 0;
                if (receivedData.UnreadLength() >= 4)
                {
                    _packetLength = receivedData.ReadInt();
                    if (_packetLength <= 0) { return true; }
                }
            }

            if (_packetLength <= 1) { return true; }
            return false;
        }

        void Disconnect()
        {
            instance.Disconnect();

            stream = null;
            receivedData = null;
            receiveBuffer = null;
            socket = null;
        }
    }

    public class UDP
    {
        public UdpClient socket;
        public IPEndPoint endPoint;

        public UDP() { endPoint = new IPEndPoint(IPAddress.Parse(SavingManager.ip), SavingManager.port); }

        public void Connect(int _localPort)
        {
            socket = new UdpClient(_localPort);

            socket.Connect(endPoint);
            socket.BeginReceive(ReceiveCallback, null);

            using (Packet _packet = new Packet())
            {
                SendData(_packet);
            };
        }

        //Sends data to the client via UDP
        public void SendData(Packet _packet)
        {
            try
            {
                _packet.InsertInt(instance.myId);

                if (socket != null) { socket.BeginSend(_packet.ToArray(), _packet.Length(), null, null); }
            }
            catch { }
        }

        private void ReceiveCallback(IAsyncResult _result)
        {
            try
            {
                byte[] _data = socket.EndReceive(_result, ref endPoint);
                socket.BeginReceive(ReceiveCallback, null);

                if (_data.Length < 4) { instance.Disconnect(); return; }

                HandleData(_data);
            }
            catch { Disconnect(); }
        }

        private void HandleData(byte[] _data)
        {
            using (Packet _packet = new Packet(_data))
            {
                int _packetLength = _packet.ReadInt();
                _data = _packet.ReadBytes(_packetLength);
            }

            ThreadManager.ExecuteOnMainThread(() =>
            {
                using (Packet _packet = new Packet(_data))
                {
                    int _packetId = _packet.ReadInt();
                    packetHandlers[_packetId](_packet);
                }
            });
        }

        private void Disconnect()
        {
            instance.Disconnect();

            endPoint = null;
            socket = null;
        }
    }

    private void InitializeClientData()
    {
        packetHandlers = new Dictionary<int, PacketHandler>()
        {
            { (int)ServerPackets.welcome, ClientHandle.Welcome },
            { (int)ServerPackets.spawnPlayer, ClientHandle.SpawnPlayer },
            { (int)ServerPackets.playerPosition, ClientHandle.PlayerPosition },
            { (int)ServerPackets.playerRotation, ClientHandle.PlayerRotation },
            { (int)ServerPackets.playerDisconnected, ClientHandle.PlayerDisconnected },
            { (int)ServerPackets.chunkData, ClientHandle.ChunkData },
            { (int)ServerPackets.propChunkUpdate, ClientHandle.PropChunkUpdate },
            { (int)ServerPackets.interact, Inventory.StartInteract },
            { (int)ServerPackets.stopInteract, Inventory.StopInteract },
            { (int)ServerPackets.updateInventory, Inventory.UpdateInventory },
            { (int)ServerPackets.updateVitals, ClientHandle.UpdateVitals },
            { (int)ServerPackets.treeInteract, Inventory.StartTreeInteract },
            { (int)ServerPackets.structuresChunkUpdate, ClientHandle.StructureChunkUpdate },
            { (int)ServerPackets.storageInteract, Inventory.StartStorageInteract }
        };
    }

    private void Disconnect()
    {
        ThreadManager.ExecuteOnMainThread(() =>
        {
            Destroy(SavingManager.instance.gameObject);
            TweeningLibrary.FadeIn(MultiplayerInputManager.instance.errorParent, 1f);
        });

        if (isConnected)
        {
            isConnected = false;
            tcp.socket.Close();
            udp.socket.Close();
        }
    }
}