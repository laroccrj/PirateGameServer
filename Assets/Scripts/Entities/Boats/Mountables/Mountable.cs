using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Mountable : MonoBehaviour, Interactable
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
    public Pirate pirate = null;

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

    protected abstract void OnMount();
    protected abstract void OnDismount();

    public Vector3 GetInteractionPoint()
    {
        return this.transform.position;
    }

    public void Interact(Pirate pirate, InteractionType interactionType)
    {
        this.pirate = pirate;
        this.mounted = true;
        this.OnMount();

        using (Packet packet = new Packet((int)ServerPackets.mounted))
        {
            packet.Write(this.id);
            ServerSend.SendTCPData(this.pirate.id, packet);
        }
    }

    public void Leave()
    {
        this.mounted = false;
        this.OnDismount();

        using (Packet packet = new Packet((int)ServerPackets.dismounted))
        {
            packet.Write(this.id);
            ServerSend.SendTCPData(this.pirate.id, packet);
        }

        this.pirate = null;
    }

    public Transform GetSeat()
    {
        return this.seat.transform;
    }

    public InteractableType GetInteractableType()
    {
        return InteractableType.MOOUNTABLE;
    }
}
