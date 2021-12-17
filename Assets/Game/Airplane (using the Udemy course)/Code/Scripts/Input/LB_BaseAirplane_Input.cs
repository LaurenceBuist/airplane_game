using UnityEngine;
using Slider = UnityEngine.UI.Slider;
using Photon.Bolt;

namespace LaurenceBuist
{
    public class LB_BaseAirplane_Input : EntityBehaviour<IPlaneState>
    {
        #region Variables
        protected float pitch = 0f;    //Pitch up and down
        protected float roll = 0f;    //Roll airplane
        protected float yaw = 0f;    //Turn with rudder
        protected float throttle = 0f;

        [SerializeField]
        protected float brake = 0f;

        [SerializeField]
        protected bool cameraSwitch = false;

        [Header("Flaps Properties")]
        public int maxFlapIncrements = 2;
        protected int flaps = 0;

        [Header("Mobile Inputs")]
        public DynamicJoystick L_Thumbstick;
        public Slider throttleSlider;
        public Slider flapsSlider;
        public FixedJoystick rudderJoystick;
        
        [Header("Mobile Gyro")]
        public float gyroSensitivity = 2f;
        public bool gyroON = false;
        protected Vector3 gyroRotIni;
        
        public GameObject HUB;
        #endregion
        
        #region Properties
        public float Pitch => pitch;
        public float Roll => roll;
        public float Yaw => yaw;
        public float Throttle => throttle;
        public int Flaps => flaps;
        
        private float normalizedFlaps;
        public float NormalizedFlaps => (float)flaps / maxFlapIncrements;

        public float Brake => brake;
        //public bool CameraSwitch => cameraSwitch;
        public bool CameraSwitch
        {
            get => cameraSwitch;
            set => cameraSwitch = value;
        }

        #endregion

        public override void Attached()
        {
            if (gyroON) setGyro();
            
            if (entity.IsOwner) HUB.SetActive(true);
        }

        public override void SimulateOwner()
        {
            if (entity.IsOwner)
            {
                HandleInput();
                ClampInputs();
            }
        }

        #region Custom Methods

        protected virtual void HandleInput()
        {
            if (L_Thumbstick.enabled)
            { 
                pitch = L_Thumbstick.Vertical; 
                roll = L_Thumbstick.Horizontal;
            }
            else
            {
                pitch = gyroSensitivity * Mathf.Round((Input.acceleration.y - gyroRotIni.y) * 100) /
                        100; //gyro.rotationRateUnbiased.x;
                roll = gyroSensitivity * Mathf.Round((Input.acceleration.x - gyroRotIni.x) * 100) /
                       100; //gyro.rotationRateUnbiased.y;
            }

            yaw = rudderJoystick.Horizontal;
            throttle = throttleSlider.value;
        }

        protected void ClampInputs()
        {
            pitch = Mathf.Clamp(pitch, -1f, 1f);
            roll = Mathf.Clamp(roll, -1f, 1f);
            yaw = Mathf.Clamp(yaw, -1f, 1f);
            brake = Mathf.Clamp(brake, 0f, 1f);
            
            flaps = Mathf.Clamp(flaps, 0, maxFlapIncrements);
        }

        public void setGyro()
        {
            L_Thumbstick.enabled = false;
            Input.gyro.enabled = true;
            gyroRotIni = Input.acceleration;
        }
        
        public void SetBrake(float aValue)
        {
            brake = aValue;
        }

        public void SetFlaps()
        {
            flaps = (int)flapsSlider.value;
        }

        public void SetCamera(bool aFlag)
        {
            cameraSwitch = true;
        }
        #endregion
    }
}

