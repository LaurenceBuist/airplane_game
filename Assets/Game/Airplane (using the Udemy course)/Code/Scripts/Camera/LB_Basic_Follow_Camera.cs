using System.Collections;
using UnityEngine;

namespace LaurenceBuist
{
    public class LB_Basic_Follow_Camera : MonoBehaviour
    {
        #region Variables
        [Header("Basic Follow Camera Properties")]
        //public Transform target;
        public float distance = 10f;
        public float height = 5f;
        public float smoothSpeed = 0.5f;
        public float addHeight = 3f;

        private Vector3 smoothVelocity;
        protected float origHeight;
        
        
        [SerializeField]
        private bool followOnStart = false;
        
        Transform cameraTransform;
        
        bool isFollowing;
        #endregion

        private void Start()
        {
            origHeight = height;
            if (followOnStart)
            {
                OnStartFollowing();
            }
        }

        private void FixedUpdate()
        {
            //if(target) HandleCamera();
            if(isFollowing) HandleCamera();
        }

        #region Cusotom Methods
        protected virtual void HandleCamera()
        {
            
            //Vector3 wantedPosition = target.position + (-target.forward * distance) + (Vector3.up*height);
            Vector3 wantedPosition = transform.position + (-transform.forward * distance) + (Vector3.up*height);
            
            //transform.position = Vector3.SmoothDamp(transform.position, wantedPosition, ref smoothVelocity, smoothSpeed);
            cameraTransform.position = Vector3.SmoothDamp(cameraTransform.position, wantedPosition, ref smoothVelocity, smoothSpeed);
            //Vector3 newTarget = target.position;
            Vector3 newTarget = transform.position;
            newTarget.y += addHeight;
            //transform.LookAt(newTarget);
            cameraTransform.LookAt(newTarget);
        }

        public void OnStartFollowing()
        {
            cameraTransform = Camera.main.transform;
            isFollowing = true;
        }
        #endregion
    }
}