using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TankGame.Editor
{
    [CustomEditor(typeof(Projectile))]
    public class ProjectileInspector : UnityEditor.Editor
    {
        private const string hitMaskName = "hitMask";
        private SerializedProperty hitMaskProperty;

        protected void OnEnable()
        {
            hitMaskProperty = serializedObject.FindProperty(hitMaskName);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.BeginVertical();

            //GUILayout.Label("Label");
            //EditorGUILayout.LabelField("Label");
            //GUILayout.Button("Button");

            List<string> labels = new List<string>(32);
            for (int i = 0; i < 32; i++)
            {
                string layerName = LayerMask.LayerToName(i);

                //if (layerName.Length > 0)
                //{
                    labels.Add(layerName);
                //}
            }

            hitMaskProperty.intValue = EditorGUILayout.MaskField(
                "Hit Layers", hitMaskProperty.intValue, labels.ToArray());

            EditorGUILayout.LabelField(string.Format("Value: {0}", hitMaskProperty.intValue));

            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.EndVertical();
        }
    }
}
