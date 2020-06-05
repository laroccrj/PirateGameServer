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
        Packet packet = new Packet((int)ServerPackets.sendBoats);

        packet.Write(Boats.Count);

        foreach (Boat boat in Boats.Values)
        {
            packet.Write(boat.id);
            packet.Write(boat.transform.position);
            packet.Write(boat.transform.rotation);
            packet.Write(boat.boatEntitiesByType.Count);

            foreach(KeyValuePair<BoatEntityType, Dictionary<int, BoatEntity>> entitiesOfType in boat.boatEntitiesByType)
            {
                packet.Write((int)entitiesOfType.Key);
                packet.Write(entitiesOfType.Value.Count);

                foreach (BoatEntity entity in entitiesOfType.Value.Values)
                {
                    packet.Write(entity.id);
                    packet.Write(entity.transform.localPosition);
                    packet.Write(entity.transform.localRotation);
                    packet.Write(entity.transform.localScale);

                    entity.WriteDataToPacket(ref packet);
                }
            }
        }

        ServerSend.SendTCPDataToAll(packet);
        packet.Dispose();
    }

    void SendBoatTransformUpdate()
    {
        foreach (Boat boat in Boats.Values)
        {
            Packet packet = new Packet((int)ServerPackets.boatTransformUpdate);

            packet.Write(boat.id);
            packet.Write(boat.transform.position);
            packet.Write(boat.transform.rotation);
            packet.Write(boat.boatEntitiesByType.Count);

            foreach (KeyValuePair<BoatEntityType, Dictionary<int, BoatEntity>> entitiesOfType in boat.boatEntitiesByType)
            {
                packet.Write((int)entitiesOfType.Key);
                packet.Write(entitiesOfType.Value.Count);

                foreach (BoatEntity entity in entitiesOfType.Value.Values)
                {
                    packet.Write(entity.id);
                    packet.Write(entity.transform.localPosition);
                    packet.Write(entity.transform.localRotation);
                    packet.Write(entity.transform.localScale);
                    entity.WriteDataToPacket(ref packet);
                }
            }

            ServerSend.SendUDPDataToAll(packet);
            packet.Dispose();
        }
    }
}
