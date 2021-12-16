using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace LaurenceBuist
{
    public class LB_Airplane_InstrumentsInfo : MonoBehaviour
    {
        #region Variables
        public LB_Airplane_Characteristics characteristics;
        public LB_Airplane_Controller controller;

        [Header("Speed")]
        public TextMeshProUGUI  speedText;
        
        [Header("Altimeter")]
        public TextMeshProUGUI  altimeterText;
        
        [Header("Damage")]
        public TextMeshProUGUI  damageText;
        #endregion
        
        #region Constants
        public const float kphToKnts = 0.539957f;
        #endregion
        
        
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (speedText)
            {
                int currentKnots = Mathf.RoundToInt(characteristics.KPH * kphToKnts);
                speedText.text = currentKnots.ToString();
            }

            if (altimeterText)
            {
                int currentAlt = Mathf.RoundToInt(controller.CurrentAgl);
                altimeterText.text = currentAlt.ToString();
            }
            
        }
    }
}