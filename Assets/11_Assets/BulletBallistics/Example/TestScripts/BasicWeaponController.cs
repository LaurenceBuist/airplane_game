using UnityEngine;
using System.Collections;
using System;


namespace Ballistics
{
    public class BasicWeaponController : MonoBehaviour
    {

        //General

        /// <summary>
        /// the weapon being controlled
        /// </summary>
        public Weapon TargetWeapon;

        /// <summary>
        /// delay between shots
        /// </summary>
        public float ShootDelay = 0.25f;


        /// <summary>
        /// Defines the type of weapon.
        /// </summary>
        public ShootingType WeaponType = ShootingType.SingleShot;


        //Savles / Burst

        /// <summary>
        /// amount of bullets per shot in shotgun- / burst- mode
        /// </summary>
        public int BulletsPerShell = 8;
        public int BulletsPerBurst = 3;

        //Spread
        public SpreadController SpreadController;
        //--

        //MagazineController
        public MagazineController MagazineController;
        //--

        //Zeroing
        public float[] zeroingDistances = new float[0];
        public int currentZeroing
        {
            get
            {
                return currentZero;
            }
            set
            {
                currentZero = Mathf.Clamp(value, -1, zeroingDistances.Length - 1);
            }
        }
        //--

        //private variables
        private float[] zeroCorrectionAngles;
        private int currentZero = -1;
        private bool shootReset = true;
        private int BurstBulletCounter;
        private float CooldownTimer = 0;

        //--

        public bool isAiming;

        public Action OnShoot;

        private void Start()
        {
            updateZeroingAngles();
        }

        public void updateZeroingAngles()
        {
            BallisticSettings settings = BulletHandler.instance.GetSettings();
            zeroCorrectionAngles = new float[zeroingDistances.Length];
            for (int i = 0; i < zeroingDistances.Length; i++)
            {
                zeroCorrectionAngles[i] = TargetWeapon.calculateZeroingCorrectionAngle(zeroingDistances[i], settings.useBulletdrag, settings.AirDensity);
            }
        }

        void Update()
        {
            CooldownTimer -= Time.deltaTime;
        }

        /// <summary>
        /// Is the Weapon Aiming
        /// </summary>
        /// <param name="active"></param>
        public void Aim(bool active)
        {
            isAiming = active;
        }

        /// <summary>
        /// Fire the gun. Call this every frame the trigger is held down.
        /// </summary>
        public void Shoot()
        {
            if (MagazineController.isBulletAvailable() && CooldownTimer <= 0)
            {
                float currentZeroingAngle = getCurrentZeroingAngle();
                switch (WeaponType)
                {
                    case ShootingType.Auto:
                        TargetWeapon.ShootBullet(SpreadController.GetCurrentSpread(TargetWeapon.PhysicalBulletSpawnPoint), currentZeroingAngle);
                        CallOnShoot();
                        break;
                    case ShootingType.Shotgun:
                        if (shootReset)
                        {
                            for (int i = 0; i < BulletsPerShell; i++)
                            {
                                TargetWeapon.ShootBullet(SpreadController.GetCurrentSpread(TargetWeapon.PhysicalBulletSpawnPoint), currentZeroingAngle);
                            }
                            CallOnShoot();
                            shootReset = false;
                        }
                        break;
                    case ShootingType.Burst:

                        if (shootReset)
                        {
                            TargetWeapon.ShootBullet(SpreadController.GetCurrentSpread(TargetWeapon.PhysicalBulletSpawnPoint), currentZeroingAngle);
                            BurstBulletCounter++;
                            CallOnShoot();
                            shootReset = false;
                            StartCoroutine(ShootBurst());
                        }
                        break;
                    case ShootingType.SingleShot:
                        if (shootReset)
                        {
                            TargetWeapon.ShootBullet(SpreadController.GetCurrentSpread(TargetWeapon.PhysicalBulletSpawnPoint), currentZeroingAngle);
                            CallOnShoot();
                            shootReset = false;
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Coroutine to Spawn Bullets for shooting in burst
        /// </summary>
        IEnumerator ShootBurst()
        {
            while (BurstBulletCounter < BulletsPerBurst)
            {
                yield return new WaitForSeconds(ShootDelay);
                TargetWeapon.ShootBullet(SpreadController.GetCurrentSpread(TargetWeapon.PhysicalBulletSpawnPoint));
                BurstBulletCounter++;
                CallOnShoot();
                if (BurstBulletCounter >= BulletsPerBurst)
                {
                    CooldownTimer = ShootDelay;
                    BurstBulletCounter = 0;
                    break;
                }
            }
        }

        private void CallOnShoot()
        {
            CooldownTimer = ShootDelay;
            ((DefaultMagazineController)MagazineController).onShoot();
            SpreadController.onShoot();
            if (OnShoot != null)
            {
                OnShoot();
            }
        }

        private float getCurrentZeroingAngle()
        {
            if (currentZero == -1) return 0;
            return zeroCorrectionAngles[currentZero];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>the distance the weapon is currently zeroed at</returns>
        public float getCurrentZeroingDistance()
        {
            if (currentZero == -1) return 0;
            return zeroingDistances[currentZero];
        }

        /// <summary>
        /// Tells the Weapon, that the fire button has been released to be able to shoot again ( when not in Auto - Mode )
        /// </summary>
        public void StopShoot()
        {
            shootReset = true;
        }
    }

    public enum ShootingType
    {
        SingleShot,
        Shotgun,
        Auto,
        Burst
    }
}