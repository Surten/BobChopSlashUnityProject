using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPickable : PickAble
{
    protected override void OnPickUp(GameObject agentWhoPickedThisItemUp)
    {
        ShopManager.Instance.AddCoins(1);
        base.OnPickUp(agentWhoPickedThisItemUp);
    }
}
