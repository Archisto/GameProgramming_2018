using UnityEngine;
using UnityEditor;

namespace TankGame.Editor
{
    [UnityEditor.CustomEditor(typeof(EnemyUnit))]
    public class EnemyUnitInspector : UnitInspector
    {
        private EnemyUnit targetEnemyUnit;
        private int damageAmount = 10;

        protected override void OnEnable()
        {
            base.OnEnable();

            targetEnemyUnit = target as EnemyUnit;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            //GUILayout.Space(5);
            GUILayout.Label("Debug", EditorStyles.largeLabel);
            //GUILayout.Label("Debug", EditorStyles.boldLabel);
            //GUILayout.Label("Debug", EditorStyles.miniLabel);
            //GUILayout.Label("Debug", EditorStyles.radioButton);
            //GUILayout.Label("Debug", EditorStyles.popup); etc.

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
