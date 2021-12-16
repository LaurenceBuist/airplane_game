using UnityEngine;

namespace LaurenceBuist
{
    public class LB_Airplane_Gun : MonoBehaviour
    {
        [Header("Gun Properties")]
        public float damage = 10f;
        public float range = 10f;

        public GameObject impactEffect;
        
        private LB_BaseAirplane_Input input;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButton(0))
            {
                Shoot();
            }
        }

        void Shoot()
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, range))
            {
                Debug.Log(hit.point);
            }
            Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Debug.DrawRay(transform.position, transform.forward*range, Color.red);
        }
    }
}