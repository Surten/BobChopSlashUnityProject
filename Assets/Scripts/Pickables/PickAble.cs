using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PickAble : MonoBehaviour
{
    public LayerMask layerMaskPickable;

    private void OnCollisionEnter(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & layerMaskPickable) != 0)
        {
            OnPickUp(collision.gameObject);
        }
    }

    protected virtual void OnPickUp(GameObject agentWhoPickedThisItemUp)
    {
        Destroy(gameObject);
    }
}
