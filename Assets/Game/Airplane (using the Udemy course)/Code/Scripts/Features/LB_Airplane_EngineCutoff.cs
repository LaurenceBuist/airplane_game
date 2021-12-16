using UnityEngine;
using UnityEngine.Events;

namespace LaurenceBuist
{
    public class LB_Airplane_EngineCutoff : MonoBehaviour
    {
        #region Variables
        [Header("Engine Cutoff Properties")]
        public KeyCode cutoffKey = KeyCode.O;
        public UnityEvent onEngineCutoff = new UnityEvent();
        #endregion

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(cutoffKey))
            {
                HandleEngineCutoff();
            }
        }

        #region Custom Methods

        void HandleEngineCutoff()
        {
            if (onEngineCutoff != null)
            {
                onEngineCutoff.Invoke();
            }
        }
        #endregion
    }
}