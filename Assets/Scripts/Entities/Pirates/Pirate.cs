using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pirate : MonoBehaviour
{
    public int id;
    public Vector2 destination;
    public float speed = 1;
    public float distanceFromDestination = 0.1f;
    public Boat boat;
    public Mounter mounter;
    public NetworkInput networkInput;
    public Damagable repairing;

    private void Start()
    {
        this.destination = this.transform.localPosition;
    }

    void Update()
    {
        if (
            Vector2.Distance(this.destination, (Vector2)this.transform.localPosition) > this.distanceFromDestination
            && this.mounter.mounterState != Mounter.STATE_MOUNTED
        )
        {
            Vector2 direction = this.destination - (Vector2)this.transform.localPosition;
            transform.Translate(direction.normalized * speed * Time.deltaTime);
        }
    }
}
