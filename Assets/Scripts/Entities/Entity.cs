using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum EntityType
{
    BOAT,
    PROJECTILE
}

public abstract class Entity : MonoBehaviour
{
    public int id;
    public abstract EntityType EntityType
    {
        get;
    }
}
