using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonPuddle : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<Hitable>().TakeDamage(5);
    }
}
