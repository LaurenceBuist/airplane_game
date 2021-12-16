using UnityEngine;

namespace Ballistics
{
    public class Weapon : MonoBehaviour
    {

        //PUBLIC _______________________________________________________________

        //General

        /// <summary>
        /// spawnpoint of the bullet visualisation
        /// </summary>
        public Transform VisualSpawnPoint;
        /// <summary>
        /// spawnpoint of the real / physical bullet (usually center of the screen)
        /// </summary>
        public Transform PhysicalBulletSpawnPoint;

        /// <summary>
        /// Lifetime of each bullet
        /// </summary>
        public float LifeTimeOfBullets = 6;

        //Bullet
        /// <summary>
        /// Physical data of the bullet
        /// </summary>
        public BulletInfo bulletInfo = new BulletInfo(100, 500, 0.008f, 0.005f, 0.35f, ~0);
        /// <summary>
        /// Prefab of the bullet visuals
        /// </summary>
        public GameObject BulletPref;
        //--

        /// <summary>
        /// Called when a bullet is fired
        /// </summary>
        [SerializeField]
        public UnityEngine.Events.UnityEvent OnShoot;

        //-----------------------------------------------------------------------

        //PRIVATE________________________________________________________________
        private BallisticSettings Settings;

        private BulletHandler bulletHandler;

        //Pool
        private PoolManager myPool;

        //store precalculated drag to save performance
        //public float PreDrag;
        //private float area;

        //-----------------------------------------------------------------------

        void Start()
        {

            myPool = PoolManager.instance;

            bulletHandler = BulletHandler.instance;
            if (bulletHandler == null) return;

            BallisticSettings bs = bulletHandler.GetSettings();
            if (bs != null)
            {
                Settings = bs;
            }
        }

        //PUBLIC FUNCTIONS____________________________________________________________________________________________

        public BallisticSettings getSettings()
        {
            return Settings;
        }

        //public functions ____________________________________________________________________________________

        /// <summary>
        /// calculate flighttime for given distance
        /// </summary>
        /// <param name="dist">distance</param>
        /// <param name="useDrag">bullet drag enabled</param>
        /// <param name="airDensity">air density</param>
        /// <returns></returns>
        public float calculateFlighttime(float dist, bool useDrag, float airDensity = 1.22f)
        {
            dist = Mathf.Max(0, dist);
            if (useDrag)
            {
                float k = (airDensity * bulletInfo.DragCoefficient * Mathf.PI * bulletInfo.Diameter * bulletInfo.Diameter * .25f) / (2 * bulletInfo.Mass);
                return (Mathf.Exp(k * dist) - 1) / (k * bulletInfo.MaxSpeed);
            }
            else
            {
                return dist / bulletInfo.MaxSpeed;
            }
        }

        /// <summary>
        /// calculate bulletdrop at given distance
        /// </summary>
        /// <param name="dist">distance</param>
        /// <param name="useDrag">bullet drag enabled</param>
        /// <param name="airDensity">air density</param>
        /// <returns></returns>
        public float calculateBulletdrop(float dist, bool useDrag, float airDensity = 1.22f)
        {
            float FlightTime = calculateFlighttime(dist, useDrag, airDensity);
            float g = -Physics.gravity.y;

            return .5f * g * FlightTime * FlightTime;
        }

        /// <summary>
        /// calculate angle to counteract bullet drop at given distance
        /// </summary>
        /// <param name="dist">distance</param>
        /// <param name="useDrag">bullet drag enabled</param>
        /// <param name="airDensity">air density</param>
        /// <returns></returns>
        public float calculateZeroingCorrectionAngle(float dist, bool useDrag, float airDensity = 1.22f)
        {
            float drop = calculateBulletdrop(dist, useDrag, airDensity);
            return Mathf.Atan(drop / dist) * Mathf.Rad2Deg;
        }

        //-----------------------------------------------------------------------------------------------------------


        /// <summary>
        /// Instantiates the Bullet and gives them over to BulletHandler for Calculation
        /// </summary>
        /// <param name="ShootDirection">the direction the bullet is fired in ( usually you want to use 'PhysicalBulletSpawnPoint.forward' + some offset for recoil )</param>
        /// <param name="zeroAngle">to counteract bullet drop you can angle the bullet slighty upwards. You can calculate this angle with 'weapon.calculateZeroingCorrectionAngle(..)'</param>
        public void ShootBullet(Vector3 ShootDirection, float zeroAngle = 0)
        {
            PoolingObject bClone = null;
            if (BulletPref != null)
            {
                bClone = myPool.Get(BulletPref.gameObject);
                bClone.transform.position = VisualSpawnPoint.position;
            }
            //calculte in zeroing corrections
            ShootDirection.Normalize();
            Vector3 right = Vector3.Cross(ShootDirection, Vector3.up);
            Vector3 dir = Quaternion.AngleAxis(zeroAngle, right) * ShootDirection;


            //give the BulletInstace over to the BulletHandler
            bulletInfo.calcDrag();
            bulletHandler.AddBullet(new BulletData(bulletInfo, PhysicalBulletSpawnPoint.position, dir, LifeTimeOfBullets, bulletInfo.MaxSpeed, bClone));
            OnShoot.Invoke();

        }
    }

    [System.Serializable]
    public struct BulletInfo
    {
        /// <summary>
        /// damage the bullet deals at initialisation
        /// </summary>
        public float MuzzleDamage;
        /// <summary>
        /// initial speed of the bullet
        /// </summary>
        public float MaxSpeed;
        /// <summary>
        /// mass of each bullet
        /// </summary>
        public float Mass;
        /// <summary>
        /// bullet diameter
        /// </summary>
        public float Diameter;
        /// <summary>
        /// the drag coefficient ( sphere .5f )
        /// </summary>
        public float DragCoefficient;
        /// <summary>
        /// wich layers does this bullet interact with
        /// </summary>
        public LayerMask HitMask;

        public float precalculatedDrag;

        public BulletInfo(float muzzleDamage, float maxSpeed, float mass, float diameter, float dragCoefficient, LayerMask hitMask)
        {
            this.MuzzleDamage = muzzleDamage;
            this.MaxSpeed = maxSpeed;
            this.Mass = mass;
            this.Diameter = diameter;
            this.DragCoefficient = dragCoefficient;
            this.HitMask = hitMask;

            precalculatedDrag = 0;
        }

        public void calcDrag()
        {
            precalculatedDrag = (Diameter * Diameter * .25f * Mathf.PI * DragCoefficient) / (2 * Mass);
        }
    }
}