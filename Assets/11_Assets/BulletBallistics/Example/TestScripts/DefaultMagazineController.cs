using UnityEngine;
using System;

namespace Ballistics
{
    /// <summary>
    /// basic magazine controller script
    /// </summary>
    public class DefaultMagazineController : MagazineController
    {
        public int MagCount;
        public int BulletsPerMag;
        [HideInInspector]
        public int StoredBullets;

        [HideInInspector]
        public int CurrentBulletAmount { get; private set; }

        public Action OnMagEmptie;

        void Awake()
        {
            StoredBullets = BulletsPerMag * MagCount;
            Reload();
        }

        /// <summary>
        /// returns if there is a bullet in the current clip
        /// </summary>
        /// <returns></returns>
        public override bool isBulletAvailable()
        {
            return CurrentBulletAmount > 0;
        }

        /// <summary>
        /// Reloads the weapon.
        /// </summary>
        /// <param name="lossless">When set to 'true' left over bullets are restored.</param>
        public bool Reload()
        {
            if (CurrentBulletAmount > 0)
            {
                //add leftover amunition back inventory..
                StoredBullets += CurrentBulletAmount;
            }
            //load amunition in current Magazine
            if (StoredBullets > BulletsPerMag)
            {
                StoredBullets -= BulletsPerMag;
                CurrentBulletAmount = BulletsPerMag;
            }
            else
            {
                if (StoredBullets <= 0)
                {
                    return false;
                }
                else
                {
                    CurrentBulletAmount += StoredBullets;
                    StoredBullets = 0;
                }
            }
            return true;
        }

        public int getBulletsInMag()
        {
            return CurrentBulletAmount;
        }

        /// <summary>
        /// called from the wheapon when the weapon shoots
        /// </summary>
        public void onShoot()
        {
            CurrentBulletAmount -= 1;
            if (CurrentBulletAmount <= 0)
            {
                CurrentBulletAmount = 0;
            }
        }
    }
}
