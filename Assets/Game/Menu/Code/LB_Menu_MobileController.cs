using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace LaurenceBuist
{
    public class LB_Menu_MobileController : MonoBehaviour
    {
        #region Variables
        public GameObject MainCamera;

        #region Settings
        [Header("Settings")]
        public Slider controlsSlider;
        public GameObject proButton;
        public Slider qualitySlider;
        public Toggle fogToggle;
        public Slider timeSlider;
        #endregion

        #region Planes

        [Header("Planes")]
        public bool changeCameraView_Planes = false;
        private bool changingCameraView_Planes = false;
        private Vector3 ogRotation;      // The original rotation of the camera
        private Vector3 ogPosition;      // The original position of the camera
        private Vector3 endPosition_Planes = new Vector3(0f, 3.61f, 5.2f);      // The position of the camera when the planes slot is active
        private Vector3 endRotation_Planes = new Vector3(9.5f, -4.34f, 0);      // The rotation of the camera when the planes slot is active
        private Vector3 endPos; // The position we want to end up in
        private Vector3 endRot; // The rotation we want to end up in
        public float rotationSpeed;     // The speed of the slerp and lerp

        public Image Background_Black;
        
        #endregion

        #region Multiplayer
        [Header("Multiplayer")]
        public GameObject ConnectingGRP;
        #endregion
        
        #endregion

        #region Builtin Methods

        // Start is called before the first frame update
        void Start()
        {
            //Get info of main camera
            ogPosition = MainCamera.transform.position;
            ogRotation = MainCamera.transform.eulerAngles;
            
            // Set settings to the last saved settings
            controlsSlider.value = PlayerPrefs.GetFloat("controls", 1);
            proButton.SetActive((controlsSlider.value == 1) ? false : true);
            qualitySlider.value = PlayerPrefs.GetInt("quality", 5);
            timeSlider.value = PlayerPrefs.GetInt("time", 0);
            if (PlayerPrefs.GetInt("fog", 1) == 1) fogToggle.isOn = true;
            else fogToggle.isOn = false;
            
            //Turn off spectator mode
            PlayerPrefs.SetInt("Spectator", 0);
        }

        private void Update()
        {
            //Rotate and move camera for planes
            if (changingCameraView_Planes)
            {
                // Movement
                MainCamera.transform.position = Vector3.Lerp(MainCamera.transform.position, endPos,
                    Time.deltaTime * rotationSpeed);

                // If at end position, stop lerping and slerping
                if (Math.Abs(endPos.y - MainCamera.transform.position.y) < 0.1)
                    changingCameraView_Planes = false;

                // Rotation
                MainCamera.transform.rotation = Quaternion.Slerp(MainCamera.transform.rotation,
                    Quaternion.Euler(endRot), Time.deltaTime * rotationSpeed);
            }
        }

        #endregion

        #region Custom Methods
        public void loadScene(string scenename)
        {
            SceneManager.LoadScene(scenename);
        }

        public void SpectatorMode()
        {
            PlayerPrefs.SetInt("Spectator", 1);
        }

        public void OnOffSwitch(GameObject gameObject)
        {
            if (gameObject.activeInHierarchy) gameObject.SetActive(false);
            else gameObject.SetActive(true);
        }
        
        public void SaveVar(string key, object any)
        {
            if (any is float) PlayerPrefs.SetFloat(key, (float) any);
            if (any is int) PlayerPrefs.SetInt(key, (int) any);
            if (any is string) PlayerPrefs.SetString(key, any.ToString());
        }
        #endregion

        #region Settings
        //Save_Settings new settings
        public void Save_Settings()
        {
            SaveVar("controls", controlsSlider.value);

            PlayerPrefs.SetInt("quality", (int)qualitySlider.value);
            
            PlayerPrefs.SetInt("fog", fogToggle.isOn ? 1 : 0);

            PlayerPrefs.SetInt("time", (int)timeSlider.value);
        }
        #endregion

        #region Planes
        public void ChangeCameraView_Planes()
        {
            changeCameraView_Planes = !changeCameraView_Planes;

            // Plane slot was selected
            if (changeCameraView_Planes)
            {
                endPos = endPosition_Planes;
                endRot = endRotation_Planes;
            } 
            // Plane slot was unselected
            else
            {
                endPos = ogPosition;
                endRot = ogRotation;
            }

            // Start to change the camera view
            changingCameraView_Planes = true;
        }

        public void Save_Planes()
        {
            ChangeCameraView_Planes();
        }

        public void Cancel_Planes()
        {
            ChangeCameraView_Planes();
        }
        #endregion

        #region Multiplayer

        #endregion
    }
}