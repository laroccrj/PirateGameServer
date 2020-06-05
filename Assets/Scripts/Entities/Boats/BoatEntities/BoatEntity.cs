using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BoatEntityType
{
    WALL = 1,
    CANNON,
    STEERING_WHEEL,
}

public abstract class BoatEntity : MonoBehaviour
{
    public int id;
    public Boat boat;

    public abstract BoatEntityType BoatEntityType
    {
        get;
    }

    public abstract int GetNextId();

    public abstract void WriteDataToPacket(ref Packet packet);
}
