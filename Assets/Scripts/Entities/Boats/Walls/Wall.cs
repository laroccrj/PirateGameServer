using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour, Damagable
{
    public int id;
    public float maxHealth = 100;
    public float health = 100;
    public Boat boat;
    public Transform repairSpot;
    public Pirate repairer = null;
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

        if (repairer != null && Vector3.Distance(repairer.transform.position, this.repairSpot.position) < .1)
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

    public void OnRepairStart(Pirate pirate)
    {
        this.repairer = pirate;
    }

    public void OnRepairEnd()
    {
        this.repairer = null;
    }
}
