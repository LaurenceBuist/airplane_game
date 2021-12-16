using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace LaurenceBuist
{
    /**
     * Loads scene async with loading bar
     */
    public class LB_LoadingScene : MonoBehaviour
    {
        #region Variables
        public GameObject LoadingUI;            // The UI of the loading bar and text
        public GameObject[] DisableObjects;     // Objects to disable when loading
        public Image bar;   // Loading bar
        #endregion
        
        // Start is called before the first frame update
        public void LoadSceneAsync(int scene_input)
        {
            int scene = 4; // Default scene //PlayerPrefs.GetInt("ActiveWorld", 0)
            if (scene_input > -1)
                scene = scene_input;
            
            // Turn on the loading UI
            LoadingUI.SetActive(true);

            // Turns off the menu UI
            if (DisableObjects.Length > 0)
            {
                foreach (GameObject obj in DisableObjects)
                    obj.SetActive(false);
            }

            // Loads scene async
            StartCoroutine(LoadAsyncOperation(scene));
        }

        IEnumerator LoadAsyncOperation(int scene)
        {
            AsyncOperation gameLevel = SceneManager.LoadSceneAsync(scene);
            while (gameLevel.progress < 1)
            {
                bar.fillAmount = gameLevel.progress;    // Update loading bar
                yield return new WaitForEndOfFrame();
            }
            yield return new WaitForEndOfFrame();
        }
    }
}