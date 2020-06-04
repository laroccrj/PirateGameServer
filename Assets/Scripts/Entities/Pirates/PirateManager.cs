using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PirateManager : MonoBehaviour
{
    public static PirateManager instance;
    public Dictionary<int, Pirate> Pirates = new Dictionary<int, Pirate>();

    public Pirate piratePrefab;

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

    private void FixedUpdate()
    {
        PirateManager.SendPiratePositions();
    }

    public Pirate SpawnPirate(Player player, Boat boat)
    {
        Pirate pirate = Instantiate<Pirate>(instance.piratePrefab, boat.transform);

        pirate.id = player.id;
        pirate.boat = boat;

        this.Pirates.Add(pirate.id, pirate);
        return pirate;
    }


    public Pirate DeSpawnPirate(Player player)
    {
        Pirate pirate = this.Pirates[player.id];
        this.Pirates.Remove(pirate.id);
        Destroy(pirate.gameObject);
        return pirate;
    }

    public void SendPirates()
    {
        using (Packet packet = new Packet((int)ServerPackets.pirateSpawn))
        {
            int pirateCount = this.Pirates.Count;
            packet.Write(pirateCount);

            foreach (Pirate pirate in this.Pirates.Values)
            {
                packet.Write(pirate.id);
                packet.Write(pirate.boat.id);
                packet.Write(pirate.transform.localPosition);
                packet.Write(pirate.transform.localRotation);
            }
            
            ServerSend.SendTCPDataToAll(packet);
        }
    }



    public static void SendPiratePositions()
    {
        using (Packet packet = new Packet((int)ServerPackets.pirateMove))
        {
            int count = instance.Pirates.Count;

            if (count > 0)
            {
                packet.Write(count);

                foreach (Pirate pirate in instance.Pirates.Values)
                {
                    packet.Write(pirate.id);
                    packet.Write(pirate.transform.localPosition);
                    packet.Write(pirate.transform.localRotation);
                }

                ServerSend.SendUDPDataToAll(packet);
            }
        }
    }

    public static void MovePirate(int fromClient, Packet packet)
    {
        int clientId = packet.ReadInt();

        if (fromClient != clientId)
        {
            Debug.Log($"Player (ID: {fromClient}) has assumed the wrong client ID ({clientId})!");
            return;
        }

        Vector3 pos = packet.ReadVector3();
        Pirate pirate = PirateManager.instance.Pirates[fromClient];
        pirate.destination = pos;
        pirate.mounter.dismount();

        if (pirate.repairing != null)
        {
            pirate.repairing.repairer = null;
            pirate.repairing = null;
        }
    }

    public static void MountRequest(int fromClient, Packet packet)
    {
        int clientId = packet.ReadInt();

        if (fromClient != clientId)
        {
            Debug.Log($"Player (ID: {fromClient}) has assumed the wrong client ID ({clientId})!");
            return;
        }

        int mountableId = packet.ReadInt();
        Pirate pirate = PirateManager.instance.Pirates[fromClient];
        Mountable mountable;

        if (pirate.boat.mountables.TryGetValue(mountableId, out mountable))
        {
            pirate.mounter.dismount();
            pirate.destination = mountable.transform.localPosition;
            pirate.mounter.mount(mountable);
        }
    }

    public static void ReapairWallRequest(int fromClient, Packet packet)
    {
        int clientId = packet.ReadInt();

        if (fromClient != clientId)
        {
            Debug.Log($"Player (ID: {fromClient}) has assumed the wrong client ID ({clientId})!");
            return;
        }

        int wallId = packet.ReadInt();
        Pirate pirate = PirateManager.instance.Pirates[fromClient];
        Wall wall;

        if (pirate.boat.walls.TryGetValue(wallId, out wall))
        {
            pirate.destination = wall.transform.localPosition;
            wall.repairer = pirate;
            pirate.repairing = wall;
        }
    }
}
