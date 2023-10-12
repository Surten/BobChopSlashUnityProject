using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitable : Hitable
{
    public Enemy1ScriptableObject enemyInfo;

    private void Start()
    {
        currentHealth = maxHealth = enemyInfo.maxHealth;
    }


    public override void TakeDamage(int value)
    {

        //sound effect
        base.TakeDamage(value);
        if (currentHealth <= 0) Die();
    }

    public override void Heal(int value)
    {
        base.Heal(value);
    }


    protected override void Die()
    {
        base.Die();
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        GetComponent<Rigidbody>().mass = 0.1f;
        GetComponent<EnemySmartAF>().SetEnemyState(EnemySmartAF.EnemyState.Dead);
    }


}
