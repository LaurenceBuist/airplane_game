using UnityEngine;
using Photon.Bolt;

public class LB_AddToNetwork : EntityBehaviour<ILogSystem>
{
    public override void Attached()
    {
        state.SetTransforms(state.LogTransform, transform);
    }
}
