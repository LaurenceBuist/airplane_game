using UnityEngine;
using System.Collections.Generic;

namespace Ballistics
{

    /// <summary>
    /// impactObject that plays a sound + particle system
    /// </summary>
    public class DefaultImpactObject : ImpactObject
    {
        public List<AudioClip> HitSounds;
        AudioSource audiosource;
        ParticleSystem particles;
        DelayedDeactivate deactivate;
        private Transform myTrans;
        public float Time = 3f;

        public float bulletHoleScale;
        private Transform bulletHole;

        void Awake()
        {
            audiosource = GetComponent<AudioSource>();
            particles = GetComponent<ParticleSystem>();
            deactivate = GetComponent<DelayedDeactivate>();
            myTrans = transform;
            bulletHole = myTrans.GetChild(0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data">Material Data of the hit object</param>
        /// <param name="rayHit">RaycastHit of the impact</param>
        public override void Hit(MaterialData data, RaycastHit rayHit)
        {
            myTrans.rotation = Quaternion.LookRotation(rayHit.normal);
            bulletHole.localScale = new Vector3(bulletHoleScale / myTrans.lossyScale.x, bulletHoleScale / myTrans.lossyScale.y, bulletHoleScale / myTrans.lossyScale.z);
            if (audiosource != null && HitSounds.Count > 0)
            {
                audiosource.clip = HitSounds[Random.Range(0, HitSounds.Count)];
                audiosource.Play();
            }

            if (particles != null)
            {
                particles.Simulate(0, true, true);
                ParticleSystem.EmissionModule module = particles.emission;
                module.enabled = true;
                particles.Play(true);
            }

            if (deactivate != null)
            {
                deactivate.Deactivate(Time);
            }

        }
    }
}
