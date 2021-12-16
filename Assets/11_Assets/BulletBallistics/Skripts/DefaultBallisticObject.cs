using UnityEngine;

namespace Ballistics
{
    /// <summary>
    /// Applys force and damage on impact
    /// </summary>
    public class DefaultBallisticObject : BallisticObject
    {
        private Rigidbody myRigidbody;
        private LivingEntity myLivingEntity;
        private LivingEntityCollider myLECollider;
        private bool hasRigidbody, hasLivingEntity;

        private void Awake()
        {
            //is static object?
            if (gameObject.isStatic)
            {
                return;
            }

            //cache rigidbody and livingentity collider if attached
            //myLECollider = GetComponent<LivingEntityCollider>();
            myLivingEntity = GetComponent<LivingEntity>();//myLECollider != null ? myLECollider.ParentLivingEntity : null;
            hasLivingEntity = myLivingEntity != null;

            myRigidbody = GetComponent<Rigidbody>();
            hasRigidbody = myRigidbody != null;
        }

        public override void BulletImpact(HitInfo hitInfo)
        {
            OnHit.Invoke(hitInfo);

            if (hasLivingEntity)
            {
                float damage = (Mathf.Pow(hitInfo.deltaSpeed, 2f) / Mathf.Pow(hitInfo.bulletInfo.MaxSpeed, 2f)) *
                               hitInfo.bulletInfo.MuzzleDamage;  // * myLECollider.DamageMultiplier;

                //damage dependent on bullet speed
                myLivingEntity.Health -= damage;
                    
                if(myLivingEntity.Health <= 0) Destroy(this);
            }

            //apply force
            if (hasRigidbody)
            {
                myRigidbody.AddForceAtPosition(hitInfo.bulletDirection * hitInfo.bulletInfo.Mass * hitInfo.deltaSpeed, hitInfo.rayHit.point, ForceMode.Impulse);
            }

            if (hitInfo.materialData.impactObject != null)
            {
                //get instance of impactObject
                PoolingObject pObj = PoolManager.instance.Get(hitInfo.materialData.impactObject.gameObject);

                Transform impact = pObj.transform;

                impact.SetParent(hitInfo.rayHit.transform, false);

                impact.gameObject.SetActive(true);

                impact.position = hitInfo.rayHit.point;

                ImpactObject myImpact = impact.GetComponent<ImpactObject>();
                if (myImpact != null)
                {
                    myImpact.Hit(hitInfo.materialData, hitInfo.rayHit);
                }
            }
        }
    }
}
