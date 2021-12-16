//disable Bullet debugging? Comment out the next line
#define BULLET_DEBUGGER

using UnityEngine;
using System.Collections.Generic;

namespace Ballistics
{
    public class BulletHandler : MonoBehaviour
    {
        /// <summary>
        /// List of all bullets in the scene
        /// </summary>
        [HideInInspector]
        public List<BulletData> Bullets = new List<BulletData>();

        /// <summary>
        /// maximal amount of bullets getting updated each frame
        /// </summary>
        [Tooltip("Maximum amount of bullet calculations per frame")]
        public int MaxBulletUpdatesPerFrame = 300;

        /// <summary>
        /// time it takes the bullet visualisation to move to the calculated virtual bullet
        /// </summary>
        [Tooltip("Time (s) until the visual representation of the bullet reaches the position of the calculation")]
        public float VisualBulletToRealBulletMovementTime = 0.1f;

        public LayerMask mask = 0;

        /// <summary>
        /// singelton to bullethandler instance
        /// </summary>
        public static BulletHandler instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<BulletHandler>();
                    if (_instance == null)
                    {
                        GameObject handler = new GameObject("BulletHandler");
                        _instance = handler.AddComponent<BulletHandler>();
                        Debug.LogWarning("Added Bullethandler to scene!");

                        BallisticSettings[] settings = Resources.FindObjectsOfTypeAll<BallisticSettings>();
                        if (settings.Length == 0)
                        {
                            Debug.LogWarning("No BallisticSettings found!");
                        }
                        else
                        {
                            _instance.Settings = settings[0];
                        }
                        
                    }
                }
                return _instance;
            }
        }
        [SerializeField]
        private static BulletHandler _instance;

        //Settings
        [SerializeField]
        private BallisticSettings Settings;
        public BallisticSettings GetSettings()
        {
            return Settings;
        }

        //private
        private float g;

        void Awake()
        {
            if (instance == null)
            {
                _instance = this;
            }
            if (Settings == null)
            {
                Debug.LogError("No Ballistic Settings assigned in BulletHandler!");
            }

            g = Physics.gravity.y;
        }

        void LateUpdate()
        {
            UpdateBullets();
        }

        /// <summary>
        /// calculate bullet flights
        /// </summary>
        void UpdateBullets()
        {
            float deltaTime = Time.deltaTime;

            float windSpeed = Settings.WindVelocity.magnitude;

            float leftOverFlyTime = 0;
            float myDeltaTime = 0;
            bool processAgain = true;

#if (UNITY_EDITOR && BULLET_DEBUGGER)
            BulletDebugger debugger = null;
#endif

            //Process each Bullet
            for (int i = 0; i < Bullets.Count; i++)
            {
                if (i < MaxBulletUpdatesPerFrame)
                {
                    BulletData cBullet = Bullets[0];
                    Bullets.RemoveAt(0);

#if (UNITY_EDITOR && BULLET_DEBUGGER) 
                    if (cBullet.visualBullet != null)
                    {
                        debugger = cBullet.visualBullet.GetComponent<BulletDebugger>();
                    }
                    else
                    {
                        debugger = null;
                    }
#endif

                    processAgain = true;
                    while (processAgain)
                    {
                        //when current bullet passed processing already but has 'flytime over'
                        if (leftOverFlyTime == 0)
                        {
                            myDeltaTime = deltaTime;
                        }
                        else
                        {
                            myDeltaTime = leftOverFlyTime;
                            leftOverFlyTime = 0;
                        }

                        //when this bullet had to wait in the query for more than 1 frame
                        if (cBullet.timeSinceLastUpdate > 0)
                        {
                            myDeltaTime += cBullet.timeSinceLastUpdate;
                            cBullet.timeSinceLastUpdate = 0;
                        }

                        //decrease Lifetime
                        cBullet.lifeTime -= myDeltaTime;
                        if (cBullet.lifeTime <= 0)
                        {
                            //remove Bullet if dead
                            DeactivateBullet(cBullet.visualBullet);
                            processAgain = false;
                            continue;
                        }

                        if (Settings.useBulletdrag)
                        {
                            //Air resistence
                            Vector3 bulletVelocity = cBullet.bulletDir * cBullet.Speed;
                            bulletVelocity -= (bulletVelocity * cBullet.Speed - Settings.WindVelocity * windSpeed) * cBullet.bulletInfo.precalculatedDrag * Settings.AirDensity * myDeltaTime;

                            cBullet.Speed = bulletVelocity.magnitude;
                            cBullet.bulletDir = bulletVelocity.normalized;
                        }

                        if (Settings.useBulletdrop)
                        {
                            //Bulletdrop
                            cBullet.ySpeed += g * myDeltaTime;

                            if (Settings.useBulletdrag)
                            {
                              cBullet.ySpeed -= cBullet.bulletInfo.precalculatedDrag * Settings.AirDensity * cBullet.ySpeed * cBullet.ySpeed * myDeltaTime;
                            }

                             cBullet.bulletPos.y += cBullet.ySpeed * myDeltaTime;
                        }


                        
                        

#if (UNITY_EDITOR && BULLET_DEBUGGER)
                        if (debugger != null)
                        {
                            debugger.AddPos(cBullet.bulletPos);
                        }
#endif
                        //Move Bullet
                        cBullet.bulletPos += cBullet.bulletDir * cBullet.Speed * myDeltaTime;

                        //Hitcheck------------------
                        Vector3 realBulletDir = cBullet.bulletPos - cBullet.LastPos;
                        float dirMag = realBulletDir.magnitude;
                        float rBulletSpeed = dirMag / myDeltaTime;
                        Ray ray = new Ray(cBullet.LastPos, realBulletDir);
                        RaycastHit hit;

                        if (Physics.Raycast(ray, out hit, dirMag, cBullet.bulletInfo.HitMask.value))
                        {
                            Transform hitTrans = hit.transform;
                            BallisticObject hitBallisticObject;

                            hitBallisticObject = Settings.useBallisticMaterials ? hitTrans.GetComponent<BallisticObject>() : null;

                            if ((hitBallisticObject != null) ? hitBallisticObject.MatID >= Settings.materialData.Count : true)
                            {
                                //hit object has no ballistic material
#if (UNITY_EDITOR && BULLET_DEBUGGER)
                                if (debugger != null)
                                {
                                    debugger.AddPos(hit.point);
                                }
#endif

                                //Stop Bullet
                                DeactivateBullet(cBullet.visualBullet);
                                processAgain = false;
                                continue;
                            }
                            else
                            {
                                MaterialData hitData = Settings.materialData[hitBallisticObject.MatID];

                                Vector3 bulletDirNormalized = realBulletDir.normalized;

                                //does this bullet ricochet?
                                if (hitData.RicochetPropability.Evaluate(Vector3.Angle(hit.normal, -bulletDirNormalized) / 90f) < Random.Range(0f, 1f))
                                {

                                    //maximal material penetration distance of this bullet
                                    float MaxRange = (0.5f * cBullet.bulletInfo.Mass * rBulletSpeed * rBulletSpeed) / hitData.EnergylossPerUnit;

                                    //backtrace the bulletpath to find out whether the bullet went through the material
                                    ray = new Ray(hit.point + bulletDirNormalized * MaxRange, -bulletDirNormalized);
                                    RaycastHit[] hits = Physics.RaycastAll(ray, MaxRange, cBullet.bulletInfo.HitMask);


                                    int OutIndex = -1;
                                    float shortestDist = 0f;
                                    for (int n = 0; n < hits.Length; n++)
                                    {
                                        if (hits[n].transform == hitTrans)
                                        {
                                            //backwards
                                            if (hits[n].distance > shortestDist)
                                            {
                                                OutIndex = n;
                                                shortestDist = hits[n].distance;
                                            }
                                        }
                                    }

                                    if (OutIndex != -1)
                                    {
                                        //Shoot through
                                        RaycastHit outHit = hits[OutIndex];

                                        //slowdown
                                        float dist = (outHit.point - hit.point).magnitude;
                                        float fac = 1 - (dist / MaxRange);
                                        float afterSpeed = rBulletSpeed * (1 - fac);
                                        cBullet.Speed *= fac;
                                        cBullet.ySpeed *= fac;

                                        //Spread
                                        cBullet.bulletDir = hitData.RndSpread > 0 ? (Quaternion.AngleAxis(Random.Range(0f, 360f), cBullet.bulletDir) * Quaternion.AngleAxis(Random.Range(0f, hitData.RndSpread), Vector3.Cross(Vector3.up, cBullet.bulletDir)) * cBullet.bulletDir) : cBullet.bulletDir;


                                        //process Bullet again
                                        leftOverFlyTime = Mathf.Max(0, (1f - ((cBullet.bulletPos - outHit.point).magnitude / dirMag))) * myDeltaTime;

                                        //call BulletImpact on the hitBallisticObject
                                        hitBallisticObject.BulletImpact(new HitInfo(hit, bulletDirNormalized, rBulletSpeed - afterSpeed, dist, cBullet.bulletInfo, hitData));

                                        //bullet exit
                                        hitBallisticObject.BulletImpact(new HitInfo(outHit, bulletDirNormalized, 0, dist, cBullet.bulletInfo, hitData));

                                        cBullet.bulletPos = cBullet.LastPos = outHit.point;

#if (UNITY_EDITOR && BULLET_DEBUGGER)
                                        if (debugger != null)
                                        {
                                            debugger.AddPos(cBullet.bulletPos);
                                        }
#endif

                                        processAgain = true;
                                        continue;
                                    }
                                    else
                                    {
                                        //Bullet stuck in object


                                        //call BulletImpact on the hitBallisticObject
                                        hitBallisticObject.BulletImpact(new HitInfo(hit, bulletDirNormalized, rBulletSpeed, MaxRange, cBullet.bulletInfo, hitData));

#if (UNITY_EDITOR && BULLET_DEBUGGER)
                                        if (debugger != null)
                                        {
                                            debugger.AddPos(hit.point);
                                        }
#endif

                                        //Stop Bullet
                                        DeactivateBullet(cBullet.visualBullet);
                                        processAgain = false;
                                        continue;
                                    }
                                }
                                else
                                {
                                    //Ricochet
                                    cBullet.StartOffset = Vector3.zero;

                                    //Reflect bullet
                                    cBullet.bulletDir = Vector3.Reflect(bulletDirNormalized, hit.normal);

                                    cBullet.bulletDir = hitData.RndSpreadRic > 0 ? (Quaternion.AngleAxis(Random.Range(0f, 360f), cBullet.bulletDir) * Quaternion.AngleAxis(Random.Range(0f, hitData.RndSpreadRic), Vector3.Cross(hit.normal, cBullet.bulletDir)) * cBullet.bulletDir) : cBullet.bulletDir;

                                    //Slowdown
                                    float fac = 1f - (Vector3.Angle(bulletDirNormalized, cBullet.bulletDir) / 180f);
                                    cBullet.Speed *= fac;
                                    cBullet.ySpeed = 0;

                                    leftOverFlyTime = Mathf.Max(0, 1f - ((hit.point - cBullet.bulletPos).magnitude / dirMag)) * myDeltaTime;

                                    //call BulletImpact on the hitBallisticObject
                                    hitBallisticObject.BulletImpact(new HitInfo(hit, -hit.normal, rBulletSpeed * (1 - fac), 0f, cBullet.bulletInfo, hitData));

                                    //process Bullet again
                                    cBullet.bulletPos = cBullet.LastPos = hit.point;

#if (UNITY_EDITOR && BULLET_DEBUGGER)
                                    if (debugger != null)
                                    {
                                        debugger.AddPos(cBullet.bulletPos);
                                    }
#endif
                                    processAgain = true;
                                    continue;
                                }
                            }
                        }
                        
                        
                        processAgain = false;

                        cBullet.LastPos = cBullet.bulletPos;
                        //Update Transform
                        if (cBullet.visualBullet != null)
                        {
                            if (cBullet.StartLifeTime - cBullet.lifeTime < VisualBulletToRealBulletMovementTime)
                            {
                                cBullet.VisualOffset = Vector3.Lerp(cBullet.StartOffset, Vector3.zero, (cBullet.StartLifeTime - cBullet.lifeTime) / VisualBulletToRealBulletMovementTime);
                            }
                            else
                            {
                                cBullet.VisualOffset = Vector3.zero;
                            }
                            cBullet.visualBullet.transform.position = cBullet.bulletPos + cBullet.VisualOffset;
                            if (dirMag > 0)
                            {
                                cBullet.visualBullet.transform.rotation = Quaternion.LookRotation(realBulletDir);
                            }
                        }
                        //Enqueue at End
                        Bullets.Add(cBullet);
                    }
                }
                else
                {
                    BulletData cBullet = Bullets[i];
                    cBullet.timeSinceLastUpdate += Time.deltaTime;
                }

            }
        }


        /// <summary>
        /// add bullet back to pool and make it invisible
        /// </summary>
        /// <param name="weapon"></param>
        /// <param name="bulletTrans"></param>
        void DeactivateBullet(PoolingObject bullet)
        {
            if (bullet != null)
            {
                 bullet.Deactivate();
            }
        }

        /// <summary>
        /// Add Bullet to update list
        /// </summary>
        /// <param name="bullet">new Bullet Data</param>
        public void AddBullet(BulletData bullet)
        {
            Bullets.Add(bullet);
        }
    }


    public struct BulletData
    {
        public BulletInfo bulletInfo;
        public Vector3 bulletPos;
        public Vector3 LastPos;
        public Vector3 bulletDir;
        public Vector3 VisualOffset;
        public float lifeTime;
        public float StartLifeTime;
        public Vector3 StartOffset;
        public float Speed;
        public float ySpeed;
        public PoolingObject visualBullet;

        public float timeSinceLastUpdate;

        public BulletData(BulletInfo bInfo, Vector3 pos, Vector3 dir, float life, float speed, PoolingObject bC)
        {
            bulletInfo = bInfo;
            bulletPos = pos;
            LastPos = pos;
            lifeTime = life;
            StartLifeTime = lifeTime;
            bulletDir = dir;
            Speed = speed;
            ySpeed = 0;
            visualBullet = bC;
            timeSinceLastUpdate = 0;

            if (bC != null)
            {
                StartOffset = bC.transform.position - pos;
                VisualOffset = StartOffset;
            }
            else
            {
                StartOffset = VisualOffset = Vector3.zero;
            }
        }
    }
}
