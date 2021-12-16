using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LaurenceBuist
{
    public class LB_Airplane_Camera : LB_Basic_Follow_Camera
    {
        
        #region Variables

        [Header("Airplane Camera Properties")] public float minHeightFromGround = 2f;
        #endregion
        protected override void HandleCamera()
        {
            //Airplane Camera Code
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit))
            {
                if (hit.distance < minHeightFromGround && hit.transform.CompareTag("ground"))
                {
                    float wantedHeight = origHeight + (minHeightFromGround - hit.distance);
                    height = wantedHeight;
                }
            }
            base.HandleCamera();
        }
    }
}