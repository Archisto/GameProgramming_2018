using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

namespace TankGame.Testing
{
    public class GetOrAddComponentTest
    {
        [Test]
        public void GetOrAddComponentTestSimplePasses()
        {
            GameObject go = new GameObject();
            TestComponent component = go.GetOrAddComponent<TestComponent>();
            Assert.NotNull(component);
        }

        [Test]
        public void GetOrAddComponentTest_OnlyOneComponent()
        {
            GameObject go = new GameObject();
            go.GetOrAddComponent<TestComponent>();
            go.GetOrAddComponent<TestComponent>();
            go.GetOrAddComponent<TestComponent>();
            go.GetOrAddComponent<TestComponent>();
            TestComponent[] components = go.GetComponents<TestComponent>();
            Assert.IsTrue(components.Length == 1);
        }
    }
}
