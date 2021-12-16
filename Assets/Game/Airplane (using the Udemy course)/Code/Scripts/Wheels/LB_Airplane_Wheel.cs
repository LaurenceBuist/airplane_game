using UnityEngine;

namespace LaurenceBuist
{
    [RequireComponent(typeof(WheelCollider))]
    public class LB_Airplane_Wheel : MonoBehaviour
    {
        #region Variables

        [Header("Wheel Properties")] 
        public Transform wheelGraphic;
        public bool isBraking = false;
        public float brakePower = 5f;
        public bool isSteering = false;
        public float steerAngle = 20f;
        public float steerSmoothSpeed = 8f;

        private WheelCollider WheelCol;
        private Vector3 worldPos;
        private Quaternion worldRot;
        private float finalBrakeForce;
        private float finalSteerAngle;
        #endregion

        private void Start()
        {
            WheelCol = GetComponent<WheelCollider>();
        }

        #region Custom Methods
        public void InitWheel()
        {
            if (WheelCol) WheelCol.motorTorque = 0.00000000000000001f;
        }

        public void HandleWheel(LB_BaseAirplane_Input input)    //float _brake, float _yaw
        {
            if (WheelCol)
            {
                WheelCol.GetWorldPose(out worldPos, out worldRot);
                if (wheelGraphic)
                {
                    wheelGraphic.rotation = worldRot;
                    wheelGraphic.position = worldPos;
                }

                if (isBraking)
                {
                    if (input.Brake > 0.1f)    //_brake
                    {
                        finalBrakeForce = Mathf.Lerp(finalBrakeForce, input.Brake * brakePower, Time.deltaTime); //_brake 
                        WheelCol.brakeTorque = finalBrakeForce;
                    }
                    else
                    {
                        finalBrakeForce = 0f;
                        WheelCol.brakeTorque = 0f;
                        WheelCol.motorTorque = 0.00000000000000001f;
                    }
                }

                if (isSteering)
                {
                    finalSteerAngle = Mathf.Lerp(finalSteerAngle,  -input.Yaw * steerAngle, Time.deltaTime * steerSmoothSpeed); // _yaw
                    WheelCol.steerAngle = finalSteerAngle;
                }
            }
        }
        #endregion
    }
}