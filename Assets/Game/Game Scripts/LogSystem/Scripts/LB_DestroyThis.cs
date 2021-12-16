using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace LaurenceBuist
{
    public class LB_DestroyThis : MonoBehaviour
    {
        private bool fade = false;
        private TextMeshProUGUI textTMP;
        private float alpha = 1;
        
        // Start is called before the first frame update
        private void Start() //public override void Attached()
        {
            textTMP = gameObject.GetComponent<TextMeshProUGUI>();
            

            //Start Processs
            StartCoroutine(Wait());
        }

        private void Update() //public override void SimulateOwner()
        {
            if (fade)
            {
                alpha = alpha - Time.deltaTime * 0.7f;    //slowly bring down alpha value
                Debug.Log(textTMP.color);
                textTMP.color = new Color(1,1,1, alpha);
                if(alpha < 0.01) Destroy(this.gameObject);
            }
        }

        IEnumerator Wait()
        {
            yield return new WaitForSeconds(3); //Wait one frame

            fade = true;
        }
        

    }
}