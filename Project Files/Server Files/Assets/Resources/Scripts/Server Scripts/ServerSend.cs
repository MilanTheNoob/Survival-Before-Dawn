using System.Linq;
using UnityEngine;

public class ServerSend : MonoBehaviour
{
    private static void SendTCPData(int _toClient, Packet _packet)
    {
        _packet.WriteLength();
        Server.clients[_toClient].tcp.SendData(_packet);
    }

    private static void SendUDPData(int _toClient, Packet _packet)
    {
        _packet.WriteLength();
        Server.clients[_toClient].udp.SendData(_packet);
    }

    private static void SendTCPDataToAll(Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            Server.clients[i].tcp.SendData(_packet);
        }
    }
    private static void SendTCPDataToAll(int _exceptClient, Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            if (i != _exceptClient)
            {
                Server.clients[i].tcp.SendData(_packet);
            }
        }
    }

    private static void SendUDPDataToAll(Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            Server.clients[i].udp.SendData(_packet);
        }
    }
    private static void SendUDPDataToAll(int _exceptClient, Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            if (i != _exceptClient)
            {
                Server.clients[i].udp.SendData(_packet);
            }
        }
    }

    #region Packets
    public static void Welcome(int _toClient)
    {
        using (Packet _packet = new Packet((int)ServerPackets.welcome))
        {
            _packet.Write(_toClient);
            SendTCPData(_toClient, _packet);
        }
    }

    public static void SpawnPlayer(int _toClient, Player _player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.spawnPlayer))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.username);
            _packet.Write(_player.transform.position);
            _packet.Write(_player.transform.rotation);

            SendTCPData(_toClient, _packet);
        }
    }

    public static void PlayerPosition(Player _player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerPosition))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.transform.position);

            SendUDPDataToAll(_packet);
        }
    }

    public static void PlayerRotation(Player _player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerRotation))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.transform.rotation);

            SendUDPDataToAll(_player.id, _packet);
        }
    }

    public static void PlayerDisconnected(int _playerId)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerDisconnected))
        {
            _packet.Write(_playerId);

            SendTCPDataToAll(_packet);
        }
    }

    public static void ChunkData(ChunkDataStruct _chunkData, Player _player, int width, int height)
    {
        using (Packet _packet = new Packet((int)ServerPackets.chunkData))
        {
            _packet.Write(_chunkData.coord);
            _packet.Write(_chunkData.heightMap, width, height);

            _packet.Write(_chunkData.props.Count);

            for (int i = 0; i < _chunkData.props.Count; i++)
            {
                _packet.Write(_chunkData.props.ElementAt(i).Value.group);
                _packet.Write(_chunkData.props.ElementAt(i).Value.prop);

                _packet.Write(_chunkData.props.ElementAt(i).Value.rot);
                _packet.Write(_chunkData.props.ElementAt(i).Key);
            }

            SendUDPData(_player.id, _packet);
        }
    }

    public static void PropChunkUpdate(ChunkDataStruct _chunkData, Player _player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.propChunkUpdate))
        {
            _packet.Write(_chunkData.coord);
            _packet.Write(_chunkData.props.Count);

            for (int i = 0; i < _chunkData.props.Count; i++)
            {
                _packet.Write(_chunkData.props.ElementAt(i).Value.group);
                _packet.Write(_chunkData.props.ElementAt(i).Value.prop);

                _packet.Write(_chunkData.props.ElementAt(i).Value.rot);
                _packet.Write(_chunkData.props.ElementAt(i).Key);
            }

            SendUDPData(_player.id, _packet);
        }
    }

    public static void InteractItem(string _interactTxt, string _item, Player _player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.interact))
        {
            _packet.Write(_interactTxt);
            _packet.Write(_item);

            SendTCPData(_player.id, _packet);
        }
    }
    public static void StopInteractItem(Player _player) { SendTCPData(_player.id, new Packet((int)ServerPackets.stopInteract)); }

    public static void UpdateInventory(Player _player)
    {
        Debug.Log("update inventory");
        using (Packet _packet = new Packet((int)ServerPackets.updateInventory))
        {
            _packet.Write(_player.inventory.Count);
            for (int i = 0; i < _player.inventory.Count; i++) { _packet.Write(_player.inventory[i]); }

            SendTCPData(_player.id, _packet);
        }
    }

    #endregion
}
