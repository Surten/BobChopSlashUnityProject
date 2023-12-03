using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitable2 : Hitable
{
    public Enemy2ScriptableObject enemyInfo;

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
            GetComponent<EnemySmartAF2>().ShowFloatingText("0", textColor, textSizeMult, showflg);
            Die();
            return;
        }
        GetComponent<EnemySmartAF2>().ShowFloatingText(currentHealth.ToString(), textColor, textSizeMult, showflg);
        GetComponent<EnemySmartAF2>().LoadWavFile(EnemySmartAF2.SoundState.Staggering);

        EnemySmartAF2.EnemyState currentState = GetComponent<EnemySmartAF2>().GetEnemyState();
        if (currentState != EnemySmartAF2.EnemyState.Dead) GetComponent<EnemySmartAF2>().StaggerCoinFlip();
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
        GetComponent<EnemySmartAF2>().SetEnemyState(EnemySmartAF2.EnemyState.Dead);
    }


}
