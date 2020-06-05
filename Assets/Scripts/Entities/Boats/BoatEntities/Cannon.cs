using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : BoatEntity, Interactable
{
    private static int nextId = 1;

    public float rotationSpeed = 1;
    public override BoatEntityType BoatEntityType => BoatEntityType.CANNON;
    public Projectile projectile;
    public Transform Launcher;
    public float power = 1;
    public float reloadTime = 2;

    public bool reverseRotation = false;
    public Pirate Pirate { get; private set; }
    public Transform body;
    public Transform Seat;

    private float lastFireTime = 0;

    public void Update()
    {
        if (Pirate != null)
        {
            bool left = this.Pirate.networkInput.GetInput(InputManager.Inputs.Left);
            bool right = this.Pirate.networkInput.GetInput(InputManager.Inputs.Right);
            bool fire = this.Pirate.networkInput.GetInputDown(InputManager.Inputs.Use);
            Vector3 rotation = Vector3.zero;

            if (left && !right)
            {
                rotation = Vector3.forward * Time.deltaTime * rotationSpeed;
            }
            else if (right && !left)
            {
                rotation = Vector3.back * Time.deltaTime * rotationSpeed;
            }

            if (reverseRotation)
                rotation *= -1;

            this.body.transform.Rotate(rotation);

            if (fire && Time.time > lastFireTime + reloadTime)
            {
                Projectile newProjectile = GameObject.Instantiate<Projectile>(projectile, this.Launcher);
                newProjectile.firedBy = this;

                Collider2D projectileCollider = newProjectile.GetComponent<Collider2D>();
                Collider2D[] boatColliders = boat.GetComponentsInChildren<Collider2D>();

                foreach(Collider2D boatCollider in boatColliders)
                {
                    Physics2D.IgnoreCollision(projectileCollider, boatCollider);
                }

                Vector3 force = Vector3.up * power;
                newProjectile.GetComponent<Rigidbody2D>().velocity = this.boat.rigid.velocity;
                newProjectile.GetComponent<Rigidbody2D>().AddRelativeForce(Vector3.up * power, ForceMode2D.Impulse);
                newProjectile.transform.parent = null;
                ProjectileManager.SpawnProjectile(newProjectile, force);
                lastFireTime = Time.time;
            }
        }
    }
    /*
    public override void ApplyDamage(float damage)
    {
        this.health -= damage;
    }*/

    public override int GetNextId()
    {
        return nextId++;
    }

    public override void WriteDataToPacket(ref Packet packet)
    {
        packet.Write(this.body.localRotation);
    }

    public Vector3 GetInteractionPoint()
    {
        return this.transform.position;
    }

    public void Interact(Pirate pirate, InteractionType interactionType)
    {
        this.Pirate = pirate;
    }

    public void Leave()
    {
        this.Pirate = null;
    }

    public Transform GetSeat()
    {
        return this.Seat;
    }
}
