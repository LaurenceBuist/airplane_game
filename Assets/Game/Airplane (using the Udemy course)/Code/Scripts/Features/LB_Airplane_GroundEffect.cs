using System;
using UnityEngine;

namespace LaurenceBuist
{
    public class LB_Airplane_GroundEffect : MonoBehaviour
    {
        #region Variables
        public float maxGroundDistance = 3f;
        public float liftForce = 100f;
        public float maxSpeed = 15f;

        [Header("Enable Ground Controls")]
        public GameObject brakeBTN;
        public bool brakeON = true;
        
        private Rigidbody rb;
        #endregion
        
        // Start is called before the first frame update
        void Start()
        {
            rb = GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (rb)
            {
                HandleGroundEffect();
            }
        }

        #region Custom Methods
        protected virtual void HandleGroundEffect()
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit))
            {
                if (hit.transform.CompareTag("ground") && hit.distance < maxGroundDistance)
                {
                    float currentSpeed = rb.velocity.magnitude;
                    float normalizedSpeed = currentSpeed / maxSpeed;
                    normalizedSpeed = Mathf.Clamp01(normalizedSpeed);
                    
                    float distance = maxGroundDistance - hit.distance;
                    float finalForce = liftForce * distance * normalizedSpeed;
                    rb.AddForce(Vector3.up * finalForce);
                    //Debug.Log("Ground Effect");
                    if (!brakeON && hit.distance < 1.7f)
                    {
                        brakeBTN.SetActive(true);
                        brakeON = true;
                    }
                }
                else
                {
                    if(brakeON) brakeBTN.SetActive(false);
                    brakeON = false;
                }
            }
        }
        #endregion
    }
}