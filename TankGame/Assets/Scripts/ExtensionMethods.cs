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

        public static TComponent GetComponentInInactiveParent<TComponent>
            (this GameObject gameObject)
            where TComponent : Component
        {
            TComponent result = null;

            if (gameObject.transform.parent != null)
            {
                GameObject parent = gameObject.transform.parent.gameObject;

                result = parent.GetComponent<TComponent>();

                if (result == null)
                {
                    result = parent.GetComponentInInactiveParent<TComponent>();
                }
            }

            return result;
        }

        public static TComponent GetComponentInHierarchy<TComponent>
            (this GameObject gameObject, bool includeInactive = false)
            where TComponent : Component
        {
            TComponent result = gameObject.GetComponent<TComponent>();
            if (result != null && !includeInactive && !result.gameObject.activeSelf)
            {
                result = null;
            }

            if (result == null)
            {
                result = gameObject.GetComponentInParent<TComponent>();
                if (includeInactive)
                {
                    result = gameObject.GetComponentInInactiveParent<TComponent>();
                }
                else
                {
                    result = gameObject.GetComponentInParent<TComponent>();
                }
            }

            if (result == null)
            {
                result = gameObject.GetComponentInChildren<TComponent>(includeInactive);
            }

            return result;
        }

        public static Vector3 CompressLargeVector3(this Vector3 vector)
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
    }
}
