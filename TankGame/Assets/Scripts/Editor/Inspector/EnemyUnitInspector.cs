using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using TankGame.WaypointSystem;

namespace TankGame.Editor
{
    [UnityEditor.CustomEditor(typeof(EnemyUnit))]
    public class EnemyUnitInspector : UnityEditor.Editor
    {
        private EnemyUnit targetEnemyUnit;
        private int damageAmount = 10;

        protected void OnEnable()
        {
            targetEnemyUnit = target as EnemyUnit;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            //GUILayout.Space(5);
            //GUILayout.Label("Debug", EditorStyles.boldLabel);
            //GUILayout.Label("Debug", EditorStyles.popup);
            GUILayout.Label("Debug", EditorStyles.largeLabel);
            //GUILayout.Label("Debug", EditorStyles.miniLabel);

            damageAmount =
                EditorGUILayout.IntField("Damage Amount", damageAmount);

            // This syntax only works in C# version 6 or greater
            //string buttonTitle = $"Take {damageAmount} Damage";

            string buttonTitle =
                string.Format("Take {0} Damage", damageAmount);

            GUI.enabled = Application.isPlaying;
            if (GUILayout.Button(buttonTitle))
            {
                targetEnemyUnit.TakeDamage(damageAmount);
            }
            GUI.enabled = true;
        }
    }
}
