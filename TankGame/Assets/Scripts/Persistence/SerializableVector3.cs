using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame.Persistence
{
    [Serializable]
    public class SerializableVector3
    {
        public float X;
        public float Y;
        public float Z;

        public SerializableVector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public SerializableVector3(Vector3 vector)
            : this(vector.x, vector.y, vector.z)
        {
        }

        public SerializableVector3(Vector2 vector)
            : this(vector.x, vector.y, 0)
        {
        }

        public static implicit operator SerializableVector3(Vector3 v)
        {
            return new SerializableVector3(v);
        }

        public static implicit operator SerializableVector3(Vector2 v)
        {
            return new SerializableVector3(v);
        }

        public static implicit operator Vector3(SerializableVector3 v)
        {
            return new Vector3(v.X, v.Y, v.Z);
        }

        public static explicit operator Vector2(SerializableVector3 v)
        {
            return new Vector2(v.X, v.Y);
        }
    }
}
