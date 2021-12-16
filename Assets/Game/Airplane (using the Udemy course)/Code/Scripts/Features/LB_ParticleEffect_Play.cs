using System.Collections.Generic;
using UnityEngine;

public class LB_ParticleEffect_Play : MonoBehaviour
{
    public GameObject particleEffect;
    public List<GameObject> Objects;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayAndDestroy()
    {
        if (Objects[0].activeInHierarchy)
        {
            particleEffect.GetComponent<ParticleSystem>().Play(true);
            foreach (GameObject x in Objects) x.SetActive(false);
        }
    }
}
