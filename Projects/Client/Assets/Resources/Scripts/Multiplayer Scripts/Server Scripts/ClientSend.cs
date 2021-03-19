using UnityEngine;

public class ClientSend : MonoBehaviour
{
    public static void WelcomeReceived()
    {
        using (Packet _packet = new Packet((int)ClientPackets.welcomeReceived))
        {
            _packet.Write(Client.instance.myId);
            _packet.Write(GPlayManager.PlayerName);
            _packet.Write(GPlayManager.PlayerID);

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

    public static void AddStructure(GameObject structure)
    {
        using (Packet _packet = new Packet((int)ClientPackets.addStructure))
        {
            _packet.Write(structure.transform.position);
            _packet.Write(structure.name);
            _packet.Write(structure.transform.eulerAngles);

            SendTCPData(_packet);
        }
    }

    public static void Craft(CraftingVariant recipe)
    {
        using (Packet _packet = new Packet((int)ClientPackets.craft))
        {
            _packet.Write(recipe.Output.Length);
            for (int i = 0; i < recipe.Output.Length; i++) { _packet.Write(recipe.Output[i].name); }
            _packet.Write(recipe.Tools.Length);
            for (int i = 0; i < recipe.Tools.Length; i++) { _packet.Write(recipe.Tools[i].name); }
            _packet.Write(recipe.Input.Length);
            for (int i = 0; i < recipe.Input.Length; i++) { _packet.Write(recipe.Input[i].name); }

            SendTCPData(_packet);
        }
    }

    #region Send Data Funcs

    public static void SendTCPData(Packet _packet) { _packet.WriteLength(); Client.instance.tcp.SendData(_packet); }
    public static void SendUDPData(Packet _packet) { _packet.WriteLength(); Client.instance.udp.SendData(_packet); }

    #endregion
}
