using System.Collections;
using System.Collections.Generic;
using Bolt.Matchmaking;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UIElements.Button;

namespace LaurenceBuist
{
    public class LB_Menu_CreateLobby : LB_Menu_Multiplayer
    {
        #region Variables

        [Header("Create Lobby")]
        public TMP_InputField LNInputField;

        public Image LNInputFieldU;

        public Button CreateLobby;
        
        #endregion
        // Start is called before the first frame update
        void Start()
        {
        }

        public void OnCreateLobby()
        {
            if (LNInputField.text != "")
            {
                LNInputFieldU.color = Color.white;
                BoltMatchmaking.CreateSession(
                    sessionID: LNInputField.text
                );
            }
            else
            {
                LNInputFieldU.color = Color.red;
            }
        }
        
    }
}