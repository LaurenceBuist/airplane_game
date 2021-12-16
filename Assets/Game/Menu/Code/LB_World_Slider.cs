using UnityEngine;

public class LB_World_Slider : MonoBehaviour
{
    #region Variables
    private int activeObject;
    #endregion

    #region Objects
    public GameObject[] gameObjects;
    public GameObject[] gameObjects2;
    #endregion

    private void Start()
    {
        activeObject = PlayerPrefs.GetInt("ActiveWorld", 0);

        try
        {
            gameObjects[activeObject].SetActive(true);
            gameObjects2[activeObject].SetActive(true);
        }
        catch (System.Exception)
        {
        }
    }

    #region Custom Methods

    public void Up()
    {
        if (activeObject < gameObjects.Length-1)
        {
            activeObject += 1;
            gameObjects[activeObject].SetActive(true);
            gameObjects[activeObject - 1].SetActive(false);
            
            gameObjects2[activeObject].SetActive(true);
            gameObjects2[activeObject - 1].SetActive(false);

            PlayerPrefs.SetInt("ActiveWorld", activeObject);
        }
    }

    public void Down()
    {
        if (activeObject > 0)
        {
            activeObject -= 1;
            gameObjects[activeObject].SetActive(true);
            gameObjects[activeObject + 1].SetActive(false);
            
            gameObjects2[activeObject].SetActive(true);
            gameObjects2[activeObject + 1].SetActive(false);

            PlayerPrefs.SetInt("ActiveWorld", activeObject);
        }
    }

    #endregion
}
