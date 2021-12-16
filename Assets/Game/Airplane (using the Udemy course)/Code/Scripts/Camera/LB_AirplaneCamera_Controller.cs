using System.Collections.Generic;
using UnityEngine;

namespace LaurenceBuist
{
    public class LB_AirplaneCamera_Controller : MonoBehaviour
    {
        #region Variables

        [Header("Camera Controller Properties")]
        public LB_BaseAirplane_Input input;
        public int startCameraIndex = 0;
        public List<Camera> cameras;// = new List<Camera>();


        [Header("Camera Change")]
        public GameObject sight;
        public float camera0Height;
        public float camera1Height;
        
            
        private int cameraIndex = 0;
        #endregion
        // Start is called before the first frame update
        void Start()
        {
            cameras.Insert(0, GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>());
            if (startCameraIndex >= 0 && startCameraIndex < cameras.Count)
            {
                DisableAllCameras();
                cameras[startCameraIndex].enabled = true;
                cameras[startCameraIndex].GetComponent<AudioListener>().enabled = true;
            }
        }

        // Update is called once per frame
        void Update()
        {
            //if (input)
            //{
            if (input.CameraSwitch)
                {
                    SwitchCamera();
                }
                
            //}
        }

        #region Custom Methods

        protected virtual void SwitchCamera()
        {
            if (cameras.Count > 0)
            {
                DisableAllCameras();
                
                //Increment camera index
                cameraIndex++;
                
                //Wrap Index
                if (cameraIndex >= cameras.Count) cameraIndex = 0;

                cameras[cameraIndex].enabled = true;
                cameras[cameraIndex].GetComponent<AudioListener>().enabled = true;

                Vector3 moveUP = sight.transform.localPosition;

                if (cameraIndex == 0)
                {
                    sight.SetActive(false);
                    moveUP.y = camera0Height;
                }
                else {
                    sight.SetActive(true);
                    moveUP.y = camera1Height;
                }

                sight.transform.localPosition = moveUP;
            }

            input.CameraSwitch = false;
        }

        void DisableAllCameras()
        {
            if (cameras.Count > 0)
            {
                foreach (Camera cam in cameras)
                {
                    cam.enabled = false;
                    cam.GetComponent<AudioListener>().enabled = false;
                }
            }
        }
        #endregion
    }
}