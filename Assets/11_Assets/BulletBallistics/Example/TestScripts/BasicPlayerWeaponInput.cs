using System.Collections;
using System.Collections.Generic;
using Bolt;
using UnityEngine;

namespace Ballistics
{
    public class BasicPlayerWeaponInput : Bolt.EntityBehaviour<IPlaneState>
    {

        public float WeaponSpreadWalking;

        public List<WeaponData> Weapons;
        [HideInInspector]
        public int currentWeapon = 0;
        private int weaponBefore = -1;

        public bool isFiring = false;

        void Awake()
        {
            for (int i = 0; i < Weapons.Count; i++)
            {
                Weapons[i].weapon.OnShoot += OnShoot;
                ((DefaultMagazineController) Weapons[i].weapon.MagazineController).OnMagEmptie += OnMagazineEmptie;
            }
        }

        void OnMagazineEmptie()
        {
            //Debug.Log("You need to press 'r' to reload!");
        }

        public override void Attached()
        {
            state.OnPlaneFire = () => Fire();
        }
        
        void Update()
        {
            if (currentWeapon != -1)
            {
                HandleWeapon();
            }
            //SwitchWeapons();

            ZeroWeapons();
        }

        void ZeroWeapons()
        {
            int currentZero = Weapons[currentWeapon].weapon.currentZeroing;
            if (Input.GetKeyDown(KeyCode.KeypadPlus) || Input.GetKeyDown(KeyCode.E))
            {
                currentZero++;
            }
            if (Input.GetKeyDown(KeyCode.KeypadMinus) || Input.GetKeyDown(KeyCode.Q))
            {
                currentZero--;
            }
            Weapons[currentWeapon].weapon.currentZeroing = currentZero;
        }

        void Fire()
        {
            for (int i = 0; i < Weapons.Count; i++)
            {
                Weapons[i].weapon.Shoot();
            }
        }
        
        /// <summary>
        /// calling shoot/aim/reload on the current weapon
        /// </summary>
        void HandleWeapon()
        {
            if (isFiring)
            {
                //Fire();
                state.PlaneFire();
            }

            //Shoot
            /*if (Input.GetButton("Fire1"))
            {
                Weapons[currentWeapon].weapon.Shoot();
            }
            if (Input.GetButtonUp("Fire1"))
            {
                Weapons[currentWeapon].weapon.StopShoot();
            }*/

            //Aim
            Weapons[currentWeapon].weapon.Aim(Input.GetButton("Fire2"));

            //Reload
            if (Input.GetKeyDown(KeyCode.R))
            {
                StartCoroutine(Reload(currentWeapon));
            }
        }

        public void FireButton(bool Fire)
        {
            if (Fire)
            {
                isFiring = Fire;
            }
            else
            {
                for (int i = 0; i < Weapons.Count; i++)
                {
                    Weapons[i].weapon.StopShoot();
                }
                //Weapons[currentWeapon].weapon.StopShoot();
                isFiring = Fire;
            }
        }

        /// <summary>
        /// wait for reload time then update the magazine controller
        /// </summary>
        /// <param name="myCurrentW">current weapon id<param>
        /// <returns></returns>
        IEnumerator Reload(int myCurrentW)
        {
            //Debug.Log("Reloading...");
            yield return new WaitForSeconds(Weapons[myCurrentW].ReloadTime);
            if (myCurrentW == currentWeapon)
            {
                ((DefaultMagazineController)Weapons[currentWeapon].weapon.MagazineController).Reload();
            }
        }

        /// <summary>
        /// switch between weapons in the weapon list
        /// </summary>
        void SwitchWeapons()
        {
            if (Input.mouseScrollDelta.y > 0)
            {
                currentWeapon++;
                if (currentWeapon >= Weapons.Count)
                {
                    currentWeapon = 0;
                }
            }

            if (Input.mouseScrollDelta.y < 0)
            {
                currentWeapon--;
                if (currentWeapon < 0)
                {
                    currentWeapon = Weapons.Count - 1;
                }
            }

            if (weaponBefore != currentWeapon)
            {
                for (int i = 0; i < Weapons.Count; i++)
                {
                    Weapons[i].weapon.gameObject.SetActive(i == currentWeapon); //activate current Weapon
                }
                if (weaponBefore != -1)
                {
                    Weapons[weaponBefore].weapon.StopShoot();
                    Weapons[weaponBefore].weapon.Aim(false);
                }

            }

            weaponBefore = currentWeapon;
        }

        /// <summary>
        /// refill all MagazineControllers
        /// </summary>
        /// <param name="col"></param>
        void OnTriggerEnter(Collider col)
        {
            if (col.tag == "supply")
            {
                for (int i = 0; i < Weapons.Count; i++)
                {
                    DefaultMagazineController defaultCont = ((DefaultMagazineController)Weapons[i].weapon.MagazineController);
                    defaultCont.StoredBullets = defaultCont.MagCount * defaultCont.BulletsPerMag;
                }
            }
        }

        /// <summary>
        /// called when current weapon shoots
        /// </summary>
        void OnShoot()
        {
            //Play Muzzle Particle System
            for (int i = 0; i < Weapons.Count; i++)
            {
                if (Weapons[i].particle != null)
                {
                    Weapons[i].particle.Simulate(0, true, true);
                    ParticleSystem.EmissionModule module = Weapons[i].particle.emission;
                    module.enabled = true;
                    Weapons[i].particle.Play(true);
                }

                //Play Shoot Sound
                if (Weapons[i].sound != null)
                {
                    Weapons[i].sound.Play();
                }
            }
        }
    }

    [System.Serializable]
    public struct WeaponData
    {
        public BasicWeaponController weapon;
        public AudioSource sound;
        public ParticleSystem particle;
        public Transform ScopePos;
        public float ReloadTime;
        public float RecoilAmount;
    }

}