using UnityEngine;
using System.Collections;

namespace Ballistics
{
    public class DefaultSpreadController : SpreadController
    {
        /// <summary>
        /// (x=0) -> start spread ; (x=1) -> max spread (in degree)
        /// </summary>
        [Tooltip("0 < x < 1  ;  0 < y < max spread angle (degree)")]
        public AnimationCurve SpreadAngleCurve = new AnimationCurve();

        /// <summary>
        /// Time to go from start- to max-spread
        /// </summary>
        [Tooltip("Time shooting until full spread is reached")]
        public float AttackTime;

        /// <summary>
        /// Time it takes until spread decreases again after shooting
        /// </summary>
        [Tooltip("Time until spread decreases after shooting")]
        public float HoldTime;

        /// <summary>
        /// Time to go from max- to start-spread
        /// </summary>
        [Tooltip("Recovertime until spread is back to the minimum")]
        public float RecoverTime;

        /// <summary>
        /// between 0-1; is the current "time" at the SpreadAngleCurve
        /// </summary>
        [HideInInspector]
        public float currentSpread = 0;

        private float shootTimer = 0;

        [HideInInspector]
        private float baseSpread;

        /// <summary>
        /// Calculations for defaultSpreadModel
        /// </summary>
        void SpreadCalc()
        {
            currentSpread = Mathf.Clamp01(currentSpread + Time.deltaTime / (shootTimer > 0 ? Mathf.Max(AttackTime, 0.00001f) : -Mathf.Max(RecoverTime, .00001f)));
            shootTimer -= Time.deltaTime;
        }

        void Update()
        {
            SpreadCalc();
        }

        /// <summary>
        /// called from the weapon when it shoots
        /// </summary>
        public override void onShoot()
        {
            shootTimer = HoldTime;
        }

        public override Vector3 GetCurrentSpread(Transform spawn)
        {
            float spread = SpreadAngleCurve.Evaluate(currentSpread) / 2 + baseSpread;
            return (Quaternion.AngleAxis(Random.Range(0, 360), spawn.forward) * (Quaternion.AngleAxis(Random.Range(0, spread), spawn.right)) * spawn.forward);
        }

        public override float GetSpreadAngle()
        {
            return SpreadAngleCurve.Evaluate(currentSpread);
        }

        public void setBaseSpread(float spread)
        {
            baseSpread = spread;
        }
    }
}
