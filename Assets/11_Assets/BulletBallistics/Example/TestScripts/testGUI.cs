using UnityEngine;
using System.Collections;

namespace Ballistics
{
    public class testGUI : MonoBehaviour
    {

        public BasicPlayerWeaponInput myBasicWeaponController;
        public UnityEngine.UI.Text zeroingText;
        public UnityEngine.UI.Text BulletsText;
        public UnityEngine.UI.Text WeaponText;
        public UnityEngine.UI.Text Fps;

        private float timer = 0;

        /// <summary>
        /// visualize weapon data
        /// </summary>
        void Update()
        {
            if (myBasicWeaponController.currentWeapon != -1)
            {
                zeroingText.text = "Zeroing: " + (myBasicWeaponController.Weapons[myBasicWeaponController.currentWeapon].weapon.getCurrentZeroingDistance());
            }
            else
            {
                zeroingText.text = "Zeroing: 0";
            }
            WeaponText.text = myBasicWeaponController.Weapons[myBasicWeaponController.currentWeapon].weapon.name;
            DefaultMagazineController magController = (DefaultMagazineController)myBasicWeaponController.Weapons[myBasicWeaponController.currentWeapon].weapon.MagazineController;
            BulletsText.text = magController.getBulletsInMag().ToString() + " / " + magController.StoredBullets.ToString();

            timer += Time.unscaledDeltaTime;

            if (timer > 0.1f)
            {
                timer = 0;
                Fps.text = "fps: " + ((int)(1.0f / Time.unscaledDeltaTime)).ToString();
            }
        }
    }
}