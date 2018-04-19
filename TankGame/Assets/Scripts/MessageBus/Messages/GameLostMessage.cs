using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame.Messaging
{
    /// <summary>
    /// A message that is sent when the game is lost.
    /// </summary>
    public class GameLostMessage : IMessage
    {
        public GameLostMessage() { }

        public void PrintMessage()
        {
            Debug.Log("Game lost");
        }
    }
}
