using UnityEngine;

public class LB_AddToNetwork : Bolt.EntityBehaviour<ILogSystem>
{
    public override void Attached()
    {
        state.SetTransforms(state.LogTransform, transform);
    }
}
