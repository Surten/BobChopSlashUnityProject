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
        float textSizeMult = 1f;
        Color textColor = Color.white;
        bool showflg = true;

        //sound effect
        base.TakeDamage(value);
        if (currentHealth <= 0) {
            GetComponent<EnemySmartAF>().ShowFloatingText("0", textColor, textSizeMult, showflg);
            Die();
            return;
        }
        GetComponent<EnemySmartAF>().ShowFloatingText(currentHealth.ToString(), textColor, textSizeMult, showflg);

        EnemySmartAF.EnemyState currentState = GetComponent<EnemySmartAF>().GetEnemyState();
        if (currentState != EnemySmartAF.EnemyState.Dead) GetComponent<EnemySmartAF>().StaggerCoinFlip();
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
