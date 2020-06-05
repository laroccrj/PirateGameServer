using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    public static Dictionary<int, Projectile> Projectiles = new Dictionary<int, Projectile>();
    public static ProjectileManager instance;

    private static int nextId = 0;

    public static Projectile SpawnProjectile(Projectile projectile, Vector3 force)
    {
        projectile.id = nextId++;
        Projectiles.Add(projectile.id, projectile);

        using (Packet packet = new Packet((int)ServerPackets.projectileSpawn))
        {
            packet.Write(projectile.id);
            packet.Write((int) projectile.ProjectileType);
            packet.Write(projectile.firedBy.boat.id);
            packet.Write(projectile.firedBy.id);
            packet.Write((int)projectile.firedBy.BoatEntityType);

            ServerSend.SendUDPDataToAll(packet);
        }

        return projectile;
    }

    public static void ProjectileHit(Projectile projectile)
    {
        using (Packet packet = new Packet((int)ServerPackets.projectileHit))
        {
            packet.Write(projectile.id);
            packet.Write(projectile.transform.position);

            ServerSend.SendUDPDataToAll(packet);
        }
    }

    private void SendProjectileTransformUpdate()
    {
        using (Packet packet = new Packet((int)ServerPackets.projectileUpdate))
        {
            int projectileCount = ProjectileManager.Projectiles.Count;

            if (projectileCount > 0)
            {
                packet.Write(projectileCount);

                foreach (Projectile projectile in ProjectileManager.Projectiles.Values)
                {
                    packet.Write(projectile.id);
                    packet.Write(projectile.transform.position);
                    packet.Write(projectile.transform.rotation);
                }

                ServerSend.SendUDPDataToAll(packet);
            }
        }
    }

    // Update is called once per frame
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }

        // Ignore collisions with the water and projects
        Physics2D.IgnoreLayerCollision(4, 9);
    }

    private void FixedUpdate()
    {
        this.SendProjectileTransformUpdate();
    }
}
