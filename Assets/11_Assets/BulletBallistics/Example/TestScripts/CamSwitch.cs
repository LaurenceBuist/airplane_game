using UnityEngine;
using System.Collections;

namespace Ballistics
{
    /// <summary>
    /// switch to 'OverCam'
    /// </summary>
    public class CamSwitch : MonoBehaviour
    {

        public Camera OverCam;
        private bool isOverCam = false;

        void Update()
        {
            //switch cams
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                isOverCam = !isOverCam;
                OverCam.enabled = isOverCam;
            }
        }
    }
}