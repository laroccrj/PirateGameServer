using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringWheel : Mountable
{
    public const int STATE_IDLE = 0;
    public const int STATE_RIGHT = 1;
    public const int STATE_LEFT = 2;

    public int state = STATE_IDLE;
    public float maxBoatRotationSpeed = 1;
    public float boatRotateSpeedStep = 0.01f;
    public float bodyRotationSpeed = 1;
    public bool breaking = false;
    private float currentBoatRotateSpeed = 0;

    public override Mountables MountableType => Mountables.STEERING_WHEEL;

    protected override void OnDismount()
    {
    }

    protected override void OnMount()
    {
    }

    public void Update()
    {
        if (this.mounted)
        {
            bool left = mounter.pirate.networkInput.GetInput(InputManager.Inputs.Left);
            bool right = mounter.pirate.networkInput.GetInput(InputManager.Inputs.Right);

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
        if (!this.mounted)
        {
            body.transform.localRotation = Quaternion.Slerp(body.transform.localRotation, Quaternion.identity, Time.deltaTime * this.bodyRotationSpeed);

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
                    rotation.eulerAngles = this.body.transform.TransformDirection(Vector3.forward) * 90;
                    this.currentBoatRotateSpeed += this.boatRotateSpeedStep;
                    break;
                case STATE_RIGHT:
                    rotation.eulerAngles = this.body.transform.TransformDirection(Vector3.back) * 90;
                    this.currentBoatRotateSpeed -= this.boatRotateSpeedStep;
                    break;
                default:
                    rotation = this.body.transform.localRotation;
                    break;
            }

            body.transform.localRotation = Quaternion.Slerp(body.transform.localRotation, rotation, Time.deltaTime * this.bodyRotationSpeed);
        }

        this.boat.rigid.AddTorque(this.boat.rotateSpeed * Time.deltaTime * this.boat.rigid.velocity.magnitude * this.currentBoatRotateSpeed);
    }
}
