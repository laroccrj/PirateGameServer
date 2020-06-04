using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pirate : MonoBehaviour
{
    public int id;
    public Vector2 destination;
    public float speed = 1;
    public Boat boat;
    public NetworkInput networkInput;

    public Interactable toInteractWith = null;
    public Interactable isIinteractingWith = null;
    public float interactionReach = 0.1f;

    private void Start()
    {
        this.destination = this.transform.localPosition;
    }

    void Update()
    {
        if (this.isIinteractingWith != null)
        {
            // We are currently interacting with an interactable

            Transform seat = this.isIinteractingWith.GetSeat();
            this.transform.position = seat.position;
            this.transform.rotation = seat.rotation;
        }
        else if (this.toInteractWith != null)
        {
            // We are currently moving towards an interactable with the intention to interact
            Vector3 interactionPoint = this.toInteractWith.GetInteractionPoint();
            interactionPoint = this.boat.transform.InverseTransformPoint(interactionPoint);

            Vector2 direction = interactionPoint - this.transform.localPosition;
            transform.Translate(direction.normalized * speed * Time.deltaTime);

            if (Vector2.Distance(this.transform.localPosition, interactionPoint) <= this.interactionReach)
            {
                this.isIinteractingWith = this.toInteractWith;
                this.toInteractWith = null;
                this.isIinteractingWith.Interact(this);
            }
        }
        else
        {
            Vector2 direction = this.destination - (Vector2)this.transform.localPosition;
            transform.Translate(direction.normalized * speed * Time.deltaTime);
        }
    }

    public void HandleMovement(Vector3 localPosition)
    {
        this.destination = localPosition;
        this.toInteractWith = null;
        this.LeaveInteractable();
    }

    public void BeginInteractWith(Interactable interactable)
    {
        this.LeaveInteractable();
        this.toInteractWith = interactable;
    }

    public void LeaveInteractable()
    {
        if (this.isIinteractingWith != null)
            this.isIinteractingWith.Leave();

        this.isIinteractingWith = null;
    }
}
