using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NetworkInput : MonoBehaviour
{
    private Dictionary<InputManager.Inputs, InputState> inputStates = new Dictionary<InputManager.Inputs, InputState>();

    private void Awake()
    {
        foreach (InputManager.Inputs input in System.Enum.GetValues(typeof(InputManager.Inputs)))
        {
            this.inputStates.Add(input, new InputState());
        }
    }

    void Update()
    {
        foreach (InputState inputState in inputStates.Values)
        {
            inputState.downLastFrame = inputState.down;
        }
    }

    public bool GetInput(InputManager.Inputs input)
    {
        return this.inputStates[input].down;
    }
    
    public bool GetInputDown(InputManager.Inputs input)
    {
        return this.inputStates[input].down && !this.inputStates[input].downLastFrame;
    }
    
    public bool GetInputUp(InputManager.Inputs input)
    {
        return !this.inputStates[input].down && this.inputStates[input].downLastFrame;
    }

    public void HandleInput(InputManager.Inputs input, bool down)
    {
        this.inputStates[input].down = down;
    }

    private class InputState
    {
        public bool down = false;
        public bool downLastFrame = false;
    }
}
