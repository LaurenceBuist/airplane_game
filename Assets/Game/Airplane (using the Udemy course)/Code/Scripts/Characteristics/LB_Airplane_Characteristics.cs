using UnityEngine;

namespace LaurenceBuist
{
    public class LB_Airplane_Characteristics : MonoBehaviour
    {
        #region Variables

        public bool shotDown = false;

        [Header("Characteristics Properties")]
        private float forwardSpeed;
        public float ForwardSpeed => forwardSpeed;
        public float kph;
        public float KPH => kph;
        public float maxKPH = 177f;
        public float rbLerpSpeed = 0.01f;
        
        [Header("Pussy Flying")]
        public bool pussyFlying;
        //public LB_Airplane_GroundEffect GroundEffect;
        //public GameObject brakeButton;

        [Header("Lift Properties")] 
        public float maxLiftPower = 50f;
        public AnimationCurve liftCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        public float flapLiftPower = 100f;

        [Header("Drag Properties")]
        public float dragFactor = 0.0004f;
        public float flapDragFactor = 0.0002f;

        [Header("Control Properties")] 
        public float pitchSpeed = 1500f;
        //public float gyroPitchSpeed;
        public float rollSpeed = 1500f;
        //public float gyroRollSpeed = 0.5f;
        public float yawSpeed = 1500f;
        public AnimationCurve controlSurfaceEfficiency = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        
        [Header("Other Properties")]
        public float finalRollAngle;
        public float finalPitchAngle;
        
        
        private LB_BaseAirplane_Input input;
        private Rigidbody rb;
        private float startDrag;
        private float startAngularDrag;

        private float maxMPS;
        private float normalizedKPH;

        private float angleOfAttack;
        private float pitchAngle;
        private float rollAngle;

        private float csEfficiencyValue;
        
        #endregion

        #region Constants

        private const float mpsToKph = 3.6f;

        #endregion

        #region Custom Methods    

        public void InitCharacteristics(Rigidbody curRB, LB_BaseAirplane_Input curInput)
        {
            input = curInput;
            rb = curRB;
            startDrag = rb.drag;
            startAngularDrag = rb.angularDrag;

            maxMPS = maxKPH / mpsToKph;

            pussyFlying = (PlayerPrefs.GetFloat("controls")==0f);
            
            
        }

        public void UpdateCharacteristics()    //int _flaps, float _normalizedFlaps, float _yaw, float _roll, float _pitch
        {
            if (rb && !shotDown)
            {
                CalculateForwardSpeed();
                CalculateLift();        //_normalizedFlaps
                CalculateDrag();       //_flaps

                HandleControlSurfaceEfficiency();
                HandlePitch();    //_pitch
                HandleRoll();    //_roll
                HandleYaw();    //_yaw
                HandleBanking();

                HandleRigidbodyTransform();
            }
        }

        //Get Forward Speed and convert it to kph
        void CalculateForwardSpeed()
        {
            Vector3 localVelocity = transform.InverseTransformDirection(rb.velocity);
            forwardSpeed = Mathf.Max(0f, localVelocity.z);
            forwardSpeed = Mathf.Clamp(forwardSpeed, 0f, maxMPS);

            //Speed in kph
            kph = forwardSpeed * mpsToKph;
            kph = Mathf.Clamp(kph, 0f, maxKPH);
            normalizedKPH = Mathf.InverseLerp(0f, maxKPH, kph);
        }

        void CalculateLift()
        {
            //Get angle of attack
            angleOfAttack = Vector3.Dot(rb.velocity.normalized, transform.forward);
            angleOfAttack *= angleOfAttack;

            //Create lift direction
            Vector3 liftDir = transform.up;
            float liftPower = liftCurve.Evaluate(normalizedKPH) * maxLiftPower;
            
            //Add Flap Lift
            float finalLiftPower = flapLiftPower * input.NormalizedFlaps;    //normalizedFlaps;
            
            //Apply lift Force to rigidbody
            Vector3 finalLiftForce = (liftPower + finalLiftPower) * angleOfAttack * liftDir;
            rb.AddForce(finalLiftForce);
        }

