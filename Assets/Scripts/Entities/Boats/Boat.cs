using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Boat : MonoBehaviour, Damagable
{
    public int id;
    public Dictionary<BoatEntityType, Dictionary<int, BoatEntity>> boatEntitiesByType;
    public int acceleration = 1;
    public int maxSpeed = 1;
    public int rotateSpeed = 1;
    public Rigidbody2D rigid;
    public float health;
    public Team team;

    // Start is called before the first frame update
    public void Init(Team team)
    {
        this.id = BoatManager.getNextId();
        this.team = team;
        this.LoadBoatEntities();
        this.rigid = this.GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        this.rigid.AddRelativeForce(Vector2.right * acceleration * Time.deltaTime);

        if (rigid.velocity.magnitude > maxSpeed)
            rigid.velocity = rigid.velocity.normalized * maxSpeed;

    }

    private void LoadBoatEntities ()
    {
        this.boatEntitiesByType = new Dictionary<BoatEntityType, Dictionary<int, BoatEntity>>();

        foreach (BoatEntityType boatEntityType in Enum.GetValues(typeof(BoatEntityType)).Cast<BoatEntityType>())
        {
            this.boatEntitiesByType[boatEntityType] = new Dictionary<int, BoatEntity>();
        }

        BoatEntity[] entities = this.GetComponentsInChildren<BoatEntity>();

        foreach (BoatEntity entity in entities)
        {
            entity.id = entity.GetNextId();
            entity.boat = this;

            this.boatEntitiesByType[entity.BoatEntityType][entity.id] = entity;
        }
    }

    public void ApplyDamage(float damage)
    {
        this.health -= damage;

        if (this.health <= 0)
            Debug.Log("Game Over");
    }
    public void OnRepairStart(Pirate pirate)
    {
    }

    public void OnRepairEnd()
    {
    }

    public Interactable GetInteractableEntityByTypeAndId (BoatEntityType type, int id)
    {
        Interactable interactable = null;

        if (this.boatEntitiesByType.ContainsKey(type))
        {
            if (this.boatEntitiesByType[type].ContainsKey(id))
                interactable = this.boatEntitiesByType[type][id] as Interactable;
        }

        return interactable;
    }
}
