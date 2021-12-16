using System;
using System.Collections;
using Bolt.Matchmaking;
using Bolt.Samples.Photon.Lobby.Utilities;
using Photon.Realtime;
using TMPro;
//using Bolt.Samples.Photon.Simple;
using UdpKit;
using UdpKit.Platform;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bolt.Samples.Photon.Lobby
{
    public partial class LobbyManager : Bolt.GlobalEventListener
    {
        public static LobbyManager Instance;

        [Header("Lobby Configuration", order = 0)]
        [SerializeField] private SceneField lobbyScene;
        [SerializeField] private SceneField gameScene;

        [SerializeField] private int minPlayers = 1;
        [SerializeField] private TMP_InputField minPlayersInputField;

        [Tooltip("Time in second between all players ready & match start")] [SerializeField]
        private float prematchCountdown = 5.0f;

        private bool isCountdown = false;
        
        private string matchName;
        private string specialMatchName;
        
        private bool randomJoin = false;
        private bool specialJoin = false;

        private bool privateMatchNotFound = false;

        private void Awake()
        {
            BoltLauncher.SetUdpPlatform(new PhotonPlatform());
        }

        public new void OnEnable()
        {
            base.OnEnable();
            
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }

            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            StartUI();
            StartGamePlay();
        }

        private void StartGamePlay()
        {
            Debug.Log(string.Format("Lobby Scene: {0}", lobbyScene.SimpleSceneName));
            Debug.Log(string.Format("Game Scene: {0}", gameScene.SimpleSceneName));
        }

        // Game Loop

        private void FixedUpdate()
        {
            if (BoltNetwork.IsServer && isCountdown == false)
            {
                VerifyReady();
            }
        }

        // Countdown

        private void VerifyReady()
        {
            var allReady = true;
            var readyCount = 0;

            foreach (var entity in BoltNetwork.Entities)
            {
                if (entity.StateIs<ILobbyPlayerInfoState>() == false) continue;

                var lobbyPlayer = entity.GetState<ILobbyPlayerInfoState>();

                allReady &= lobbyPlayer.Ready;

                if (allReady == false) break;
                readyCount++;
            }

            //if (minPlayersInputField.text != "") {
                if (allReady && readyCount >= minPlayers)
                {
                    isCountdown = true;
                    StartCoroutine(ServerCountdownCoroutine());

                }
            //}
        }

        private IEnumerator ServerCountdownCoroutine()
        {
            var remainingTime = prematchCountdown;
            var floorTime = Mathf.FloorToInt(remainingTime);

            LobbyCountdown countdown;

            while (remainingTime > 0)
            {
                yield return null;

                remainingTime -= Time.deltaTime;
                var newFloorTime = Mathf.FloorToInt(remainingTime);

                if (newFloorTime != floorTime)
                {
                    floorTime = newFloorTime;

                    countdown = LobbyCountdown.Create(GlobalTargets.Everyone);
                    countdown.Time = floorTime;
                    countdown.Send();
                }
            }

            countdown = LobbyCountdown.Create(GlobalTargets.Everyone);
            countdown.Time = 0;
            countdown.Send();

   
        }

        // Bolt Callbacks

        //// API

        private void StartServerEventHandler(string matchName)
        {
            this.matchName = matchName;
            BoltLauncher.StartServer();
        }

        private void StartClientEventHandler(bool randomJoin = false)
        {
            this.randomJoin = randomJoin;
            BoltLauncher.StartClient();
        }
        
        private void StartSpecialClientEventHandler(string specialMatchName, bool specialJoin = true)
        {
            this.randomJoin = false;
            this.specialMatchName = specialMatchName;
            this.specialJoin = specialJoin;
            BoltLauncher.StartClient();
        }

        private void JoinEventHandler(UdpSession session)
        {
            if (BoltNetwork.IsClient)
            {
                BoltNetwork.Connect(session);
            }
        }
        

        private void ShutdownEventHandler()
        {
            BoltLauncher.Shutdown();
        }

        //// Callbacks

        public override void BoltStartBegin()
        {
            /*BoltNetwork.RegisterTokenClass<RoomProtocolToken>();
            BoltNetwork.RegisterTokenClass<ServerAcceptToken>();
            BoltNetwork.RegisterTokenClass<ServerConnectToken>();*/
        }

        public override void BoltStartDone()
        {
            if (BoltNetwork.IsServer)
            {
                /*var token = new RoomProtocolToken()
                {
                    ArbitraryData = "My DATA",
                };*/

                BoltLog.Info("Starting Server");

                // Start Photon Room
                BoltMatchmaking.CreateSession(
                    sessionID: matchName //,
                    //token: token
                );
            }
            else if (BoltNetwork.IsClient)
            {
                try
                {
                    if (randomJoin)
                    {
                        BoltMatchmaking.JoinRandomSession();
                    }
                    else if (specialJoin)
                    {
                        BoltMatchmaking.JoinSession(specialMatchName);
                    }
                    else
                    {
                        ClientStaredUIHandler();
                    }
                }
                catch (Exception e)
                {
                    Debug.Log("FUCK OFF ERROR: " + e);
                }

                randomJoin = false;
            }
        }

        public override void SessionCreated(UdpSession session)
        {
            SessionCreatedUIHandler(session);

            // Build Server Entity
            var entity = BoltNetwork.Instantiate(BoltPrefabs.PlayerInfo);
            entity.TakeControl();
        }

        public override void BoltShutdownBegin(AddCallback registerDoneCallback)
        {
            if(!privateMatchNotFound) 
                LoadingUI();

            matchName = "";

            if (lobbyScene.IsLoaded == false)
            {
                if (BoltNetwork.IsServer)
                {
                    BoltNetwork.LoadScene(lobbyScene.SimpleSceneName);
                }
                else if (BoltNetwork.IsClient)
                {
                    SceneManager.LoadScene(lobbyScene.SimpleSceneName);
                }
            }

            registerDoneCallback(() =>
            {
                Debug.Log("Shutdown Done");
                ResetUI();
            });

            privateMatchNotFound = false;
        }

        public override void EntityAttached(BoltEntity entity)
        {
            EntityAttachedEventHandler(entity);

            var photonPlayer = entity.gameObject.GetComponent<LobbyPlayer>();
            if (photonPlayer)
            {
                if (entity.IsControlled)
                {
                    photonPlayer.SetupPlayer();
                }
                else
                {
                    photonPlayer.SetupOtherPlayer();
                }
            }
        }

        public override void Connected(BoltConnection connection)
        {
            if (BoltNetwork.IsClient)
            {
                BoltConsole.Write(string.Format("Connected Client: {0}", connection), Color.blue);
                ClientConnectedUIHandler();
            }
            else if (BoltNetwork.IsServer)
            {
                BoltConsole.Write(string.Format("Connected Server: {0}", connection), Color.blue);

                var entity = BoltNetwork.Instantiate(BoltPrefabs.PlayerInfo);
                entity.AssignControl(connection);
            }
        }

        public override void Disconnected(BoltConnection connection)
        {
            foreach (var entity in BoltNetwork.Entities)
            {
                if (entity.StateIs<ILobbyPlayerInfoState>() == false || entity.IsController(connection) == false) continue;
                
                var player = entity.GetComponent<LobbyPlayer>();

                if (player)
                {
                    player.RemovePlayer();
                }
            }
        }

        public override void SessionConnectFailed(UdpSession session, Bolt.IProtocolToken token)
        {
            BoltLog.Error("Failed to connect to session {0} with token {1}", session, token);
            privateMatchNotFound = true;
            NothingFound();
            if(BoltNetwork.IsRunning) BoltLauncher.Shutdown();
        }


        public void ShutDown()
        {
            LoadingUI();
            if(BoltNetwork.IsRunning) BoltLauncher.Shutdown();
            
        }
    }
}