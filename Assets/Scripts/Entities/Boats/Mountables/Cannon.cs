using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : Mountable
{
    public float rotationSpeed = 1;
    public override Mountables MountableType => Mountables.CANNNON;
    public Projectile projectile;
    public Transform Launcher;
    public float power = 1;
    public float reloadTime = 2;

    private float lastFireTime = 0;

    protected override void OnDismount()
    {
    }

    protected override void OnMount()
    {
    }

    public void Update()
    { 
        if (this.mounted)
        {
            bool left = this.pirate.networkInput.GetInput(InputManager.Inputs.Left);
            bool right = this.pirate.networkInput.GetInput(InputManager.Inputs.Right);
            bool fire = this.pirate.networkInput.GetInputDown(InputManager.Inputs.Use);
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
                newProjectile.mount = this;

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
}
