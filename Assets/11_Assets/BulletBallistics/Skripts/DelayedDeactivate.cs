using UnityEngine;
using System.Collections;

namespace Ballistics
{
    public class DelayedDeactivate : PoolingObject
    {

        public void Deactivate(float t)
        {
            Invoke("delayedDeactivate", t);
        }

        private void delayedDeactivate()
        {
            Deactivate();
        }
    }
}