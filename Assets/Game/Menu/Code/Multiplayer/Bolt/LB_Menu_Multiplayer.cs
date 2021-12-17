
using System;
using System.Collections;
using Photon.Bolt;
using Photon.Bolt.Matchmaking;
using UdpKit;
using UdpKit.Platform;
using UnityEngine;

namespace LaurenceBuist
{
    public class LB_Menu_Multiplayer : GlobalEventListener
    {
        public String[] Worlds;

        public void StartFreeFlight()
        {
            BoltLauncher.StartSinglePlayer();
        }
        
        public override void BoltStartDone()
        {
            Debug.Log(Worlds[PlayerPrefs.GetInt("ActiveWorld", 0)]);
            if (BoltNetwork.IsServer)
            {
                string matchName = Guid.NewGuid().ToString();

                BoltMatchmaking.CreateSession(
                    sessionID: matchName,
                    sceneToLoad: Worlds[1]      //PlayerPrefs.GetInt("ActiveWorld", 0)
                );
            }
        }

        public override void SessionListUpdated(Map<Guid, UdpSession> sessionList)
        {
            Debug.LogFormat("Session list updated: {0} total sessions", sessionList.Count);

            foreach (var session in sessionList)
            {
                UdpSession photonSession = session.Value as UdpSession;

                if (photonSession.Source == UdpSessionSource.Photon)
                {
                    //BoltNetwork.Connect(photonSession);
                }
            }
        }
    }
}