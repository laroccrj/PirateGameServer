using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ProjectileType
{
    CANNON_BALL = 1
}

public abstract class Projectile : Entity
{
    public override EntityType EntityType => EntityType.PROJECTILE;
    public Rigidbody2D rigid;
    public Mountable mount;

    public abstract ProjectileType ProjectileType
    {
        get;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        ProjectileManager.ProjectileHit(this);
        this.OnProjectileHit(collision);
    }

    public abstract void OnProjectileHit(Collision2D collision);
}
