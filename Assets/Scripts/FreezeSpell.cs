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
        Debug.Log(enemiesHit.Length);
        foreach (var enemy in enemiesHit)
        {
            Debug.Log(enemy.collider.gameObject.transform.position);
            enemy.collider.gameObject.GetComponent<EnemyHitable>().TakeDamage(damage);
        }
        Destroy(gameObject, 1f);
    }
}
