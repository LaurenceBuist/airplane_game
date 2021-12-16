using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ballistics {

    //very simple Example, how to make your Weapon shoot
    //for a more complex integration take a look at the BasicWeaponController script
    public class SimpleWeaponInput : MonoBehaviour {

        //reference to your weapon
        public Weapon myWeapon;

        public string FireButtonName = "Fire1";

        void Awake()
        {
            //myWeapon not assigned
            if (myWeapon == null)
            {
                //check for weapon attached to this object
                myWeapon = GetComponent<Weapon>();
            }
        }

        void Update()
        {
            if (Input.GetButtonDown(FireButtonName))
            {
                //call the ShootBullet methode with the bulletdirection when the "Fire1" button is pressed
                myWeapon.ShootBullet(myWeapon.PhysicalBulletSpawnPoint.forward);
            }
        }
    }
}
