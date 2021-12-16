using UnityEngine;

namespace LaurenceBuist
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(AudioSource))]
    public class LB_BaseRigidBody_Controller : Bolt.EntityBehaviour<IPlaneState>
    {
        #region Variables
        [Header("Offline")]
        public bool isOffline;
        
        protected Rigidbody rb;
        protected AudioSource aSource;
        
        #endregion

        // Called when the game object has been setup inside Bolt and exists on the network
        public override void Attached()
        {
            WhenStart();
            
            //AddCamera();
        }

        public virtual void WhenStart()
        {
            rb = GetComponent<Rigidbody>();
            aSource = GetComponent<AudioSource>();
            if (aSource) aSource.playOnAwake = false;
        }
        
        // Update is called once per frame
        public override void SimulateOwner()    //void FixedUpdate()
        {
            if (!BoltNetwork.IsSinglePlayer || !isOffline)
            {
                HandleNetwork();
                
            }
            
            if (rb)
            {
                HandlePhysics();
            }
        }
        
        #region Custom Methods

        

        protected virtual void HandlePhysics(){}

        protected virtual void HandleNetwork(){}


        /*void AddCamera()
        {
            LB_Basic_Follow_Camera _cameraWork = gameObject.GetComponent<LB_Basic_Follow_Camera>();
            if (_cameraWork != null)
            {
                if (entity.IsOwner || isOffline)
                {
                    _cameraWork.OnStartFollowing();
                }
            }
        }*/

        #endregion
    }
}