using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

namespace Ballistics
{
    public class BallisticsEditor : EditorWindow
    {

        private BallisticSettings Settings;
        bool[] MatEnabled;
        Vector2 scrollVec;

        [MenuItem("Ballistics/Settings")]
        static void Init()
        {
            BallisticsEditor myWindow = (BallisticsEditor)GetWindow(typeof(BallisticsEditor), false, "Ballistics Settings");
            myWindow.Show();
        }

        void OnEnable()
        {
            Settings = BulletHandler.instance.GetSettings();

            if (Settings != null)
            {
                MatEnabled = new bool[Settings.materialData.Count];
            }
        }

        void OnInspectorUpdate()
        {
            //Load Ballistic Settings Scriptable Object
            BallisticSettings bs = BulletHandler.instance.GetSettings();
            if (Settings != bs)
            {
                Settings = bs;
                MatEnabled = new bool[Settings.materialData.Count];
            }
        }

        void OnGUI()
        {
            scrollVec = EditorGUILayout.BeginScrollView(scrollVec);
            if (Settings != null)
            {
                // World Settings:

                EditorGUILayout.LabelField("World Settings", EditorStyles.largeLabel);
                EditorGUILayout.Separator();

                //Quality Settings
                EditorGUILayout.LabelField("Ballistic Quality:", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                Settings.useBulletdrop = EditorGUILayout.Toggle(new GUIContent("Bulletdrop: ", "Activate bulletdrop calculations"), Settings.useBulletdrop);
                Settings.useBulletdrag = EditorGUILayout.Toggle(new GUIContent("Bulletdrag: ", "Activate bulletdrag calculations"), Settings.useBulletdrag);
                Settings.useBallisticMaterials = EditorGUILayout.Toggle(new GUIContent("Ballistic Materials: ", "Enable custom material behaviour"), Settings.useBallisticMaterials);

                //________________

                if (Settings.useBulletdrag)
                {
                    EditorGUILayout.Separator();
                    EditorGUI.indentLevel--;
                    EditorGUILayout.LabelField("Drag Settings:", EditorStyles.boldLabel);
                    EditorGUI.indentLevel++;
                    Settings.AirDensity = EditorGUILayout.FloatField(new GUIContent("Air Density", "air density (kg/uints³)"), Settings.AirDensity);
                    Settings.WindVelocity = EditorGUILayout.Vector3Field(new GUIContent("Wind Velocity", "Wind velocity (uints/s)"), Settings.WindVelocity);
                }

                //---------------------------------------------------------------------
                //Ballistic Material Editor
                EditorGUILayout.Separator();
                if (Settings.useBallisticMaterials)
                {
                    EditorGUI.indentLevel--;
                    EditorGUILayout.LabelField("Material Settings:", EditorStyles.boldLabel);
                    EditorGUI.indentLevel++;

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Material Types");

                    if (GUILayout.Button("+", GUILayout.MaxWidth(85)))
                    {
                        MaterialData newData = new MaterialData();
                        newData.RicochetPropability = new AnimationCurve();
                        newData.Name = "New Mat";
                        newData.EnergylossPerUnit = 1500f;
                        newData.RndSpread = 0.1f;
                        newData.RndSpreadRic = 0.2f;
                        newData.RicochetPropability = new AnimationCurve(new Keyframe[] { new Keyframe(0f, 0f), new Keyframe(1f, 0.1f) });
                        Settings.materialData.Add(newData);

                        //add 1 element to array
                        bool[] newMatEnabled = new bool[Settings.materialData.Count];
                        System.Array.Copy(MatEnabled, newMatEnabled, MatEnabled.Length);
                        MatEnabled = newMatEnabled;
                    }

                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space();
                    EditorGUI.indentLevel++;

                    for (int i = 0; i < Settings.materialData.Count; i++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        MaterialData data = Settings.materialData[i];
                        MatEnabled[i] = EditorGUILayout.Foldout(MatEnabled[i], (i+1)+"- "+data.Name);
                        if (GUILayout.Button("▲", GUILayout.MaxWidth(40)) && i != 0)
                        {
                            Swap(Settings.materialData, i, i - 1);
                            Swap(MatEnabled, i, i - 1);
                            break;
                        }
                        if (GUILayout.Button("▼", GUILayout.MaxWidth(40)) && i != Settings.materialData.Count - 1)
                        {
                            Swap(Settings.materialData, i, i + 1);
                            Swap(MatEnabled, i, i + 1);
                            Repaint();
                            break;
                        }

                        if (GUILayout.Button("-", GUILayout.MaxWidth(40)) && EditorUtility.DisplayDialog("Remove Material", "Are you sure you want to delete "+data.Name+"?","Yes","No"))
                        {
                            Settings.materialData.RemoveAt(i);

                            //remove at i
                            bool[] newMatEnabled = new bool[Settings.materialData.Count];
                            for(int n = 0; n < newMatEnabled.Length; n++)
                            {
                                newMatEnabled[n] = MatEnabled[n + (n >= i ? 1 : 0)];
                            }
                            MatEnabled = newMatEnabled;
                            i--;
                            continue;
                        }
                        EditorGUILayout.EndHorizontal();
                        if (MatEnabled[i])
                        {
                            EditorGUI.indentLevel++;
                            data.Name = EditorGUILayout.TextField("Material Name:", data.Name);
                            if (Settings.useBallisticMaterials)
                            {
                                data.EnergylossPerUnit = BigFloatField(new GUIContent("Energyloss Per Unit:", "Energyloss of a bullet penetrating through 1 unit of this material"), data.EnergylossPerUnit);
                                data.RndSpread = BigFloatField(new GUIContent("Spreadangle:", "Maximum spread applied to the bullet direction after exiting this material (degree)"), data.RndSpread);
                                data.RndSpreadRic = BigFloatField(new GUIContent("Spreadangle (ricochet):", "Maximum spread applied to the bullet direction after being reflected from this material (degree)"), data.RndSpreadRic);
                                data.RicochetPropability = BigCurveField(new GUIContent("Ricochet Propability", "Propability (0<y<1) of a bullet being reflected by this material at 0°-90° impact angle (0<x<1)"), data.RicochetPropability);
                                data.impactObject = (ImpactObject)BigObjectField(new GUIContent("Impact Object:", "ImpactObject created at bullet impact point (ImpactObject)"), data.impactObject, typeof(ImpactObject));

                            }
                            EditorGUI.indentLevel--;
                        }

                        Settings.materialData[i] = data;
                        EditorGUILayout.Space();
                    }
                }
                //-----------------------------------------------------------------------------

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Save_Settings", EditorStyles.miniButton))
                {
                    EditorUtility.SetDirty(Settings);
                    AssetDatabase.SaveAssets();
                }
                EditorGUILayout.LabelField("");
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.Separator();
                Settings = BulletHandler.instance.GetSettings();
                EditorGUILayout.LabelField("No Ballistic Settings found! Add BulletHandler with settings file to the scene!");
            }

            EditorGUILayout.EndScrollView();
        }

        float BigFloatField(GUIContent Text, float inVal)
        {
            int indent = EditorGUI.indentLevel;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(Text);
            EditorGUI.indentLevel = 0;
            float outVal = EditorGUILayout.FloatField("", inVal);
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel = indent;
            return outVal;
        }

        AnimationCurve BigCurveField(GUIContent Text, AnimationCurve inVal)
        {
            int indent = EditorGUI.indentLevel;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(Text);
            EditorGUI.indentLevel = 0;
            AnimationCurve outVal = EditorGUILayout.CurveField("", inVal);
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel = indent;
            return outVal;
        }

        Object BigObjectField(GUIContent Text, Object inVal, System.Type myType)
        {
            int indent = EditorGUI.indentLevel;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(Text);
            EditorGUI.indentLevel = 0;
            Object outVal = EditorGUILayout.ObjectField("", inVal, myType, false);
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel = indent;
            return outVal;
        }

        private void Swap<T>(List<T> list, int a, int b)
        {
            T tmp = list[a];
            list[a] = list[b];
            list[b] = tmp;
        }

        private void Swap<T>(T[] list, int a, int b)
        {
            T tmp = list[a];
            list[a] = list[b];
            list[b] = tmp;
        }
    }
}