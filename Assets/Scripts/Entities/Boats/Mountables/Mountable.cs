using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Mountable : MonoBehaviour
{
    public enum Mountables
    {
        STEERING_WHEEL,
        CANNNON
    }

    public int id;
    public Boat boat;
    public bool mounted = false;
    public Transform seat = null;
    public Transform body = null;
    public Mounter mounter = null;

    // Used when the mountable is facing -y
    public bool reverseRotation = false;

    public abstract Mountables MountableType
    {
        get;
    }

    void Start()
    {
        if (this.seat == null)
            this.seat = this.transform;

        if (this.body == null)
            this.body = this.transform;

        this.boat = this.GetComponentInParent<Boat>();
    }

    public void mount(Mounter mounter)
    {
        this.mounted = true;
        this.mounter = mounter;
        this.OnMount();

        Pirate pirate = mounter.pirate;

        using (Packet packet = new Packet((int)ServerPackets.mounted))
        {
            packet.Write(this.id);
            ServerSend.SendTCPData(pirate.id, packet);
        }
    }

    public void dismount()
    {
        this.mounted = false;
        this.OnDismount();

        Pirate pirate = mounter.pirate;

        using (Packet packet = new Packet((int)ServerPackets.dismounted))
        {
            packet.Write(this.id);
            ServerSend.SendTCPData(pirate.id, packet);
        }

        this.mounter = null;
    }

    protected abstract void OnMount();
    protected abstract void OnDismount();
}
