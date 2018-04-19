using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame.Messaging
{
    /// <summary>
    /// A message that is sent when the game is reset.
    /// </summary>
    public class GameResetMessage : IMessage
    {
        public GameResetMessage() { }

        public void PrintMessage()
        {
            Debug.Log("Game reset");
        }
    }
}
