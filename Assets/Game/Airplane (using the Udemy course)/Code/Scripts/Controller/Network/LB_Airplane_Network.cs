using Ballistics;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace LaurenceBuist
{
    public class LB_Airplane_Network : LB_BaseRigidBody_Controller
    {
        //[Header("Colliders")]
        //public GameObject[] colliders;
        
        [Header("Name Tag")]
        public GameObject NameTag;

        [Header("Player Weapons")]
        public Weapon[] weapons;
        public GameObject otherPlayerBullets;

        [Header("World Controller")]
        public GameObject worldController;

        [Header("Living Entity")]
        public LivingEntity LivingEntity;

        // Start is called before the first frame update
        public override void Attached()
        {
            state.AddCallback("Health", () => LivingEntity.Health = state.Health);
            if (entity.IsOwner)
            {
                state.Health = LivingEntity.StartHealth;
            }
            
            
            state.AddCallback("PlaneName", () => NameTag.GetComponent<LB_Airplane_GUINameTag>().playerName = state.PlaneName);
            if(!BoltNetwork.IsSinglePlayer && !isOffline){
                state.SetTransforms(state.PlaneTransform, transform);

                if (!BoltNetwork.IsServer)
                {
                    var evnt = PlayerJoinedEvent.Create();
                    evnt.Message = PlayerPrefs.GetString("PlayerName") + "joined the game";
                    evnt.Send();
                }
            }

            if (entity.IsOwner)
            {
                //Name Tag
                NameTag.SetActive(false);
                state.PlaneName = PlayerPrefs.GetString("PlayerName");
                
                worldController.tag = "Player";
                foreach (Weapon weapon in weapons)
                {
                    //weapon.BulletPref = otherPlayerBullets;
                    weapon.bulletInfo.HitMask = LayerMask.GetMask("OtherPlayers");
                }
            }
            else
            {
                //Bullet Settings
                gameObject.layer = 10;
                foreach (Weapon weapon in weapons)
                {
                    //weapon.BulletPref = otherPlayerBullets;
                    weapon.bulletInfo.HitMask = LayerMask.GetMask("Player");
                }
            }
        }

        protected override void HandleNetwork()
        {
            
        }

        public void UpdateHealth(float Health)
        {
            if (entity.IsOwner)
            {
                state.Health = Health;
            }
        }
    }
}