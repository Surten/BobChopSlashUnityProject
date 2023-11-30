using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeSpell : MonoBehaviour
{
    public LayerMask targets;
    public int damage;

    private void Start()
    {
        RaycastHit[] enemiesHit = Physics.SphereCastAll(transform.position, 3f, transform.forward, 30f, targets);
        foreach (var enemy in enemiesHit)
        {
            enemy.collider.gameObject.GetComponent<EnemyHitable>().TakeDamage(damage);
        }
        Destroy(gameObject, 1f);
    }
}
