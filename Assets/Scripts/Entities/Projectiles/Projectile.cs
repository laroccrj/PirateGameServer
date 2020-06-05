using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ProjectileType
{
    CANNON_BALL = 1
}

public abstract class Projectile : MonoBehaviour
{
    public int id;
    public Rigidbody2D rigid;
    public BoatEntity firedBy;

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
