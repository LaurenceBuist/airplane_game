using UnityEngine;

namespace Ballistics
{
    /// <summary>
    /// example for changing wind values in runtime
    /// </summary>
    public class DefaultWindManager : MonoBehaviour
    {

        public BallisticSettings Settings;
        public Vector3 MainWind = new Vector3(1f, 0f, 0.1f);
        public float MainWindStrength = 3f;
        public float RandomWindStrength = 1.5f;
        public float Speed = 0.5f;

        private Vector3 ToDirection;

        private Transform myTrans;

        void Awake()
        {
            MainWind.Normalize();
            Settings.WindVelocity = ToDirection = MainWind * MainWindStrength;
            myTrans = transform;
        }

        public void Update()
        {
            if ((ToDirection - Settings.WindVelocity).sqrMagnitude < .05f)
            {
                ToDirection = MainWind * MainWindStrength + Random.onUnitSphere * (Random.Range(0, RandomWindStrength) - (RandomWindStrength / 2));
            }
            else
            {
                Settings.WindVelocity = Vector3.Lerp(Settings.WindVelocity, ToDirection, Speed * Time.deltaTime / (ToDirection - Settings.WindVelocity).magnitude);
            }

            myTrans.rotation = Quaternion.LookRotation(Settings.WindVelocity);
            myTrans.localScale = Vector3.one * Settings.WindVelocity.magnitude;
        }

    }
}