        void CalculateDrag()    //int flaps
        {
            //Speed Drag
            float speedDrag = forwardSpeed * dragFactor;

            //Flap Drag
            float flapDrag = input.Flaps * flapDragFactor;    //flaps 
                        
            //add all drags together
            float finalDrag = startDrag + speedDrag + flapDrag;

            //Put drag on rigidbody
            rb.drag = finalDrag;
            rb.angularDrag = startAngularDrag * forwardSpeed;
        }

        void HandleRigidbodyTransform()
        {
            if (rb.velocity.magnitude > 1f)
            {
                Vector3 updatedVelocity = Vector3.Lerp(rb.velocity, transform.forward * forwardSpeed,
                    forwardSpeed * angleOfAttack * Time.deltaTime * rbLerpSpeed);
                rb.velocity = updatedVelocity;

                Quaternion updatedRotation = Quaternion.Slerp(rb.rotation,
                    Quaternion.LookRotation(rb.velocity.normalized, transform.up), Time.deltaTime * rbLerpSpeed);
                rb.MoveRotation(updatedRotation);
            }
        }

        void HandleControlSurfaceEfficiency()
        {
            csEfficiencyValue = controlSurfaceEfficiency.Evaluate(normalizedKPH);
        }
        
        void HandlePitch()        //float pitch
        {
            Vector3 flatForward = transform.forward;
            flatForward.y = 0f;
            flatForward = flatForward.normalized;
            pitchAngle = Vector3.Angle(transform.forward, flatForward);
            
            /*Vector3 pitchTorque = input.Pitch * pitchSpeed * csEfficiencyValue * transform.right;
            rb.AddTorque(pitchTorque);*/

            /*Vector3 localRot = transform.localRotation.eulerAngles;
            localRot.x = csEfficiencyValue / maxKPH + localRot.x;
            transform.localRotation = Quaternion.Euler(localRot);*/

            if (pussyFlying)
            {
                finalPitchAngle = Mathf.Lerp(finalPitchAngle, input.Pitch, Time.deltaTime * 0.7f); //pitch
                Vector3 localRot = transform.localRotation.eulerAngles;
                localRot.x = finalPitchAngle * csEfficiencyValue * 80 - 18 * (1 - csEfficiencyValue);
                transform.localRotation = Quaternion.Euler(localRot);
            }
            else
            {
                Vector3 pitchTorque = input.Pitch * pitchSpeed * csEfficiencyValue * transform.right; //pitch 
                rb.AddTorque(pitchTorque);
            }
        }

        void HandleRoll()    //float roll
        {
            Vector3 flatRight = transform.right;
            flatRight.y = 0f;
            flatRight = flatRight.normalized;
            rollAngle = Vector3.SignedAngle(transform.right, flatRight, transform.forward);

            /*Vector3 rollTorque = -input.Roll * rollSpeed * csEfficiencyValue * transform.forward;
            rb.AddTorque(rollTorque);*/
            
            if (pussyFlying)
            {
                finalRollAngle = Mathf.Lerp(finalRollAngle, -input.Roll, Time.deltaTime * 0.7f);    //-roll
                Vector3 localRot = transform.localRotation.eulerAngles;
                localRot.z = finalRollAngle * csEfficiencyValue * 80;
                transform.localRotation = Quaternion.Euler(localRot);
            }
            else
            {
                Vector3 rollTorque = -input.Roll * rollSpeed * csEfficiencyValue * transform.forward;    //-roll 
                rb.AddTorque(rollTorque);
            }


        }

        void HandleYaw()        //float yaw
        {
            Vector3 yawTorque = input.Yaw * yawSpeed * csEfficiencyValue * transform.up;    //yaw
            rb.AddTorque(yawTorque);
        }

        void HandleBanking()
        {
            float bankSide = Mathf.InverseLerp(-90f, 90f, rollAngle);
            float bankAmount = Mathf.Lerp(-1f, 1f, bankSide);
            Vector3 bankTorque = csEfficiencyValue * bankAmount * rollSpeed * Vector3.up; //instead of: bankAmount * rollSpeed * transform.up
            rb.AddTorque(bankTorque);
            //Debug.Log(bankTorque);
        }

        #endregion

        #region public Methods

        public void OnDeath()
        {
            shotDown = true;
        }

        #endregion

        
    }
}