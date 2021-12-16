using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

namespace Ballistics
{
    [CustomEditor(typeof(FalloffMapGenerator))]
    public class FalloffMapGeneratorEditor : Editor
    {

        FalloffMapGenerator t;
        bool ShowZeroList = true;
        private BallisticSettings Settings;

        public void OnSceneGUI()
        {
            Handles.matrix = t.transform.localToWorldMatrix;
            t.BarrelPos = Handles.PositionHandle(t.BarrelPos, Quaternion.LookRotation(Vector3.forward));
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("Weapon Zeroing Settings", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            if (t.TargetWeapon == null)
            {
                EditorGUILayout.LabelField("Set TargetWeapon.");
            }
            else if (Settings == null)
            {
                EditorGUILayout.LabelField("No Settings file connected! Check Ballistics Manager.");
            }

            EditorGUILayout.LabelField("General", EditorStyles.miniBoldLabel);
            EditorGUI.indentLevel++;
            t.TargetWeapon = (Weapon)EditorGUILayout.ObjectField("My Weapon:", (Object)t.TargetWeapon, typeof(Weapon), true);
            t.Size = EditorGUILayout.Slider("Gizmo Size:", t.Size, 0.02f, 1.2f);

            EditorGUILayout.Space();
            EditorGUI.indentLevel--;
            EditorGUILayout.LabelField("Zeroing", EditorStyles.miniBoldLabel);
            EditorGUI.indentLevel++;

            t.ScopeDist = Mathf.Clamp(EditorGUILayout.FloatField("Scope distance:", t.ScopeDist), 0.05f, float.MaxValue);
            t.BarrelPos = EditorGUILayout.Vector3Field("Barrel Position:", t.BarrelPos);
            t.TextureSize = Mathf.Clamp((EditorGUILayout.IntField("Texture size:", t.TextureSize) / 2) * 2, 128, 2048);


            EditorGUILayout.BeginHorizontal();
            ShowZeroList = EditorGUILayout.Foldout(ShowZeroList, "Scope Zeroings");
            if (GUILayout.Button("+"))
            {
                t.ZeroingDist.Add(150);
                ShowZeroList = true;
            }
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel++;
            if (ShowZeroList)
            {
                for (int i = 0; i < t.ZeroingDist.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    t.ZeroingDist[i] = Mathf.Clamp(EditorGUILayout.FloatField("Zero distance " + i.ToString() + " :", t.ZeroingDist[i]), 10, float.MaxValue);
                    if (GUILayout.Button("-"))
                    {
                        t.ZeroingDist.RemoveAt(i);
                        i--;
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }

            if (t.ZeroingDist.Count > 0)
            {
                if (GUILayout.Button("Generate Falloff Texture"))
                {
                    GenerateFalloffTexture();
                }
            }


            if (GUI.changed)
            {
                SceneView.RepaintAll();
            }
        }


        void OnEnable()
        {
            t = (FalloffMapGenerator)target;
            Settings = BulletHandler.instance.GetSettings();
        }

        public void GenerateFalloffTexture()
        {
            if (Settings != null)
            {
                if (!Settings.useBulletdrop)
                {
                    Debug.LogAssertion("Bulletdrop is not enabled! Check 'Ballistic Settings' window.");
                }
                else
                {
                    if (t.ZeroingDist.Count > 0)
                    {
                        t.ZeroingDist.Sort();
                        int texSize = t.TextureSize;
                        Texture2D falloffmap = new Texture2D(texSize, texSize);
                        falloffmap.wrapMode = TextureWrapMode.Clamp;
                        List<float> drops = new List<float>(t.ZeroingDist.Count);

                        for (int i = 0; i < t.ZeroingDist.Count; i++)
                        {
                            drops.Add(0);

                            drops[i] = t.TargetWeapon.calculateBulletdrop(t.ZeroingDist[i], Settings.useBulletdrag, Settings.AirDensity);

                            //scope height above barrel
                            drops[i] -= t.BarrelPos.y;

                            //Zeroing Dot Position
                            drops[i] = Mathf.Abs(drops[i]) * (t.ScopeDist / t.ZeroingDist[i]);
                        }

                        drops.Sort();
                        float Max = drops[drops.Count - 1];
                        int y = texSize / 2;
                        Color[] c = new Color[texSize * texSize];
                        Color bg = new Color(0, 0, 0, 0);
                        Color line = Color.red;
                        for (int i = 0; i < c.Length; i++)
                        {
                            c[i] = bg;
                        }

                        for (int i = 0; i < texSize; i++)
                        {
                            c[texSize / 2 + i * texSize] = line;
                            c[texSize / 2 * texSize + i] = line;
                        }
                        falloffmap.SetPixels(c);

                        for (int i = 0; i < drops.Count; i++)
                        {
                            y = texSize / 2 - (int)((drops[i] / Max) * texSize / 2);
                            for (int x = -texSize / 10; x < texSize / 10; x++)
                            {
                                falloffmap.SetPixel(texSize / 2 + x, y, line);
                            }
                        }
                        falloffmap.Apply();

                        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
                        if (path == "")
                        {
                            path = "Assets";
                        }
                        else if (System.IO.Path.GetExtension(path) != "")
                        {
                            path = path.Replace(System.IO.Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
                        }
                        path = AssetDatabase.GenerateUniqueAssetPath(path + "/" + t.TargetWeapon.name + "_zeroingMap.png");

                        File.WriteAllBytes(path, falloffmap.EncodeToPNG());
                        AssetDatabase.Refresh(ImportAssetOptions.Default);
                        EditorUtility.FocusProjectWindow();
                        EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<Texture>(path));

                        Transform FalloffTexPlane = t.transform.Find("FalloffTex");
                        if (FalloffTexPlane != null)
                        {
                            FalloffTexPlane.GetComponent<Renderer>().sharedMaterial.mainTexture = falloffmap;
                            FalloffTexPlane.localPosition = new Vector3(0, 0, t.ScopeDist);
                            FalloffTexPlane.localScale = new Vector3(Max * 2, Max * 2, Max * 2);
                        }
                        else
                        {
                            FalloffTexPlane = GameObject.CreatePrimitive(PrimitiveType.Quad).transform;
                            FalloffTexPlane.name = "FalloffTex";
                            FalloffTexPlane.SetParent(t.transform);
                            FalloffTexPlane.localRotation = Quaternion.Euler(0, 0, 0);
                            FalloffTexPlane.localPosition = new Vector3(0, 0, t.ScopeDist);
                            Material m = new Material(Shader.Find("Unlit/Transparent"));
                            m.mainTexture = falloffmap;
                            FalloffTexPlane.GetComponent<Renderer>().sharedMaterial = m;
                            FalloffTexPlane.localScale = new Vector3(Max * 2, Max * 2, Max * 2);
                            DestroyImmediate(FalloffTexPlane.GetComponent<Collider>());
                        }
                    }
                    else
                    {
                        Debug.LogAssertion("No set Scopezeroing Distances...");
                    }
                }
            }
            else
            {
                Settings = BulletHandler.instance.GetSettings();
                EditorGUILayout.LabelField("No Ballistic Settings found! Add BulletHandler with settings file to the scene!");
            }
        }
    }
}