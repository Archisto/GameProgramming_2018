using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

namespace TankGame.Testing
{
    public class GetComponentInHierarchyTester : MonoBehaviour
    {
        private bool includeInactive;

        public void Setup(bool includeInactive, bool componentInParent, bool setActive)
        {
            this.includeInactive = includeInactive;
            GameObject go;
            if (componentInParent)
            {
                go = transform.parent.gameObject;
            }
            else
            {
                go = transform.GetChild(0).gameObject;
            }

            go.AddComponent<TestComponent>();
            go.SetActive(setActive);
        }

        public TestComponent Run()
        {
            return gameObject.GetComponentInHierarchy<TestComponent>(includeInactive);
        }
    }
}
