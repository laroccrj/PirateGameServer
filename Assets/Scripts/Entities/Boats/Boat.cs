using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Boat : Entity, Damagable
{
    public override EntityType EntityType => EntityType.BOAT;
    public Dictionary<int, Mountable> mountables = new Dictionary<int, Mountable>();
    public Dictionary<int, Wall> walls = new Dictionary<int, Wall>();
    public int acceleration = 1;
    public int maxSpeed = 1;
    public int rotateSpeed = 1;
    public Rigidbody2D rigid;
    public float health;
    public Team team;

    private int nextMountableId = 1;
    private int nextWallId = 1;

    // Start is called before the first frame update
    public void Init(Team team)
    {
        this.id = BoatManager.getNextId();
        this.team = team;
        this.LoadMountables();
        this.LoadWalls();
        this.rigid = this.GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        this.rigid.AddRelativeForce(Vector2.right * acceleration * Time.deltaTime);

        if (rigid.velocity.magnitude > maxSpeed)
            rigid.velocity = rigid.velocity.normalized * maxSpeed;

    }

    private void LoadMountables()
    {
        Mountable[] mountables = this.GetComponentsInChildren<Mountable>();

        foreach(Mountable mountable in mountables)
        {
            mountable.id = this.nextMountableId++;
            this.mountables.Add(mountable.id, mountable);
        }
    }

    private void LoadWalls()
    {
        Wall[] walls = this.GetComponentsInChildren<Wall>();

        foreach(Wall wall in walls)
        {
            wall.id = this.nextWallId++;
            this.walls.Add(wall.id, wall);
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
}
