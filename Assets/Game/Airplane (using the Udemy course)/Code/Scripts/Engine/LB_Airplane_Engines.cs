using UnityEngine;

namespace LaurenceBuist
{
    public class LB_Airplane_Engines : MonoBehaviour
    {
        #region Variables
        [Header("Engine Properties")]
        public float maxForce = 2000f;
        public float maxRPM = 2550f;
        public float shutOffSpeed = 1.3f;
        public AnimationCurve powerCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

        [Header("Propellers")]
        public LB_Airplane_Propeller propeller;

        private bool isShutOff = false;
        private float lastThrottleValue;
        private float finalShutOffThrottleValue;
        #endregion
        
        #region Properties

        public bool ShutEngineOff
        {
            set => isShutOff = value;
        }

        private float currentRPM;
        public float CurrentRPM => currentRPM;
        #endregion

        #region Custom Methods
        public Vector3 CalculatedForce(float throttle)
        {
            //Calculate Power
            //float finalThrottle = Mathf.Clamp01(throttle);
            float throttleNumber = Mathf.Clamp01(throttle);
            float finalThrottle = lastThrottleValue;

            if (!isShutOff)
            {
                finalThrottle = Mathf.Lerp(lastThrottleValue, throttleNumber, Time.deltaTime * 0.2f);
                //finalThrottle = powerCurve.Evaluate(finalThrottle);
                lastThrottleValue = finalThrottle;
            }
            else
            {
                lastThrottleValue -= Time.deltaTime * shutOffSpeed;
                lastThrottleValue = Mathf.Clamp01(lastThrottleValue);
                finalShutOffThrottleValue = powerCurve.Evaluate(lastThrottleValue);
                finalThrottle = finalShutOffThrottleValue;
            }

            //Calculate RPM
            //currentRPM = Mathf.Lerp((currentRPM/maxRPM), finalThrottle, Time.deltaTime * 0.2f) * maxRPM;
            currentRPM = finalThrottle * maxRPM;
            if (propeller) propeller.HandlePropeller(currentRPM);
            
            //Calculate Force
            float finalPower = finalThrottle * maxForce;
            Vector3 finalForce = transform.forward * finalPower;

            return finalForce;
        }
        #endregion
    }
}