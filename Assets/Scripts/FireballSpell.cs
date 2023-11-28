using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballSpell : MonoBehaviour
{
    public LayerMask targets;
    public int damage;
    private void OnCollisionEnter(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & targets) != 0)
        {
            collision.gameObject.GetComponent<EnemyHitable>().TakeDamage(damage);
        }
        Destroy(gameObject);
    }
}
