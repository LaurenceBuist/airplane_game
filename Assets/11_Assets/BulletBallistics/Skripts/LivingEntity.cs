using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LaurenceBuist;

namespace Ballistics
{
    public class LivingEntity : MonoBehaviour
    {
        /// <summary>
        /// maximum health
        /// </summary>
        public float StartHealth;

        public LB_Airplane_Network Network;
        
        public LB_Airplane_InstrumentsInfo InstrumentsInfo;

        public LB_Airplane_Characteristics Characteristics;

        [Header("Death Controller")]
        public GameObject particleEffect;
        public List<GameObject> ShutOffObjects;
        //public List<GameObject> BodyColliders;
        public bool shotDown = false;

        /// <summary>
        /// health handler
        /// </summary>
        [HideInInspector]
        public float Health
        {
            get { return myHealth; }
            set
            {
                float before = myHealth;
                if (value > 0f && value <= StartHealth)
                {
                    myHealth = value;
                }
                else
                {
                    if (value > 0f)
                    {
                        myHealth = StartHealth;
                    }
                    else
                    {
                        myHealth = 0;
                        OnDeath();
                    }
                }
                OnHealthChanged(value - before);
            }
        }

        private float myHealth = 0;

        void Awake()
        {
            myHealth = StartHealth;
            OnHealthChanged(myHealth);
        }

        /// <summary>
        /// called when health is 0
        /// </summary>
        public virtual void OnDeath()
        {
            Characteristics.shotDown = true;
            gameObject.GetComponent<Rigidbody>().useGravity = false;
            gameObject.GetComponent<Rigidbody>().isKinematic = true;
            if (ShutOffObjects[0].activeInHierarchy)
            {
                particleEffect.GetComponent<ParticleSystem>().Play(true);
                //particleEffect.GetComponent<DefaultBallisticObject>().enabled = false;
                foreach (GameObject x in ShutOffObjects) x.SetActive(false);
                //foreach (GameObject x in BodyColliders) x.layer = 10;
            }
            //gameObject.layer = 10;
        }

        /// <summary>
        /// called when the health value changed
        /// </summary>
        /// <param name="amount">value of the change</param>
        public virtual void OnHealthChanged(float amount)
        {
            InstrumentsInfo.damageText.text = (Mathf.Round((1-myHealth / StartHealth)*100)).ToString();
            
            //Update health through network
            Network.UpdateHealth(Health);
        }
    }
}