using UnityEngine;
using System.Collections;

namespace Ballistics
{
    public class LivingEntityCollider : MonoBehaviour
    {
        //set e.g. to 0.75f if LivingEntity has armor
        [Tooltip("Multiplier to increase/ decrease bullet damage")]
        public float DamageMultiplier = 1;

        //when LivingEntityCollider is child of a GameObject with LivingEntity 
        //ParentLE can be set automatically through LivingEntity-Inspector (Button)
        [Tooltip("The Entity damage will be applied to")]
        public LivingEntity ParentLivingEntity;
    }
}
