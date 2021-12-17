using UnityEngine;
using Photon.Bolt;

namespace LaurenceBuist
{
    /**
     * This script makes the camera follow an object.
     */
    public class LB_Camera_AuthorativeMovement : BoltSingletonPrefab<LB_Camera_AuthorativeMovement>
    {
        #region Variables

        [Header("Basic Follow Camera Properties")]
        //public Transform target;
        public float distance = 10f;

        public Transform _target;
        
        public float height = 5f;
        public float smoothSpeed = 0.5f;
        public float addHeight = 3f;

        private Vector3 smoothVelocity;
        protected float origHeight;


        [SerializeField] private bool followOnStart = false;

        bool isFollowing;

        #endregion

        
        void Awake ()
        {
            //DontDestroyOnLoad (gameObject);
        }
        
        
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
            if (isFollowing) HandleCamera();
        }

        #region Custom Methods

        protected virtual void HandleCamera()
        {

            //Vector3 wantedPosition = target.position + (-target.forward * distance) + (Vector3.up*height);
            Vector3 wantedPosition = _target.position + (-_target.forward * distance) + (Vector3.up * height);

            //transform.position = Vector3.SmoothDamp(transform.position, wantedPosition, ref smoothVelocity, smoothSpeed);
            transform.position =
                Vector3.SmoothDamp(transform.position, wantedPosition, ref smoothVelocity, smoothSpeed);
            //Vector3 newTarget = target.position;
            Vector3 newTarget = _target.position;
            newTarget.y += addHeight;
            transform.LookAt(newTarget);
        }

        public void OnStartFollowing()
        {
            isFollowing = true;
        }
        
        #endregion
        
        public void SetTarget (BoltEntity entity)
        {
            _target = entity.transform;
        }
    }
}