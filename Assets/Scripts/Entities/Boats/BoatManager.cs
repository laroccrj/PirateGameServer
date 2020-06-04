using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoatManager : MonoBehaviour
{
    public static BoatManager instance;
    public static Dictionary<int, Boat> Boats = new Dictionary<int, Boat>();

    public Boat boatPrefab;

    private static int nextId = 1;

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
        this.SendBoatTransformUpdate();
    }

    public static int getNextId()
    {
        return nextId++;
    }

    public Boat SpawnBoat(Team team, Vector3 position, Quaternion rotation)
    {
        Boat boat = GameObject.Instantiate<Boat>(boatPrefab, position, rotation);
        boat.Init(team);
        Boats.Add(boat.id, boat);
        return boat;
    }

    public static void SendBoatsToPlayers()
    {
        using (Packet packet = new Packet((int)ServerPackets.sendBoats))
        {
            int boatCount = Boats.Count;

            if (boatCount > 0)
            {
                packet.Write(boatCount);

                foreach (Boat boat in Boats.Values)
                {
                    packet.Write(boat.id);
                    packet.Write(boat.transform.position);
                    packet.Write(boat.transform.rotation);

                    Mountable[] mountables = boat.mountables.Values.ToArray<Mountable>();
                    packet.Write(mountables.Length);

                    foreach (Mountable mountable in mountables)
                    {
                        packet.Write(mountable.id);
                        packet.Write((int)mountable.MountableType);
                        packet.Write(mountable.reverseRotation);
                        packet.Write(mountable.transform.localPosition);
                        packet.Write(mountable.transform.localRotation);
                    }

                    Wall[] walls = boat.walls.Values.ToArray<Wall>();
                    packet.Write(walls.Length);

                    foreach (Wall wall in walls)
                    {
                        packet.Write(wall.id);
                        packet.Write(wall.maxHealth);
                        packet.Write(wall.health);
                        packet.Write(wall.transform.localPosition);
                        packet.Write(wall.transform.localRotation);
                        packet.Write(wall.transform.localScale);
                    }
                }

                ServerSend.SendTCPDataToAll(packet);
            }
        }
    }

    void SendBoatTransformUpdate()
    {
        using (Packet packet = new Packet((int)ServerPackets.boatTransformUpdate))
        {
            int boatCount = BoatManager.Boats.Count;
            packet.Write(boatCount);

            if (boatCount > 0)
            {
                foreach (Boat boat in BoatManager.Boats.Values)
                {
                    packet.Write(boat.id);
                    packet.Write(boat.transform.position);
                    packet.Write(boat.transform.rotation);

                    Mountable[] mountables = boat.mountables.Values.ToArray<Mountable>();
                    packet.Write(mountables.Length);

                    foreach (Mountable mountable in mountables)
                    {
                        packet.Write(mountable.id);
                        packet.Write(mountable.body.transform.localPosition);
                        packet.Write(mountable.body.transform.localRotation);
                    }
                }

                ServerSend.SendUDPDataToAll(packet);
            }
        }
    }
}
