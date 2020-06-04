using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface Interactable
{   
    // The spot the pirate needs to reach to interact in world space
    Vector3 GetInteractionPoint();

    void Interact(Pirate pirate);

    void Leave();

    Transform GetSeat();
}
