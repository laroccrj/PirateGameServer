using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringWheel : BoatEntity, Interactable
{
    private static int nextId = 1;

    public const int STATE_IDLE = 0;
    public const int STATE_RIGHT = 1;
    public const int STATE_LEFT = 2;

    public Pirate Pirate { get; private set; }
    public int state = STATE_IDLE;
    public float maxBoatRotationSpeed = 1;
    public float boatRotateSpeedStep = 0.01f;
    public float bodyRotationSpeed = 1;
    public bool breaking = false;
    public Transform Seat;
    public Transform Body;

    private float currentBoatRotateSpeed = 0;

    public override BoatEntityType BoatEntityType => BoatEntityType.STEERING_WHEEL;

    public override int GetNextId()
    {
        return nextId++;
    }

    public override void WriteDataToPacket(ref Packet packet)
    {
        packet.Write(this.Body.localRotation);
    }

    public Vector3 GetInteractionPoint()
    {
        return this.transform.position;
    }

    public void Interact(Pirate pirate, InteractionType interactionType)
    {
        this.Pirate = pirate;
    }

    public void Leave()
    {
        this.Pirate = null;
    }

    public Transform GetSeat()
    {
        return this.Seat;
    }

    public void Update()
    {
        if (this.Pirate != null)
        {
            bool left = this.Pirate.networkInput.GetInput(InputManager.Inputs.Left);
            bool right = this.Pirate.networkInput.GetInput(InputManager.Inputs.Right);

            if (left && !right)
            {
                this.state = STATE_LEFT;
            }
            else if (!left && right)
            {
                this.state = STATE_RIGHT;
            }
            else
            {
                this.state = STATE_IDLE;
            }
        }
        else
        {
            this.state = STATE_IDLE;
        }
    }

    private void FixedUpdate()
    {
        if (this.Pirate == null)
        {
            this.Body.transform.localRotation = Quaternion.Slerp(this.Body.transform.localRotation, Quaternion.identity, Time.deltaTime * this.bodyRotationSpeed);

            if (this.currentBoatRotateSpeed > 0)
            {
                this.currentBoatRotateSpeed -= boatRotateSpeedStep;
            }
            else if (this.currentBoatRotateSpeed < 0)
            {
                this.currentBoatRotateSpeed += boatRotateSpeedStep;
            }
        } 
        else
        {
            Quaternion rotation = Quaternion.identity;

            switch (this.state)
            {
                case STATE_LEFT:
                    rotation.eulerAngles = this.Body.transform.TransformDirection(Vector3.forward) * 90;
                    this.currentBoatRotateSpeed += this.boatRotateSpeedStep;
                    break;
                case STATE_RIGHT:
                    rotation.eulerAngles = this.Body.transform.TransformDirection(Vector3.back) * 90;
                    this.currentBoatRotateSpeed -= this.boatRotateSpeedStep;
                    break;
                default:
                    rotation = this.Body.transform.localRotation;
                    break;
            }

            Body.transform.localRotation = Quaternion.Slerp(Body.transform.localRotation, rotation, Time.deltaTime * this.bodyRotationSpeed);
        }

        this.boat.rigid.AddTorque(this.boat.rotateSpeed * Time.deltaTime * this.boat.rigid.velocity.magnitude * this.currentBoatRotateSpeed);
    }
}
