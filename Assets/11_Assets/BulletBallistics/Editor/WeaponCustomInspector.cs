using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

namespace Ballistics
{

    [CustomEditor(typeof(Weapon), true)]
    [CanEditMultipleObjects]
    public class WeaponCustomInspector : Editor
    {
        Weapon TargetWeapon;

        private bool showBarrelZero;

        private BallisticSettings Settings;

        private SerializedProperty visualSpawnPointProperty;
        private SerializedProperty physicalSpawnPointProperty;
        private SerializedProperty lifeTimeProperty;
        private SerializedProperty hitMaskProperty;
        private SerializedProperty bulletPrefabProperty;
        private SerializedProperty muzzleDamageProperty;
        private SerializedProperty maxSpeedProperty;
        private SerializedProperty massProperty;
        private SerializedProperty dragProperty;
        private SerializedProperty diameterProperty;

        void OnEnable()
        {
            TargetWeapon = (Weapon)target;
            Settings = BulletHandler.instance.GetSettings();

            visualSpawnPointProperty = serializedObject.FindProperty("VisualSpawnPoint");
            physicalSpawnPointProperty = serializedObject.FindProperty("PhysicalBulletSpawnPoint");
            lifeTimeProperty = serializedObject.FindProperty("LifeTimeOfBullets");
            bulletPrefabProperty = serializedObject.FindProperty("BulletPref");
            SerializedProperty bulletInfoProperty = serializedObject.FindProperty("bulletInfo");
            muzzleDamageProperty = bulletInfoProperty.FindPropertyRelative("MuzzleDamage");
            maxSpeedProperty = bulletInfoProperty.FindPropertyRelative("MaxSpeed");
            massProperty = bulletInfoProperty.FindPropertyRelative("Mass");
            diameterProperty = bulletInfoProperty.FindPropertyRelative("Diameter");
            dragProperty = bulletInfoProperty.FindPropertyRelative("DragCoefficient");
            hitMaskProperty = bulletInfoProperty.FindPropertyRelative("HitMask");
        }

        public override void OnInspectorGUI()
        {
            if (Settings != null)
            {
                EditorGUILayout.LabelField("Weapon Editor", EditorStyles.largeLabel);
                EditorGUILayout.Separator();

                EditorGUILayout.LabelField("Weapon Settings:", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(visualSpawnPointProperty, new GUIContent("Visual Spawn Point:", "Position at wich the visual bullet is instanciated (e.g. weapon barrel)"));

                EditorGUILayout.PropertyField(physicalSpawnPointProperty, new GUIContent("Bullet Spawn Point:", "Position where bullet calculations begin (e.g. camera)"));

                EditorGUILayout.PropertyField(lifeTimeProperty, new GUIContent("Lifetime of Bullet:", "Time until destruction (s)"));

                EditorGUILayout.PropertyField(hitMaskProperty, new GUIContent("Hit Mask:", "The layers the bullet can interact with"));


                EditorGUILayout.Space();
                EditorGUI.indentLevel--;
                EditorGUILayout.LabelField("Bullet:", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(muzzleDamageProperty, new GUIContent("Muzzle Damage:", "Damage of one bullet at maximum speed"));

                EditorGUILayout.PropertyField(bulletPrefabProperty, new GUIContent("Bullet Prefab:", "Visual bullet object (can be null)"));

                EditorGUILayout.PropertyField(maxSpeedProperty, new GUIContent("Bullet Speed:", "Start speed of a bullet (units/s)"));

                EditorGUILayout.PropertyField(massProperty, new GUIContent("Mass of Bullet:", "Mass of the bullet (kg)"));

                EditorGUI.indentLevel++;
                EditorGUILayout.LabelField("maximal kinetic energy:\t " + (0.5f * TargetWeapon.bulletInfo.Mass * TargetWeapon.bulletInfo.MaxSpeed * TargetWeapon.bulletInfo.MaxSpeed).ToString() + " J", EditorStyles.miniLabel);
                EditorGUI.indentLevel--;

                if (Settings.useBulletdrag)
                {
                    EditorGUILayout.PropertyField(diameterProperty, new GUIContent("Diameter:", "Bullet diameter (uints)"));

                    EditorGUILayout.PropertyField(dragProperty, new GUIContent("Drag Coefficient:", "Drag coefficient of the bullets (0.5 sphere; ~0.3 bullets)"));
                }

                EditorGUILayout.Space();
                SerializedProperty onShoot = serializedObject.FindProperty("OnShoot");
                EditorGUILayout.PropertyField(onShoot, true);

                serializedObject.ApplyModifiedProperties();
            }
            else
            {
                Settings = BulletHandler.instance.GetSettings();
                EditorGUILayout.LabelField("No Ballistic Settings found! Add BulletHandler with settings file to the scene!");
            }
        }
    }
}