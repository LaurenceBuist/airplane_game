
using UnityEngine;
using Photon.Bolt;

namespace LaurenceBuist
{
    [BoltGlobalBehaviour(BoltNetworkModes.Server)]
    public class LB_Multiplayer_ServerCallbacks :  GlobalEventListener
    {

        public override void Disconnected(BoltConnection connection)
        {
            var log = LogEvent.Create();
            log.Message = string.Format("{0} disconnected", connection.RemoteEndPoint);
            log.Send();
        }
    }
}
