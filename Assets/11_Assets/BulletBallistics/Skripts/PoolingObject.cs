using UnityEngine;

namespace Ballistics
{
    public class PoolingObject : MonoBehaviour
    {
        private int parentID;

        public void SetID(GameObject parent)
        {
            this.parentID = parent.GetInstanceID();
        }

        public int GetID()
        {
            return this.parentID;
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
            PoolManager.instance.AddObject(this);
        }

        /// <summary>
        /// called from PoolManager when the object reawakes
        /// </summary>
        public virtual void ReAwake() { }
    }
}
