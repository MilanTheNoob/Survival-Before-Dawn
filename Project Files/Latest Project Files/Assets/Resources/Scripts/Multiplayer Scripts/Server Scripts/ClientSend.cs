using UnityEngine;

public class ClientSend : MonoBehaviour
{
    public static void WelcomeReceived()
    {
        using (Packet _packet = new Packet((int)ClientPackets.welcomeReceived))
        {
            _packet.Write(Client.instance.myId);
            _packet.Write("null");

            SendTCPData(_packet);
        }
    }

    public static void SendPlayerMovement(float horizontal, float vertical, bool jump)
    {
        using (Packet _packet = new Packet((int)ClientPackets.playerMovement))
        {
            _packet.Write(horizontal);
            _packet.Write(vertical);
            _packet.Write(jump);

            _packet.Write(PlayerMovement.instance.transform.rotation);
            _packet.Write(MouseLook.instance.transform.rotation);

            _packet.Write(InputManager.moving);

            SendUDPData(_packet);
        }
    }

    public static void UpdateVital(float healthChange, float hungerChange)
    {
        using (Packet _packet = new Packet((int)ClientPackets.modifyVital))
        {
            _packet.Write(healthChange);
            _packet.Write(hungerChange);

            SendTCPData(_packet);
        }
    }

    #region Send Data Funcs

    public static void SendTCPData(Packet _packet) { _packet.WriteLength(); Client.instance.tcp.SendData(_packet); }
    public static void SendUDPData(Packet _packet) { _packet.WriteLength(); Client.instance.udp.SendData(_packet); }

    #endregion
}
