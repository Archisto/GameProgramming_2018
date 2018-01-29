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

        /// <summary>
        /// Adds an item to the list only if it
        /// doesn't exist on the list already.
        /// </summary>
        /// <typeparam name="T">The type of the item</typeparam>
        /// <param name="list">The list to which
        /// the item should be added</param>
        /// <param name="item">The item to be added to the list</param>
        /// <returns>True if the item was not on the list before and
        /// was added to the list successfully, otherwise false</returns>
        public static bool AddUnique<T>(this IList<T> list, T item)
        {
            if (list.Contains(item))
            {
                return false;
            }
            else
            {
                list.Add(item);
                return true;
            }
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
