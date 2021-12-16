using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LB_Clouds_Controller : MonoBehaviour
{
    #region Variables
    public Material LowClouds;
    public Material HighClouds;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "Menu")
        {
            LowClouds.SetColor("_CloudColor", new Color32(255, 255, 255, 47));
            LowClouds.SetFloat("_Density", 1.2f);
            HighClouds.SetColor("_CloudColor", new Color32(255, 255, 255, 235));
            HighClouds.SetFloat("_Density", 1.74f);
        }
        else
        {
            if (PlayerPrefs.GetInt("time", 0) == 0)
            {
                LowClouds.SetColor("_CloudColor", new Color32(255, 255, 255, 47));
                HighClouds.SetColor("_CloudColor", new Color32(255, 255, 255, 235));
            }
            else if (PlayerPrefs.GetInt("time") == 1)
            {
                LowClouds.SetColor("_CloudColor", new Color32(118, 137, 173, 47));
                HighClouds.SetColor("_CloudColor", new Color32(62, 72, 103, 235));
            }
        }
    }
}
