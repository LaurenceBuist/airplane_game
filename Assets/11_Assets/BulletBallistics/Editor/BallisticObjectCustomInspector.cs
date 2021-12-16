using UnityEngine;
using System.Collections;
using UnityEditor;

namespace Ballistics
{

    [CustomEditor(typeof(BallisticObject), true)]
    [CanEditMultipleObjects]
    public class BallisticObjectCustomInspector : Editor
    {
        string[] Names;
        BallisticSettings Settings;
        BallisticObject myTarget;

        public override void OnInspectorGUI()
        {
            BallisticObject myTarget = (BallisticObject)target;
            Settings = BulletHandler.instance.GetSettings();

            DrawDefaultInspector();
            
            if (Settings != null)
            {
                if (Names == null || Names.Length != Settings.materialData.Count)
                {
                    Names = new string[Settings.materialData.Count];
                }
                for (int i = 0; i < Settings.materialData.Count; i++)
                {
                    Names[i] = (i + 1) + "- " + Settings.materialData[i].Name;
                }


                EditorGUI.BeginChangeCheck();
                myTarget.MatID = EditorGUILayout.Popup("Material type:", myTarget.MatID, Names);
                if (EditorGUI.EndChangeCheck())
                {
                    foreach (Object obj in targets)
                    {
                        ((BallisticObject)obj).MatID = myTarget.MatID;
                    }
                }

                SerializedProperty onHit = serializedObject.FindProperty("OnHit");
                EditorGUILayout.PropertyField(onHit, true);

                if (GUI.changed)
                {
                    serializedObject.ApplyModifiedProperties();
                }
            }
            else
            {
                EditorGUILayout.LabelField("No Settings Found!");
                EditorGUILayout.LabelField("Check Ballistics Editor!");
            }
        }
    }
}
