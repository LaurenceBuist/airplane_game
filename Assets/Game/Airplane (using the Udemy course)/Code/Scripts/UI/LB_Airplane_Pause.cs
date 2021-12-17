using Photon.Bolt;
using Photon.Bolt.Matchmaking;
using UnityEngine;

public class LB_Airplane_Pause : MonoBehaviour//EntityBehaviour<IPlaneState>
{
    public void LeaveRoom()
    {
        /*if (!BoltNetwork.IsServer)
        {
            var evnt = PlayerJoinedEvent.Create();
            evnt.Message = PlayerPrefs.GetString("Player") + "left the game";
            evnt.Send();
        }
        if(BoltNetwork.IsServer) 
            BoltLauncher.Shutdown();
        else 
            BoltNetwork.Server.Disconnect();*/
    }
}
