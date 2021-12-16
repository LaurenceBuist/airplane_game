using System;
using UnityEditor;
using UnityEngine;

namespace LaurenceBuist
{
    public class LB_AirplaneSetup_Window : EditorWindow
    {
        #region Variables
        private string wantedName;
        #endregion

        #region BuiltIn Methods
        public static void LaunchSetupWindow()
        {
            LB_AirplaneSetup_Window.GetWindow(typeof(LB_AirplaneSetup_Window), true,
                    "Airplane Setup").Show();
        }

        private void OnGUI()
        {
            wantedName = EditorGUILayout.TextField("Airplane Name:", wantedName);
            if (GUILayout.Button("Create new Airplane"))
            {
                LB_Airplane_SetupTools.BuildDefaultAirplane(wantedName);
                LB_AirplaneSetup_Window.GetWindow<LB_AirplaneSetup_Window>().Close();
            }
        }

        #endregion
    }
}