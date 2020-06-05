using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class CannonBall : Projectile
{
    public override ProjectileType ProjectileType => ProjectileType.CANNON_BALL;
    public float damage;
    public float damageRadius;

    private bool explodeNextFrame = false;

    public void FixedUpdate()
    {
        if (explodeNextFrame)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(this.transform.position, this.damageRadius);

            foreach(Collider2D collider in colliders)
            {
                Debug.Log(collider.gameObject.name);
                Damagable damagable = collider.GetComponent<Damagable>();

                if (damagable == null)
                    continue;

                Debug.Log("Has collider");

                Vector2 damagePoint = collider.ClosestPoint(this.transform.position);
                float distance = Vector2.Distance(this.transform.position, damagePoint);
                float distanceDamageReduction = this.damage * (distance / damageRadius);
                damagable.ApplyDamage(this.damage - distanceDamageReduction);
            }

            ProjectileManager.Projectiles.Remove(this.id);
            Destroy(this.gameObject);
        }
    }

    public override void OnProjectileHit(Collision2D collision)
    {
        this.explodeNextFrame = true;
    }
}
