using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LB_Airplane_GUINameTag : MonoBehaviour
{
    [Header("Visual Settings")]
        public GUISkin guiSkin;
        public Color color = Color.white;
        public int fontSize = 14;
 
        [Header("Automatic Offset Y")]
        public float maxCameraDistance = 8f;                // Maximum camera distance
        public float minCamDistanceOffsetY = 20f;           // Minimum Offset Y applied when camera distance at minimum from player
        public float maxCamDistanceOffsetY = -30f;          // Maximum Offset Y applied when camera distance at maximum from player
        private float offsetX = 0;    //15f
        private float offsetY = 0;
 
        private float boxW = 0f;
        private Vector2 boxPosition;
 
        public CapsuleCollider _playerCollider;
        
        public string playerName;
 
        void Start()
        {
            //_playerCollider = GetComponent<CapsuleCollider>();
        }
 
        /// <summary>
        /// Sync handler for setting player name
        /// </summary>
        /// <param name="name"></param>
        void OnPlayerName(string name)
        {
            playerName = name;
        }
 
        /// <summary>
        /// Set the player name
        /// </summary>
        /// <param name="name"></param>
        public void SetPlayerName(string name)
        {
            playerName = name;
        }
 
        /// <summary>
        /// Main name tag rendering loop
        /// </summary>
        void OnGUI()
        {
            // Calculate the name tag position based on the height of the players capsule collider
            Vector3 nameTagPosition = new Vector3(transform.position.x, transform.position.y + _playerCollider.height * 1.1f, transform.position.z);
 
            // Calculate the world to viewport point in relation to the camera
            Vector3 vpPos = Camera.main.WorldToViewportPoint(nameTagPosition);
 
            // Only render when the camera see's the player on the view frustrum
            if (vpPos.x > 0 && vpPos.x < 1 && vpPos.y > 0 && vpPos.y < 1 && vpPos.z > 0)
            {
                // Calculate name tag box position from world to screen coordinates
                boxPosition = Camera.main.WorldToScreenPoint(nameTagPosition);
                boxPosition.y = Screen.height - boxPosition.y;
                boxPosition.x -= boxW;
 
                // Dyanmic name tag size calculation
                GUI.skin = guiSkin;
                Vector2 content = guiSkin.box.CalcSize(new GUIContent(playerName));
                guiSkin.box.fontSize = fontSize;
                GUI.contentColor = color;
 
                // Automatic Offset Y Calculation
                float camDist = Vector3.Distance(Camera.main.transform.position, nameTagPosition);
                offsetY = ScaleValueFloat(camDist, 0f, maxCameraDistance, minCamDistanceOffsetY, maxCamDistanceOffsetY);
 
                // Center the position of the name tag
                Rect rectPos = new Rect(boxPosition.x - content.x / 2, boxPosition.y + offsetY-1, content.x, content.y); //* offsetX    
 
                // Display the name tag
                GUI.Box(rectPos, playerName);
            }
        }
 
        /// <summary>
        /// Scales one range of values to another range of values.
        /// This function can handle inverted ScaleMin and ScaleMax as well.
        /// </summary>
        /// <param name="fValue"></param>
        /// <param name="fInputMin"></param>
        /// <param name="fInputMax"></param>
        /// <param name="fScaleMin"></param>
        /// <param name="fScaleMax"></param>
        /// <returns></returns>
        public float ScaleValueFloat(float fValue, float fInputMin, float fInputMax, float fScaleMin, float fScaleMax)
        {
            float fVal = 0;
 
            //Inputs
            float fInputRange = fInputMax - fInputMin;
 
            //Scale
            float fScaleRange = fScaleMax - fScaleMin;
 
            //Rate Per Scale
            float fRate = fScaleRange / fInputRange;
 
            //Output
            if (fValue < fInputMin)
            {
                fValue = fInputMin;
            }
            if (fValue > fInputMax)
            {
                fValue = fInputMax;
            }
            float fOut1 = (fValue - fInputMin) * fRate;
            float fOut2 = fOut1 + fScaleMin;
            fVal = fOut2;
 
            return fVal;
        }
}
