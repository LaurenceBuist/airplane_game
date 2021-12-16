using System;
using System.Xml.Schema;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Bolt.Samples.Photon.Lobby
{
    public class LobbyUIMainMenu : MonoBehaviour, ILobbyUI
    {
        public event Action OnCreateButtonClick;
        public event Action OnSpecialServerClick;
        public event Action OnJoinRandomClick;

        public string MatchName => matchNameInput.text;
        public string SpecialMatchName => specialMatchNameInput.text;

        [Header("Server UI")]
        [SerializeField] private TMP_InputField matchNameInput;
        [SerializeField] private Button createRoomButton;

        [Header("Client UI")]
        [SerializeField] private Button joinSpecialServer;
        [SerializeField] private TMP_InputField specialMatchNameInput;
        [SerializeField] private Button joinRandomButton;
        
        public void OnEnable()
        {
            createRoomButton.onClick.RemoveAllListeners();
            createRoomButton.onClick.AddListener(() =>
            {
                if (OnCreateButtonClick != null) OnCreateButtonClick();
            });
            
            joinSpecialServer.onClick.RemoveAllListeners();
            joinSpecialServer.onClick.AddListener(() =>
            {
                if (OnSpecialServerClick != null) OnSpecialServerClick();
            });
            
            joinRandomButton.onClick.RemoveAllListeners();
            joinRandomButton.onClick.AddListener(() =>
            {
                if (OnJoinRandomClick != null) OnJoinRandomClick();
            });

            
            String any = Guid.NewGuid().ToString();
            matchNameInput.text = any.Substring(any.Length - 5);

        }

        public void ToggleVisibility(bool visible)
        {
            gameObject.SetActive(visible);
        }
    }
}
