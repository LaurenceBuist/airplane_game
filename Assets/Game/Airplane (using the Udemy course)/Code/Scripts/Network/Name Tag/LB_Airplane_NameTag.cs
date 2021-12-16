using UnityEngine;

namespace LaurenceBuist
{
    public class LB_Airplane_NameTag : MonoBehaviour
    {
        private Camera camera;

        [Header("Name Tag Canvas")]
        public GameObject nameTagGameObject;
        private bool isActive;
        public RectTransform nameTagCanvas;
        
        [Header("Target plane")]
        public GameObject plane;
        public Vector3 targetOffset = new Vector3(0, 2, 0);

        [Header("Name Tag")]
        public RectTransform thisRect;
        
        // Start is called before the first frame update
        void Start()
        {
            //Find camera in scene
            camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        }

        // Update is called once per frame
        void Update()
        {
            Vector3 heading = plane.transform.position - camera.transform.position;
            if (Vector3.Dot(camera.transform.forward, heading) > 0)
            {
                //Plane is in front of camera
                if (!isActive) nameTagGameObject.SetActive(true);
                
                Vector2 point = camera.WorldToScreenPoint(plane.transform.position + targetOffset);
                thisRect.anchoredPosition = point - nameTagCanvas.sizeDelta / 2f;
            }
            else
            {
                if(isActive) nameTagGameObject.SetActive(false);
            }
        }
    }
}