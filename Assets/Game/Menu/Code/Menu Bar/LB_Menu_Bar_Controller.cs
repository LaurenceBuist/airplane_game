using LaurenceBuist;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/**
 * This script controls the top bar of the menu
 */
public class LB_Menu_Bar_Controller : MonoBehaviour
{
    #region Public Variables
    [Header("Menu Bar")]
    public GameObject[] MenuSlots;
    public Color32 activeSlotFontColor;
    public TMP_FontAsset activeFont;
    public TMP_FontAsset inactiveFont;

    [Header("Bottom Bar")]
    public GameObject[] BottomBarSlots;

    [Header("Menu Mobile Controller Script")]
    public LB_Menu_MobileController LB_Menu_MobileController;
    #endregion

    #region Private Variables
    private bool planesSlotLastActive = false; // True if the last active slot that was selected was the planes slot
    private bool settingsSlotLastActive = false; // True if the settings slot was last active
    #endregion

    //Sets background images and text colours of the single menu slots, depending on which one is clicked
    public void SwitchActiveMenuSlot(int active)
    {
        //Reset everything
        for (int i = 0; i < MenuSlots.Length; i++)
        {
            MenuSlots[i].GetComponent<Button>().interactable = true;
            MenuSlots[i].GetComponent<Image>().color = new Color32(255, 255, 255, 0);         //Background Image of Slot
            MenuSlots[i].GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
            MenuSlots[i].GetComponentInChildren<TextMeshProUGUI>().font = inactiveFont;
            
            BottomBarSlots[i].SetActive(false);
        }

        //Select correct menu bar slot
        MenuSlots[active].GetComponent<Button>().interactable = false;
        MenuSlots[active].GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        MenuSlots[active].GetComponentInChildren<TextMeshProUGUI>().color = activeSlotFontColor;
        MenuSlots[active].GetComponentInChildren<TextMeshProUGUI>().font = activeFont;
        BottomBarSlots[active].SetActive(true);


        // The planes slot was clicked
        if (active == 1)
        {
            LB_Menu_MobileController.ChangeCameraView_Planes();     // Change camera angle
            planesSlotLastActive = true;
        }

        // The planes slot was not clicked but it was last selected
        if (active != 1 && planesSlotLastActive)
        {
            LB_Menu_MobileController.ChangeCameraView_Planes();     // Reset camera angle
            planesSlotLastActive = false;     // Reset this
        }


        // The settings slot was clicked
        if (active == 3)
            settingsSlotLastActive = true;

        // The settings slot was not clicked but it was last active
        if (active != 3 && settingsSlotLastActive)
        {
            LB_Menu_MobileController.Save_Settings();       // Save the current settings
            settingsSlotLastActive = false;                 // Rest this
        }
    }

}
