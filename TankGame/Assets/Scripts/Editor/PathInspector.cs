using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using TankGame.WaypointSystem;

namespace TankGame.Editor
{
    [UnityEditor.CustomEditor(typeof(Path))]
    public class PathInspector : UnityEditor.Editor
    {
        private Path targetPath;

        protected void OnEnable()
        {
            this.targetPath = target as Path;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Add waypoint"))
            {
                int waypointCount = targetPath.transform.childCount;
                string waypointName =
                    string.Format("Waypoint{0}", (waypointCount + 1).ToString("D3"));
                GameObject waypoint = new GameObject(waypointName);
                waypoint.AddComponent<Waypoint>();
                waypoint.transform.SetParent(targetPath.transform);
            }
        }
    }
}
