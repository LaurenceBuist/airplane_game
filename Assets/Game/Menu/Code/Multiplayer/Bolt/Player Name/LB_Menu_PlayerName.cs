using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LB_Menu_PlayerName : MonoBehaviour
{
    public TMP_InputField playerInputField;

    private void OnEnable()
    {
        playerInputField.text = PlayerPrefs.GetString("PlayerName", "");
    }

    private void OnDisable()
    {
        PlayerPrefs.SetString("PlayerName", playerInputField.text);
    }
}
