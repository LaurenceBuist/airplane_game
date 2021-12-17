using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Bolt.Utils;
using UnityEngine;

namespace LaurenceBuist
{
    public class LB_OnTriggerSetActive : MonoBehaviour
    {

        [Header("Tag of object which triggers")]
        public string tag;
        
        [Header("Objects to disable")]
        public GameObject[] sceneObjects;

        [Header("Set GameObject active on Trigger?")]
        public bool yes;
        
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(tag))
            {
                foreach (var sceneObj in sceneObjects) sceneObj.SetActive(!yes);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(tag))
            {
                foreach (var sceneObj in sceneObjects) sceneObj.SetActive(yes);
            }
        }
    }
}