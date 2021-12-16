using ashleydavis;
using LaurenceBuist;
using UnityEngine;
using UnityEngine.SceneManagement;

/**
 * Controls the settings of the scene. This code runs automatically as soon as the scene has been loaded.
 */
public class LB_SceneSettings : MonoBehaviour
{
    // Post volume settings
    [Header("Post Volume")]
    public GameObject postVolumeDay;
    public GameObject postVolumeNight;

    // Light settings
    [Header("Lights")]
    public GameObject directionalLightDay;
    public GameObject directionalLightNight;

    // Skybox settings
    [Header("Skybox")]
    public Material skyBoxDay;
    public Material skyBoxNight;

    // Planes
    [Header("Planes")]
    public GameObject plane1;

    // Start is called before the first frame update
    void Start()
    {
        // Set the quality settings of the game to the player's preference
        QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("quality"), true);

        // Which time of the day is it
        #region Day Time
        postVolumeDay.SetActive(false);
        directionalLightDay.SetActive(false);
        postVolumeNight.SetActive(false);
        directionalLightNight.SetActive(false);

        // If set to light
        if (PlayerPrefs.GetInt("time") == 0)
        {
            if (PlayerPrefs.GetInt("quality") != 0) postVolumeDay.SetActive(true);
            directionalLightDay.SetActive(true);
            RenderSettings.skybox = skyBoxDay;          //Set skybox to day

            RenderSettings.fogColor = new Color32(157, 197, 214, 255);         //Change fog colour
        }

        // If set to night
        if (PlayerPrefs.GetInt("time") == 1)
        {
            if (PlayerPrefs.GetInt("quality") != 0) postVolumeNight.SetActive(true);
            directionalLightNight.SetActive(true);
            RenderSettings.skybox = skyBoxNight;        //Set skybox to night

            RenderSettings.fogColor = new Color32(52, 83, 96, 255);         //Change fog colour            
        }
        #endregion

        // Fog settings
        #region Fog Settings
        if (SceneManager.GetActiveScene().name == "World1")
        {
            RenderSettings.fogStartDistance = 250;
            RenderSettings.fogEndDistance = 500;
        }
        else if (SceneManager.GetActiveScene().name == "World2")
        {
            RenderSettings.fogStartDistance = 750;
            RenderSettings.fogEndDistance = 1000;
        }

        if (PlayerPrefs.GetInt("fog") == 1) RenderSettings.fog = true;
        else RenderSettings.fog = false;
        #endregion

        // If the world was loaded with spectator mode
        #region Spectator mode
        if (PlayerPrefs.GetInt("Spectator", 0) == 1)
        {
            // Instantiate the camera
            LB_Camera_AuthorativeMovement.Instantiate();

            // Set the authorative movement script to false
            LB_Camera_AuthorativeMovement.instance.GetComponent<LB_Camera_AuthorativeMovement>().enabled = false;

            // Set the spectator movement script to true
            LB_Camera_AuthorativeMovement.instance.GetComponent<LB_Camera_Spectator>().enabled = true;

            // Reset spectator preferences
            PlayerPrefs.SetInt("Spectator",0);
        }
        #endregion

        // If the world was loaded with free flight mode
        #region Free flight mode
        if (PlayerPrefs.GetInt("FreeFlight", 0) == 1)
        {
            // Instantiate the camera
            LB_Camera_AuthorativeMovement.Instantiate();

            // Set the authorative movement script to true
            LB_Camera_AuthorativeMovement.instance.GetComponent<LB_Camera_AuthorativeMovement>().enabled = true;

            // Set the spectator movement script to false
            LB_Camera_AuthorativeMovement.instance.GetComponent<LB_Camera_Spectator>().enabled = false;

            // Reset spectator preferences
            PlayerPrefs.SetInt("FreeFlight", 0);
        }
        #endregion
    }
}
