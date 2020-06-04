using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CannonBall : Projectile
{
    public override ProjectileType ProjectileType => ProjectileType.CANNON_BALL;
    public float damage;

    public override void OnProjectileHit(Collision2D collision)
    {
        Damagable damagable = collision.collider.GetComponent<Damagable>();

        if (damagable != null)
            damagable.ApplyDamage(damage);

        ProjectileManager.Projectiles.Remove(this.id);
        Destroy(this.gameObject);
    }
}
