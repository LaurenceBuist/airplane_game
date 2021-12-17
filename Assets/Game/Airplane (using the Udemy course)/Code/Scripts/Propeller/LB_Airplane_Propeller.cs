using UnityEngine;
using Photon.Bolt;

namespace LaurenceBuist
{
    public class LB_Airplane_Propeller : EntityBehaviour<IPlaneState>
    {
        #region Variables
        [Header("Propeller Properties")]
        public float minRotationRPM = 1100f;
        public float minQuadRPMs = 300f;
        public float minTextureSwap = 600f;
        public GameObject mainProp;
        public GameObject blurredProp;

        [Header("Material Properties")]
        public Material blurredPropMat;
        public Texture2D blurLevel1;
        public Texture2D blurLevel2;

        [Header("Bolt")] 
        public GameObject[] objects;
        private int stateInt = 0;
        
        #endregion

        #region Custom Methods

        public override void Attached()
        {
            state.SetTransforms(state.PropellerRotation, transform);

            if (mainProp && blurredProp) HandleSwapping(0f);

            if (entity.IsOwner)
            {
                for (int i = 0; i < state.PropellerArray.Length; i++)
                {
                    state.PropellerArray[i].PropellerId = i;
                }

                state.PropellerActiveIndex = 0;
            }
            
            state.AddCallback("PropellerActiveIndex", PropellerActiveIndexChanged);
        }

        public void HandlePropeller(float currentRPM)
        {
            //get degrees per second
            float dps = ((currentRPM * 360f) / 60f) * Time.deltaTime;
            dps = Mathf.Clamp(dps, 0f, minRotationRPM);
            
            //Rotate the Propeller Group
            transform.Rotate(Vector3.forward, dps);

            //Handle Propeller Swapping
            if (mainProp && blurredProp) HandleSwapping(currentRPM);
        }

        void HandleSwapping(float currentRPM)
        {
            if (currentRPM > minQuadRPMs)
            { 
                //if(stateInt != 1) state.PropellerActiveIndex = 1;

                if (blurredPropMat && blurLevel1 && blurLevel2)
                {
                    if (currentRPM > minTextureSwap)
                    {
                        if (stateInt != 2) state.PropellerActiveIndex = 2;
                    }
                    else
                    {
                        if (stateInt != 1) state.PropellerActiveIndex = 1;
                    }
                }
            }
            
            else
            {
                if(stateInt != 0) state.PropellerActiveIndex = 0;
            }
        }

        void PropellerActiveIndexChanged()
        {
            stateInt = state.PropellerActiveIndex;
            //Debug.Log("stateInt: " + stateInt + "    PropellerInt: " + state.PropellerActiveIndex);
            for (int i = 0; i < objects.Length; i++)
            {
                objects[i].SetActive(false);
            }

            int objectId = state.PropellerArray[state.PropellerActiveIndex].PropellerId;
            objects[objectId].SetActive(true);
            /*if (state.PropellerActiveIndex == 1)
            {
                blurredProp.gameObject.SetActive(true);
                mainProp.gameObject.SetActive(false);
            } else if (state.PropellerActiveIndex == 2)
                blurredPropMat.SetTexture("_MainTex", blurLevel1);
            else if (state.PropellerActiveIndex == 3)
                blurredPropMat.SetTexture("_MainTex", blurLevel2);
            else
            {
                blurredProp.gameObject.SetActive(false);
                mainProp.gameObject.SetActive(true);
            }*/
        }
        #endregion

    }
}

