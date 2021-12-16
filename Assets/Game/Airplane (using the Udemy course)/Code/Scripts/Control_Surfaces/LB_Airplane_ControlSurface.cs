using UnityEngine;

namespace LaurenceBuist
{
    public enum ControlSurfaceType
    {
        Rudder,
        Elevator,
        Flap,
        Aileron
    }
    
    public class LB_Airplane_ControlSurface : MonoBehaviour
    {
        #region Variables
        [Header("Control Surfaces Properties")]
        public ControlSurfaceType type = ControlSurfaceType.Rudder;
        public float maxAngle = 30f;
        public Vector3 axis = Vector3.right;
        public Transform controlSurfaceGraphic;
        public float smoothSpeed = 2f;

        private float wantedAngle;
        #endregion

        #region Network
        #endregion

        // Update is called once per frame
        void Update()
        {
            if (controlSurfaceGraphic)
            {
                Vector3 finalAngleAxis = axis * wantedAngle;
                controlSurfaceGraphic.localRotation = Quaternion.Slerp(controlSurfaceGraphic.localRotation,
                    Quaternion.Euler(finalAngleAxis), Time.deltaTime * smoothSpeed);
            }
        }

        #region Custom Methods

        public void HandleControlSurface(LB_BaseAirplane_Input input)        //float _yaw, float _pitch, float _flaps, float _roll
        {
            float inputValue = 0f;
            switch (type)
            {
                case ControlSurfaceType.Rudder:
                    inputValue = input.Yaw;    //_yaw;
                    break;
                
                case ControlSurfaceType.Elevator:
                    inputValue = input.Pitch;    //_pitch;
                    break;
                
                case ControlSurfaceType.Flap:
                    inputValue = input.Flaps;    //_flaps;
                    break;
                
                case ControlSurfaceType.Aileron:
                    inputValue = -input.Roll;    //_roll;
                    break;
                
                default:
                    break;
            }

            wantedAngle = maxAngle * inputValue;
        }
        
        #endregion
    }
}