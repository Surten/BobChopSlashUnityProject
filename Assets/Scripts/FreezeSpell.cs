using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeSpell : MonoBehaviour
{
    public LayerMask targets;
    public int damage;

    private void Start()
    {
        Ray ray = new Ray(transform.position, transform.forward);

    }
}
