using UnityEditor;
using UnityEngine;

namespace LaurenceBuist
{
    public static class LB_Airplane_SetupTools
    {
        public static void BuildDefaultAirplane(string aName)
        {
            //Create root GameObject
            GameObject rootGO = new GameObject(aName, typeof(LB_Airplane_Controller), typeof(LB_BaseAirplane_Input));
            
            //Create Center of Gravity
            GameObject cogGO = new GameObject("CenterOfGravity");
            cogGO.transform.SetParent(rootGO.transform, false);
            
            //Create the Base Components or Find them
            LB_BaseAirplane_Input input = rootGO.GetComponent<LB_BaseAirplane_Input>();
            LB_Airplane_Controller controller = rootGO.GetComponent<LB_Airplane_Controller>();
            LB_Airplane_Characteristics characteristics = rootGO.GetComponent<LB_Airplane_Characteristics>();

            //Setup the Airplane
            if (controller)
            {
                //Assign core Components
                controller.input = input;
                controller.characteristics = characteristics;
                controller.centerOfGravity = cogGO.transform;

                //Create Structure
                GameObject graphicsGrp = new GameObject("Graphics_GRP");
                GameObject collisionGrp = new GameObject("Collision_GRP");
                GameObject controlSurfaceGrp = new GameObject("ControlSurfaces_GRP");

                graphicsGrp.transform.SetParent(rootGO.transform, false);
                collisionGrp.transform.SetParent(rootGO.transform, false);
                controlSurfaceGrp.transform.SetParent(rootGO.transform, false);
                
                //Create First Engine
                GameObject engineGO = new GameObject("Engine", typeof(LB_Airplane_Engines));
                LB_Airplane_Engines engine = engineGO.GetComponent<LB_Airplane_Engines>();
                controller.engines.Add(engine);
                engineGO.transform.SetParent(rootGO.transform, false);
                
                //Create the base Airplane
                GameObject defaultAirplane = (GameObject)AssetDatabase.LoadAssetAtPath(
                    "Assets/Art/ShutOffObjects/Airplanes/IndiePixel_Airplanes/Indie-Pixel_Airplane/FighterInterceptor.fbx", typeof(GameObject));
                if (defaultAirplane) GameObject.Instantiate(defaultAirplane, graphicsGrp.transform);
            }
            
            //Select the Airplane Setup
            Selection.activeGameObject = rootGO;
        }
    }
}