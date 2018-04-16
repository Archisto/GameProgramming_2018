using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame.Persistence
{
    [Serializable]
    public class UnitData
    {
        public int ID;
        public int Health;
        public float RemainingRespawnTime;

        // Position
        public SerializableVector3 Position;
        public float YRotation;
    }
}
