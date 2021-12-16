using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

namespace Ballistics
{

    [CustomEditor(typeof(BasicWeaponController))]
    [CanEditMultipleObjects]
    public class WeaponControllerCustomInspector : Editor
    {

        private BallisticSettings Settings;
        private bool showZeroDistances;

        private SerializedProperty weaponProperty;
        private SerializedProperty shootdelayProperty;
        private SerializedProperty weaponTypeProperty;
        private SerializedProperty bulletsPerBurstProperty;
        private SerializedProperty bulletsPerShellProperty;
        private SerializedProperty spreadControllerProperty;
        private SerializedProperty magazineControllerProperty;

        void OnEnable()
        {
            Settings = BulletHandler.instance.GetSettings();

            weaponProperty = serializedObject.FindProperty("TargetWeapon");
            shootdelayProperty = serializedObject.FindProperty("ShootDelay");
            weaponTypeProperty = serializedObject.FindProperty("WeaponType");
            bulletsPerBurstProperty = serializedObject.FindProperty("BulletsPerBurst");
            bulletsPerShellProperty = serializedObject.FindProperty("BulletsPerShell");
            spreadControllerProperty = serializedObject.FindProperty("SpreadController");
            magazineControllerProperty = serializedObject.FindProperty("MagazineController");
        }

        public override void OnInspectorGUI()
        {
            if (Settings != null)
            {
                EditorGUILayout.LabelField("Weapon Controller Editor", EditorStyles.largeLabel);
                EditorGUILayout.Separator();

                EditorGUILayout.LabelField("General Settings:", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(weaponProperty, new GUIContent("Target Weapon:"));

                EditorGUILayout.PropertyField(shootdelayProperty, new GUIContent("Shootdelay:", "Delay between shots (s)"));

                EditorGUILayout.PropertyField(weaponTypeProperty, new GUIContent("Weapon Type:", "Single Shot, Auto, Volley or Burst"));

                EditorGUILayout.Separator();

                EditorGUI.indentLevel--;
                EditorGUILayout.LabelField("Burst/ Shotgun Mode:", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(bulletsPerBurstProperty, new GUIContent("Bullets per burst:", "Bullets shot in one burst"));
                EditorGUILayout.PropertyField(bulletsPerShellProperty, new GUIContent("Bullets per shotgun shell:", "Bullets in one shell"));

                EditorGUILayout.Space();

                EditorGUILayout.Space();
                EditorGUI.indentLevel--;
                EditorGUILayout.LabelField("Controller:", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(spreadControllerProperty, new GUIContent("Spread Controller:"));
                EditorGUILayout.PropertyField(magazineControllerProperty, new GUIContent("Magazine Controller:"));

                EditorGUILayout.Space();


                if (Settings.useBulletdrop)
                {
                    BasicWeaponController weaponController = (BasicWeaponController)target;
                    EditorGUI.indentLevel--;
                    EditorGUILayout.LabelField(new GUIContent("Zeroing:", "Account for bulletdrop at given distances"), EditorStyles.boldLabel);
                    EditorGUI.indentLevel++;

                    EditorGUILayout.BeginHorizontal();
                    showZeroDistances = EditorGUILayout.Foldout(showZeroDistances, "Barrel Zerodistances");
                    if (GUILayout.Button("+"))
                    {
                        float[] dists = weaponController.zeroingDistances;
                        float[] newDists = new float[dists.Length + 1];
                        dists.CopyTo(newDists, 0);
                        newDists[dists.Length] = 100;
                        weaponController.zeroingDistances = newDists;

                        showZeroDistances = true;
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUI.indentLevel++;
                    if (showZeroDistances)
                    {
                        for (int i = 0; i < weaponController.zeroingDistances.Length; i++)
                        {
                            EditorGUILayout.BeginHorizontal();
                            weaponController.zeroingDistances[i] = EditorGUILayout.FloatField("Zero distance " + i.ToString() + " :", weaponController.zeroingDistances[i]);
                            if (GUILayout.Button("-"))
                            {
                                float[] dists = weaponController.zeroingDistances;
                                float[] newDists = new float[dists.Length - 1];
                                for (int n = 0; n < newDists.Length; n++)
                                {
                                    newDists[n] = dists[n + ((n >= i) ? 1 : 0)];
                                }
                                weaponController.zeroingDistances = newDists;
                                i--;
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                    }
                }
            }
            else
            {
                Settings = BulletHandler.instance.GetSettings();
                EditorGUILayout.LabelField("No Ballistic Settings found! Add BulletHandler with settings file to the scene!");
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}