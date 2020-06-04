using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallManager : MonoBehaviour
{
    public static WallManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    public static void UpdateWallHealth(Wall wall)
    {
        using (Packet packet = new Packet((int)ServerPackets.updateWallHealth))
        {
            packet.Write(wall.id);
            packet.Write(wall.boat.id);
            packet.Write(wall.maxHealth);
            packet.Write(wall.health);

            ServerSend.SendUDPDataToAll(packet);
        }
    }
}
