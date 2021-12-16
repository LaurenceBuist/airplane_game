using System.Collections;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace Ballistics
{
    public class BallisticSettings : ScriptableObject
    {
        //calculate with bulletdrop?
        public bool useBulletdrop = true;
        //calculate with bulletdrag?
        public bool useBulletdrag = true;
        //calculate with ballisticMaterials?
        public bool useBallisticMaterials = true;

        //Wind Direction
        public Vector3 WindVelocity;

        //Air density
        public float AirDensity;

        //BallisticObjectData
        public List<MaterialData> materialData = new List<MaterialData>();

    }

    [System.Serializable]
    public struct MaterialData
    {
        /// <summary>
        /// material name
        /// </summary>
        public string Name;

        /// <summary>
        /// Energyloss of the bullet traveling through 1 unit of this material
        /// </summary>
        public float EnergylossPerUnit;

        /// <summary>
        /// random spread when the bullet flew through this material
        /// </summary>
        public float RndSpread;

        /// <summary>
        /// random spread when the bullet got reflected by this material
        /// </summary>
        public float RndSpreadRic;

        /// <summary>
        /// curve describes from x: 0 - 1 ( -> 0° - 90° impacat angle)
        /// the propability y: 0 - 1 ( -> 0% - 100% ) that the bullet becomes a ricochet when it hits this material
        /// </summary>
        public AnimationCurve RicochetPropability;

        /// <summary>
        /// prefab with script ( that inherits from impactObject ) attached that is instantiated on the impact with this material
        /// </summary>
        public ImpactObject impactObject;
    }

    
}