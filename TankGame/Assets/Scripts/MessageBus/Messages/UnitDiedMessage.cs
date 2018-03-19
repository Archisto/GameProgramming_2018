using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame.Messaging
{
    public class UnitDiedMessage : IMessage
    {
        public Unit DeadUnit { get; private set; }

        public UnitDiedMessage(Unit unit)
        {
            DeadUnit = unit;
        }

        public void PrintMessage()
        {
            Debug.Log("UnitDiedMessage - DeadUnit: " + DeadUnit.ToString());
        }
    }
}
