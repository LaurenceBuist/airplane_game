using UnityEngine;

namespace Ballistics
{
    public class BallisticObject : MonoBehaviour
    {
        [HideInInspector]
        public int MatID;

        [HideInInspector][SerializeField]
        public OnHitEvent OnHit;
        
        /// <summary>
        /// Bullet Impact with this object
        /// </summary>
        /// <param name="hitInfo">All the relevant information for the impact</param>
        public virtual void BulletImpact(HitInfo hitInfo)
        {
            OnHit.Invoke(hitInfo);
        }
    }

    [System.Serializable]
    public class OnHitEvent : UnityEngine.Events.UnityEvent<HitInfo> { }

    [System.Serializable]
    public struct HitInfo
    {
        /// <summary>
        /// Raycasthit of the impact
        /// </summary>
        public RaycastHit rayHit;
        /// <summary>
        /// traveldirection of the bullet
        /// </summary>
        public Vector3 bulletDirection;
        /// <summary>
        /// speed lost in the impact
        /// </summary>
        public float deltaSpeed;
        /// <summary>
        /// penetrationdepth of the bullet in the material
        /// </summary>
        public float penetrationDepth;
        /// <summary>
        /// Physical data of the bullet
        /// </summary>
        public BulletInfo bulletInfo;
        /// <summary>
        /// Data of the hit material
        /// </summary>
        public MaterialData materialData;

        public HitInfo(RaycastHit rayHit, Vector3 bulletDirection, float deltaSpeed, float penetrationDepth, BulletInfo bulletInfo, MaterialData materialData)
        {
            this.rayHit = rayHit;
            this.bulletDirection = bulletDirection;
            this.deltaSpeed = deltaSpeed;
            this.penetrationDepth = penetrationDepth;
            this.bulletInfo = bulletInfo;
            this.materialData = materialData;
        }
    }
}
