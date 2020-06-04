using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mounter : MonoBehaviour
{
    public const int STATE_IDLE = 0;
    public const int STATE_MOUNTING = 1;
    public const int STATE_MOUNTED = 2;

    public Pirate pirate;
    public float mountDistance = 1;
    public Mountable mountable = null;
    public int mounterState = STATE_IDLE;

    public void mount(Mountable mountable)
    {
        this.mountable = mountable;
        this.mounterState = STATE_MOUNTING;
    }

    public void dismount()
    {
        if (this.mountable != null)
        {
            this.mountable.dismount();
            this.mountable = null;
            this.mounterState = STATE_IDLE;
        }

    }

    public void Update()
    {
        if (
            this.mounterState == STATE_MOUNTING
            && Vector2.Distance(this.transform.position, this.mountable.transform.position) <= this.mountDistance
        )
        {
            this.mounterState = STATE_MOUNTED;
            this.mountable.mount(this);
        }

        if (this.mounterState == STATE_MOUNTED)
        {
            this.transform.position = this.mountable.seat.position;
            //this.transform.rotation = PirateEngine.lookAt(transform, this.mountable.transform.position);
        }
    }
}
