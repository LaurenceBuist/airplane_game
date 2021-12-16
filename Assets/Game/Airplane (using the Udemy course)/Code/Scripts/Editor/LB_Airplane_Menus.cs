using UnityEditor;

namespace LaurenceBuist
{
    
    public static class LB_Airplane_Menus
    {
        [MenuItem("Airplane Tools/Create New Airplane")]
        public static void CreateMenuAirplane()
        {
           // LB_Airplane_SetupTools.BuildDefaultAirplane("New Airplane");
           LB_AirplaneSetup_Window.LaunchSetupWindow();
        }
    }
}