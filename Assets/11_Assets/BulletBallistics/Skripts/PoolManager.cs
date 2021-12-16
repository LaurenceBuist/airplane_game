 using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Ballistics
{
    public class PoolManager : MonoBehaviour
    {
        /// <summary>
        /// all objects in the pool
        /// </summary>
        private Dictionary<int, Queue<PoolingObject>> Pool = new Dictionary<int, Queue<PoolingObject>>();

        /// <summary>
        /// current instance
        /// </summary>
        static PoolManager _instance;

        public static PoolManager instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<PoolManager>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("Pool");
                        _instance = go.AddComponent<PoolManager>();
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// add a object to the pool
        /// </summary>
        /// <param name="obj">the object that is added to the pool</param>
        public void AddObject<T>(T obj) where T : PoolingObject
        {
            int id = obj.GetID();
            if (Pool.ContainsKey(id))
            {
                Pool[id].Enqueue(obj);
                
            }
            else
            {
                Pool.Add(id, new Queue<PoolingObject>());
                Pool[id].Enqueue(obj);
            }
        }

        /// <summary>
        /// get the next object in the queue
        /// </summary>
        /// <returns></returns>
        public PoolingObject Get(GameObject ID)
        {
            int id = ID.GetInstanceID();
            PoolingObject pObj = null;
            if (Pool.ContainsKey(id))
            {
                if (Pool[id].Count > 0)
                {
                    pObj = Pool[id].Dequeue();
                    pObj.ReAwake();
                    pObj.gameObject.SetActive(true);
                    return pObj;
                }
            }

            GameObject instance = Instantiate(ID);
            pObj = instance.GetComponent<PoolingObject>();
            if (pObj == null)
            {
                pObj = instance.AddComponent<PoolingObject>();
            }
            pObj.SetID(ID);

            return pObj;
        }
    }
}
