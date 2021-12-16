using UnityEngine;

namespace LaurenceBuist
{
    [CreateAssetMenu(menuName = "Airplane/Create Airplane Preset")]
    public class LB_Airplane_Preset : ScriptableObject
    {
        #region Controller Properties

        [Header("Controller Properties")]
        public Vector3 cogPosition;
        public float airplaneWeight;
        #endregion

        #region MyRegion
        [Header("Characteristics Properties")]
        public float maxKPH = 177f;
        public float rbLerpSpeed = 0.01f;
        public float maxLiftPower = 50f;
        public AnimationCurve liftCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        public float dragFactor = 0.0004f;
        public float flapDragFactor = 0.0002f;
        public float pitchSpeed = 4000f;
        public float rollSpeed = 1500f;
        public float yawSpeed = 1000f;
        #endregion
    }
}