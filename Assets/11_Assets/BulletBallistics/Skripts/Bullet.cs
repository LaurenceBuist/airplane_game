using UnityEngine;
using System.Collections;

namespace Ballistics
{
    public class Bullet : PoolingObject
    {

        TrailRenderer trail;

        /// <summary>
        /// Bullet Debugger draws bulletpath in the Editor
        /// </summary>
#if UNITY_EDITOR
        BulletDebugger dbg;
#endif

        void Awake()
        {
            trail = GetComponent<TrailRenderer>();

#if UNITY_EDITOR
            dbg = GetComponent<BulletDebugger>();
#endif
        }

        /// <summary>
        /// called when the object reawakes from the object pool
        /// </summary>
        public override void ReAwake()
        {
            //clear trail and debugger if attached
            if (trail != null)
            {
                trail.Clear();
            }

#if UNITY_EDITOR
            if (dbg != null)
            {
                dbg.Clear();
            }
#endif
        }
    }
}
