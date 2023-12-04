using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpOrbPickable : PickAble
{
    protected override void OnPickUp(GameObject agentWhoPickedThisItemUp)
    {
        PlayerLeveler.Instance.AddExp(1);
        base.OnPickUp(agentWhoPickedThisItemUp);
    }
}
