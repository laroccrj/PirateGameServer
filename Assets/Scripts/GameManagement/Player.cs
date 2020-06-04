using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public int id;
    public string name;
    public Team team;

    public Player(int id, string name)
    {
        this.id = id;
        this.name = name;
    }
}
