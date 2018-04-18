using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame.Messaging
{
    public class GameResetMessage : IMessage
    {
        public GameResetMessage() { }

        public void PrintMessage()
        {
            Debug.Log("Game reset");
        }
    }
}
