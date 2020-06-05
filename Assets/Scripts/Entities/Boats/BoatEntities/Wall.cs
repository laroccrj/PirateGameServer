using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : BoatEntity, Damagable, Interactable
{
    private static int nextId = 1;

    public float maxHealth = 100;
    public float health = 100;
    public Transform repairSpot;
    public bool repairing = false;
    public float repairRate = 0.5f;

    public override BoatEntityType BoatEntityType => BoatEntityType.WALL;

    void Start()
    {
        this.boat = GetComponentInParent<Boat>();

        if (this.repairSpot == null)
            this.repairSpot = this.transform;
    }

    public void Update()
    {
        if (repairing)
            this.health += repairRate;

        if (this.health > this.maxHealth)
            this.health = this.maxHealth;
    }

    public void ApplyDamage(float damage)
    {
        this.health -= damage;

        if (this.health <= 0)
            this.GetComponent<Collider2D>().enabled = false;
    }

    public Vector3 GetInteractionPoint()
    {
        return this.repairSpot.position;
    }

    public void Interact(Pirate pirate, InteractionType interactionType)
    {
        this.repairing = true;
    }

    public void Leave()
    {
        this.repairing = false;
    }

    public Transform GetSeat()
    {
        return this.repairSpot.transform;
    }

    public BoatEntityType GetBoatEntityType()
    {
        throw new System.NotImplementedException();
    }

    public override int GetNextId()
    {
        return nextId++;
    }

    public override void WriteDataToPacket(ref Packet packet)
    {
        packet.Write(maxHealth);
        packet.Write(health);
    }
}
