using UnityEngine;
using UnityEditor;
using System.Collections;

namespace TankGame.Editor
{
    [UnityEditor.CustomEditor(typeof(Unit), editorForChildClasses: true)]
    public class UnitInspector : UnityEditor.Editor
    {
        private Unit targetUnit;

        protected virtual void OnEnable()
        {
            targetUnit = target as Unit;

            if (targetUnit != null && targetUnit.ID < 0)
            {
                Undo.RecordObject(targetUnit, "Set ID for the Unit");
                targetUnit.RequestID();
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Label("ID: " + targetUnit.ID);
        }
    }
}
