using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame
{
    public class ExtensionTest : MonoBehaviour
    {
        [SerializeField]
        private Collider collider;

        [SerializeField]
        private bool includeInactive;

        public void Run()
        {
            collider = gameObject.GetComponentInHierarchy<Collider>(includeInactive);

            if (collider != null)
            {
                Debug.Log("Collider found");
            }
            else
            {
                Debug.Log("Collider not found");
            }
        }
    }
}
