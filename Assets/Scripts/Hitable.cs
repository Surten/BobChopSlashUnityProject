using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class IntEvent : UnityEvent<int> { }

public class Hitable : MonoBehaviour
{
    protected int maxHealth = 1;
    protected int armor = 0;


    protected int currentHealth = 1;


    public ParticleSystem bloodParticle;
    public UnityEvent OnDeath;

    public virtual void Heal(int value)
    {
        value -= Math.Max((currentHealth + value) - maxHealth, 0);
        currentHealth += value;
    }

    public virtual void TakeDamage(int value)
    {
        int damageTaken = value - armor;
        if (damageTaken < 1) damageTaken = 1;
        currentHealth -= damageTaken;

        //particles
        var bloodPS = Instantiate(bloodParticle, transform);
        bloodPS.transform.parent = transform;
        Destroy(bloodPS.gameObject, 1f);
    }

    protected virtual void Die()
    {
        OnDeath.Invoke();
    }
}
