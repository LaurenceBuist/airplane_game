using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace LaurenceBuist
{
    [CustomEditor(typeof(LB_Airplane_Controller))]
    public class LB_AirplaneController_Editor : Editor
    {
        #region Variables
        private LB_Airplane_Controller targetController;
        #endregion
        
        #region BuiltIn Methods
        private void OnEnable()
        {
            targetController = (LB_Airplane_Controller) target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Space(15);
            if(GUILayout.Button("Get Airplane Components" ,GUILayout.Height(35)))
            {
                //Find All Engines
                targetController.engines.Clear();
                targetController.engines = FindAllEngines().ToList<LB_Airplane_Engines>();
                
                //Find All Wheels
                targetController.wheels.Clear();
                targetController.wheels = FindAllWheels().ToList<LB_Airplane_Wheel>();
                
                //Find All Control Surfaces
                targetController.controlSurfaces.Clear();
                targetController.controlSurfaces= FindAllControlSurfaces().ToList<LB_Airplane_ControlSurface>();
            }

            if (GUILayout.Button("Create Airplane Preset", GUILayout.Height(35)))
            {
                string filePath =
                    EditorUtility.SaveFilePanel("Save_Settings Airplane Preset", "Assets", "New_Airplane_Setup", "asset");
                SaveAirplanePreset(filePath);
            }
            GUILayout.Space(15);
        }
        #endregion
        
        #region Custom Methods
        LB_Airplane_Engines[] FindAllEngines()
        {
            LB_Airplane_Engines[] engines = new LB_Airplane_Engines[0];
            if (targetController)
                engines = targetController.transform.GetComponentsInChildren<LB_Airplane_Engines>(true);
            return engines;
        }
        
        LB_Airplane_Wheel[] FindAllWheels()
        {
            LB_Airplane_Wheel[] wheels = new LB_Airplane_Wheel[0];
            if (targetController)
                wheels = targetController.transform.GetComponentsInChildren<LB_Airplane_Wheel>(true);
            return wheels;
        }
        
        LB_Airplane_ControlSurface[] FindAllControlSurfaces()
        {
            LB_Airplane_ControlSurface[] controlSurfaces = new LB_Airplane_ControlSurface[0];
            if (targetController)
                controlSurfaces = targetController.transform.GetComponentsInChildren<LB_Airplane_ControlSurface>(true);
            return controlSurfaces;
        }

        void SaveAirplanePreset(string aPath)
        {
            if(targetController && !string.IsNullOrEmpty(aPath))
            {
                string appPath = Application.dataPath;
                string finalPath = "Assets" + aPath.Substring(appPath.Length);
                
                //Create new Preset
                LB_Airplane_Preset newPreset = ScriptableObject.CreateInstance<LB_Airplane_Preset>();
                newPreset.airplaneWeight = targetController.airplaneWeight;
                if(targetController.centerOfGravity) newPreset.cogPosition = targetController.centerOfGravity.localPosition;

                if (targetController.characteristics)
                {
                    newPreset.dragFactor = targetController.characteristics.dragFactor;
                    newPreset.flapDragFactor = targetController.characteristics.flapDragFactor;
                    newPreset.maxKPH = targetController.characteristics.maxKPH;
                    newPreset.rbLerpSpeed = targetController.characteristics.rbLerpSpeed;
                    newPreset.liftCurve = targetController.characteristics.liftCurve;
                    newPreset.maxLiftPower = targetController.characteristics.maxLiftPower;
                    newPreset.pitchSpeed = targetController.characteristics.pitchSpeed;
                    newPreset.rollSpeed = targetController.characteristics.rollSpeed;
                    newPreset.yawSpeed = targetController.characteristics.yawSpeed;
                }
                //Create Final Preset
                AssetDatabase.CreateAsset(newPreset, finalPath);
            }
        }
        #endregion
    }
}