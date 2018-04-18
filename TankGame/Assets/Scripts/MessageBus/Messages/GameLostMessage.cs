using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame.Messaging
{
    public class GameLostMessage : IMessage
    {
        public GameLostMessage() { }

        public void PrintMessage()
        {
            Debug.Log("Game lost");
        }
    }
}
