using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame
{
    public class ExtensionTestRunner : MonoBehaviour
    {
        [SerializeField]
        private ExtensionTester test;

        public void Start()
        {
            test.Run();
        }
    }
}
