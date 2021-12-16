using System;
using UnityEngine;

namespace LaurenceBuist
{
    public class LB_Menu_TurnOffOnAwake : MonoBehaviour
    {
        public GameObject off;
        public bool onOff;

        private void OnEnable()
        {
            off.SetActive(onOff);
        }
    }
}