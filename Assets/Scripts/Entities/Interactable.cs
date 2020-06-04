using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InteractionType
{
    USE = 1,
    REPAIR
}

public enum InteractableType
{
    MOOUNTABLE,
    WALL
}

public interface Interactable
{   
    // The spot the pirate needs to reach to interact in world space
    Vector3 GetInteractionPoint();

    void Interact(Pirate pirate, InteractionType interactionType);

    void Leave();

    Transform GetSeat();

    InteractableType GetInteractableType();
}
