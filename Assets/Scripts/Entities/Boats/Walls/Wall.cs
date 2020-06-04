using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour, Damagable, Interactable
{
    public int id;
    public float maxHealth = 100;
    public float health = 100;
    public Boat boat;
    public Transform repairSpot;
    public bool repairing = false;
    public float repairRate = 0.5f;

    void Start()
    {
        this.boat = GetComponentInParent<Boat>();

        if (this.repairSpot == null)
            this.repairSpot = this.transform;
    }

    public void Update()
    {
        float currentHealth = this.health;

        if (repairing)
            this.health += repairRate;

        if (this.health > this.maxHealth)
            this.health = this.maxHealth;

        if (currentHealth != this.health)
            WallManager.UpdateWallHealth(this);
    }

    public void ApplyDamage(float damage)
    {
        this.health -= damage;

        if (this.health <= 0)
            this.GetComponent<Collider2D>().enabled = false;

        WallManager.UpdateWallHealth(this);
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

    public InteractableType GetInteractableType()
    {
        return InteractableType.WALL;
    }
}
