using UnityEngine;

namespace Ballistics
{
    public abstract class SpreadController : MonoBehaviour
    {

        /// <summary>
        /// Gets a euler rotation for bullet spread
        /// </summary>
        public abstract Vector3 GetCurrentSpread(Transform spawn);

        /// <summary>
        /// get spread angle
        /// </summary>
        /// <returns></returns>
        public abstract float GetSpreadAngle();


        /// <summary>
        /// called when myWeapon shoots
        /// </summary>
        public abstract void onShoot();
    }
}
