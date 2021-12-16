using UnityEngine;

namespace LaurenceBuist
{
    public class LB_Airplane_Audio : MonoBehaviour
    {
        #region Variables

        [Header("Airplane Audio Properties")]
        public LB_BaseAirplane_Input input;
        public LB_Airplane_Engines engine;
        public AudioSource idleSource;
        public AudioSource fullThrottleSource;
        public float maxPitchValue = 1.2f;
        
        private float finalVolumeValue;
        private float finalPitchValue;

        private float pitchBeforeShutOffValue;
        private bool isShutOff = false;
        public float shutOffSpeed = 2f;
        public bool ShutEngineOff
        {
            set => isShutOff = value;
        }

        #endregion

        // Start is called before the first frame update
        void Start()
        {
            if (fullThrottleSource)
            {
                fullThrottleSource.volume = 0f;
            }
        }

        // Update is called once per frame
        void Update()
        {
            //if (input)
            //{
            HandleAudio();
            //}
        }

        #region Custom Methods

        protected virtual void HandleAudio()
        {
            if (!isShutOff)
            {
                //finalVolumeValue = Mathf.Lerp(0f, 1f, input.throttleSlider.value);
                finalVolumeValue = Mathf.Lerp(0f, 1f, engine.CurrentRPM/engine.maxRPM);
                //finalPitchValue = Mathf.Lerp(0f, maxPitchValue, input.throttleSlider.value);
                finalPitchValue = Mathf.Lerp(0f, maxPitchValue, engine.CurrentRPM/engine.maxRPM);

                if (fullThrottleSource)
                {
                    fullThrottleSource.volume = finalVolumeValue;
                    fullThrottleSource.pitch = finalPitchValue;
                    pitchBeforeShutOffValue = finalPitchValue;
                }
            }
            else
            {
                idleSource.enabled = false;
                finalVolumeValue = Mathf.Lerp(finalVolumeValue, 0f, Time.deltaTime*shutOffSpeed);
                //finalPitchValue = Mathf.Lerp(finalPitchValue, 0f, Time.deltaTime*shutOffSpeed);

                if (fullThrottleSource)
                {
                    fullThrottleSource.volume = finalVolumeValue;
                    //fullThrottleSource.pitch = finalPitchValue;
                }
            }
        }


        #endregion
    }
}