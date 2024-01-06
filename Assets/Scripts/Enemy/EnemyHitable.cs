using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitable : Hitable
{
    public EnemyBaseScriptableObject enemyInfo;

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
            GetComponent<EnemySmart>().ShowFloatingText("0", textColor, textSizeMult, showflg);
            Die();
            return;
        }
        GetComponent<EnemySmart>().ShowFloatingText(currentHealth.ToString(), textColor, textSizeMult, showflg);

        EnemySmart.EnemyState currentState = GetComponent<EnemySmart>().GetEnemyState();
        if (currentState != EnemySmart.EnemyState.Dead) GetComponent<EnemySmart>().StaggerCoinFlip();
    }

    public override void Heal(int value)
    {
        base.Heal(value);
    }

    public void ScaleHealth(float val) {
        currentHealth = maxHealth = (int)(enemyInfo.maxHealth * val);
    }

    protected override void Die()
    {
        base.Die();
        EnemySmart e = GetComponent<EnemySmart>();
        e.SetEnemyState(EnemySmart.EnemyState.Dead);
        Invoke("MassLess", 2.9f);
    }

    protected void MassLess() {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.None;
        rb.mass = 0.1f;
    }

}
