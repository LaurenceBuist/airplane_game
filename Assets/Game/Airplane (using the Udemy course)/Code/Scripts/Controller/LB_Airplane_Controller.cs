using System.Collections.Generic;
using UnityEngine;

namespace LaurenceBuist
{
    [RequireComponent(typeof(LB_Airplane_Characteristics))]
    public class LB_Airplane_Controller : LB_BaseRigidBody_Controller
    {
        #region Variables

        [Header("Base Airplane Properties")]
        public LB_Airplane_Preset airplanePreset;
        public LB_BaseAirplane_Input input;
        public LB_Airplane_Characteristics characteristics;
        public Transform centerOfGravity;

        [Tooltip("Weight is in LBS")]
        public float airplaneWeight = 363f;

        [Header("Engines")]
        public List<LB_Airplane_Engines> engines = new List<LB_Airplane_Engines>();

        [Header("Wheels")]
        public List<LB_Airplane_Wheel> wheels = new List<LB_Airplane_Wheel>();
        
        [Header("Control Surfaces")]
        public List<LB_Airplane_ControlSurface> controlSurfaces = new List<LB_Airplane_ControlSurface>();
        #endregion

        #region Properties
        private float currentMSL;
        public float CurrentMsl => currentMSL;

        private float currentAGL;
        public float CurrentAgl => currentAGL;
        #endregion
        
        #region Constants
        private const float metersToFeet = 3.28084f;
        #endregion

        public override void WhenStart()
        {
            //GetPresetInfo();
            base.WhenStart();

            if (rb)
            {
                rb.mass = airplaneWeight;
                if (centerOfGravity) rb.centerOfMass = centerOfGravity.localPosition;
                characteristics = GetComponent<LB_Airplane_Characteristics>();
                if(characteristics) characteristics.InitCharacteristics(rb, input);
            }

            if (wheels != null && wheels.Count > 0)
            {
                foreach (LB_Airplane_Wheel wheel in wheels)
                {
                    wheel.InitWheel();
                }
                
            }
        }

        #region Custom Methods
        protected override void HandlePhysics()
        {
           if (input)
           {
                HandleEngines();
                HandleCharacteristics();
                HandleControlSurfaces();
                HandleWheel();
                HandleAltitude();
           }
        }

        void HandleEngines()
        {
            if (engines != null && engines.Count > 0)
            {
                foreach (LB_Airplane_Engines engine in engines)
                {
                    rb.AddForce(engine.CalculatedForce(input.throttleSlider.value));    //_throttle
                }
            }
        }

        void HandleCharacteristics()
        {
            if (characteristics)characteristics.UpdateCharacteristics();    //_flaps, _normalizedFlaps, _yaw, _roll, _pitch
        }

        void HandleControlSurfaces()
        {
            if (controlSurfaces.Count > 0)
            {
                foreach (LB_Airplane_ControlSurface controlSurface in controlSurfaces)
                {
                    controlSurface.HandleControlSurface(input);
                }
            }
        }

        void HandleWheel()
        {
            if (wheels.Count > 0)
            {
                foreach (LB_Airplane_Wheel wheel in wheels)
                {
                    wheel.HandleWheel(input);    //_brake, _yaw
                }
            }
        }

        void HandleAltitude()
        {
            currentMSL = transform.position.y * metersToFeet;

            RaycastHit hit;
            if(Physics.Raycast(transform.position, Vector3.down, out hit))
            {
                if (hit.transform.tag == "ground")
                {
                    currentAGL = (transform.position.y - hit.point.y) * metersToFeet;
                }
            }
        }

        void GetPresetInfo()
        {
            if (airplanePreset)
            {
                airplaneWeight = airplanePreset.airplaneWeight;
                if(centerOfGravity) centerOfGravity.localPosition = airplanePreset.cogPosition;

                if (characteristics)
                {
                    characteristics.dragFactor = airplanePreset.dragFactor;
                    characteristics.flapDragFactor = airplanePreset.flapDragFactor;
                    characteristics.liftCurve = airplanePreset.liftCurve;
                    characteristics.maxLiftPower = airplanePreset.maxLiftPower;
                    characteristics.maxKPH = airplanePreset.maxKPH;
                    characteristics.rollSpeed = airplanePreset.rollSpeed;
                    characteristics.yawSpeed = airplanePreset.yawSpeed;
                    characteristics.pitchSpeed = airplanePreset.pitchSpeed;
                    characteristics.rbLerpSpeed = airplanePreset.rbLerpSpeed;
                }
            }
        }

        #endregion
    }
}
