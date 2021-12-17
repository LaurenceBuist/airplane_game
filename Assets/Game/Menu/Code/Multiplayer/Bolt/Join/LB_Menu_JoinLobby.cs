using Photon.Bolt.Matchmaking;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LaurenceBuist
{
    public class LB_Menu_JoinLobby : LB_Menu_Multiplayer
    {
        #region Variables

        [Header("Create Lobby")]
        public TMP_InputField LNInputField;

        public Image LNInputFieldU;

        #endregion
        // Start is called before the first frame update
        void Start()
        {
        }

        public void OnJoinLobby()
        {
            if (LNInputField.text != "")
            {
                LNInputFieldU.color = Color.white;
                BoltMatchmaking.JoinSession(LNInputField.text);
                /*BoltMatchmaking.JoinSession(
                    sessionID: LNInputField.text
                );*/
            }
            else
            {
                LNInputFieldU.color = Color.red;
            }
        }
    }
}