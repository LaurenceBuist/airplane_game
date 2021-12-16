using UnityEngine;
using System.Collections.Generic;

namespace Ballistics
{
    /// <summary>
    /// example player class
    /// </summary>
    public class PlayerController : LivingEntity
    {

        Transform Trans;
        CharacterController controller;
        Camera Cam;
        public BasicPlayerWeaponInput basicWeaponHandle;

        public float MoveSpeed;
        public float TurnSpeed;

        public Transform HandTrans;
        private Vector3 StartHandPos;
        private Vector3 StartHandEuler;
        public Vector3 relhandMovement;
        public Vector3 relHandRotEuler;
        private float t;
        public float RecoilCorrectionTime;

        private float startFov;

        private float ySpeed;
        public float JumpForce;

        private List<WeaponData> Weapons;

        void Awake()
        {
            Trans = this.transform;
            controller = GetComponent<CharacterController>();
            Cam = Camera.main;
            startFov = Cam.fieldOfView;
            StartHandPos = HandTrans.localPosition;
            StartHandEuler = HandTrans.localEulerAngles;

            Weapons = basicWeaponHandle.Weapons;
            for (int i = 0; i < Weapons.Count; i++)
            {
                Weapons[i].weapon.OnShoot += OnShoot;
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                Debug.Break();
            }
            if (Input.GetMouseButtonDown(0))
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }

            if (basicWeaponHandle.currentWeapon != -1)
            {
                Recoil();
                Aim();
            }
            Move();

            //slowmo
            Time.timeScale = Mathf.Lerp(Time.timeScale, Input.GetKey(KeyCode.LeftShift) ? 0.05f : 1, Time.unscaledDeltaTime * 30);
        }

        /// <summary>
        /// change camera fov etc.
        /// </summary>
        void Aim()
        {
            if (Weapons[basicWeaponHandle.currentWeapon].weapon.isAiming)
            {
                Vector3 scopePos = Weapons[basicWeaponHandle.currentWeapon].ScopePos.localPosition;
                HandTrans.localPosition = new Vector3(-scopePos.x, -scopePos.y, -scopePos.z);
            }
            Cam.fieldOfView = Mathf.Lerp(Cam.fieldOfView, startFov * (Weapons[basicWeaponHandle.currentWeapon].weapon.isAiming ? 0.35f : 1f), Time.deltaTime * 15);
        }

        /// <summary>
        /// basic visual weapon recoil
        /// </summary>
        void Recoil()
        {
            if (!Weapons[basicWeaponHandle.currentWeapon].weapon.isAiming)
            {
                HandTrans.localPosition = Vector3.Lerp(StartHandPos, StartHandPos + relhandMovement, t);
                HandTrans.localEulerAngles = Vector3.Lerp(StartHandEuler, StartHandEuler + relHandRotEuler, t);
            }
            else
            {
                HandTrans.localEulerAngles = Vector3.Lerp(StartHandEuler, StartHandEuler + relHandRotEuler, t);
            }
            t = Mathf.Clamp01(t - (Time.deltaTime / RecoilCorrectionTime));
        }



        /// <summary>
        /// basic player movement
        /// </summary>
        private void Move()
        {

            //standart movement
            //rotate
            Vector2 rot = new Vector2(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X")) * TurnSpeed * Time.timeScale;
            Cam.transform.Rotate(rot.x, 0, 0);
            Trans.Rotate(0, rot.y, 0);

            //move
            Vector3 keyInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

            if (!controller.isGrounded)
            {
                ySpeed -= 60 * Time.deltaTime;
            }
            else
            {
                ySpeed = -1;
            }
            if (Input.GetButton("Jump") && controller.isGrounded)
            {
                ySpeed = JumpForce;
            }

            controller.Move(((Trans.TransformDirection(keyInput) * MoveSpeed) + Vector3.up * ySpeed) * Time.deltaTime);

            //Spread when walking
            ((DefaultSpreadController)Weapons[basicWeaponHandle.currentWeapon].weapon.SpreadController).setBaseSpread(keyInput.magnitude * basicWeaponHandle.WeaponSpreadWalking);
        }

        void OnShoot()
        {
            t = Mathf.Clamp01(t + Weapons[basicWeaponHandle.currentWeapon].RecoilAmount);
        }


    }
}

