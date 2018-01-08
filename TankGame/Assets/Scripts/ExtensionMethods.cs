using System;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame
{
    public static class ExtensionMethods
    {
        public static TComponent GetOrAddComponent<TComponent>(this GameObject obj)
            where TComponent : Component
        {
            TComponent component = obj.GetComponent<TComponent>();
            if (component == null)
            {
                component = obj.AddComponent<TComponent>();
            }

            return component;
        }

        public static Component GetOrAddComponent(this GameObject obj, Type type)
        {
            Component component = obj.GetComponent(type);
            if (component == null)
            {
                component = obj.AddComponent(type);
            }

            return component;
        }

        public static Vector3 NormalizeLargeVector3(this Vector3 vector)
        {
            while (vector.x > 360)
            {
                vector.x -= 360;
            }
            while (vector.y > 360)
            {
                vector.y -= 360;
            }
            while (vector.z > 360)
            {
                vector.z -= 360;
            }

            return vector;
        }

        public static Vector3 AngleToDirection(this Vector3 vector)
        {
            vector.x = vector.x / 360;
            vector.y = vector.y / 360;
            vector.z = vector.z / 360;

            return vector;
        }
    }
}
