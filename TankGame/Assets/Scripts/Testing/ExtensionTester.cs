using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame
{
    public class ExtensionTester : MonoBehaviour
    {
        [SerializeField]
        private Collider anyCollider;

        [SerializeField]
        private bool includeInactive;

        public void Run()
        {
            anyCollider = gameObject.GetComponentInHierarchy<Collider>(includeInactive);

            if (anyCollider != null)
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
