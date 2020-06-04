using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public enum Inputs
    {
        Left,
        Right,
        Use
    }

    public delegate void InputHandle(Inputs input, bool down);

    public static InputManager instance;

    public Pirate piratePrefab;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    public static void RecieveInputs(int fromClient, Packet packet)
    {
        int clientId = packet.ReadInt();

        if (fromClient != clientId)
        {
            Debug.Log($"Player (ID: {fromClient}) has assumed the wrong client ID ({clientId})!");
            return;
        }


        Pirate pirate = PirateManager.instance.Pirates[fromClient];
        int inputNumber = packet.ReadInt();

        for(int i = 0; i < inputNumber; i++)
        {
            Inputs input = (Inputs) packet.ReadInt();
            bool down = packet.ReadBool();
            pirate.networkInput.HandleInput(input, down);
        }
    }
}
