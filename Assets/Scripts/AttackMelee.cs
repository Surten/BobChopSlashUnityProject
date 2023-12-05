using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackMelee : MonoBehaviour
{

    public Animator anim;
    public Transform attackPoint;

    public float attackRadius = 1f;
    public int attackDamage = 5;

    public float attackRadiusHeavy = 3f;
    public int attackDamageHeavy = 15;

    public float attackSpeed = 1f;
    public LayerMask layerMask;

    public bool swingingSword = false;
    public bool swingingSwordHeavy = false;

    public void UpdateAttackSpeed(float percentageAdded)
    {
        attackSpeed += (percentageAdded * attackSpeed * 0.01f);
        anim.SetFloat("AttackSpeed", attackSpeed);
    }

    public void UpdateAttackRadius(float percentageAdded)
    {
        float volume = 4.0f / 3.0f * Mathf.PI * Mathf.Pow(attackRadius, 3);
        volume = volume + (percentageAdded * 0.01f * volume);
        attackRadius = Mathf.Pow((3.0f * volume)/(4 * Mathf.PI), 1.0f / 3.0f);
    }


    public void MeleeAttackLight()
    {
        if (!swingingSword)
        {
            StartCoroutine(LightAttackCoroutine());
        }
    }

    IEnumerator LightAttackCoroutine()
    {
        anim.SetTrigger("swordSlash");
        swingingSword = true;
        yield return new WaitForSeconds((1 / attackSpeed) * 0.6f);
        Collider[] enemiesHit = Physics.OverlapSphere(attackPoint.position, attackRadius, layerMask);
        ResolveSwordHit(enemiesHit, attackDamage);
        yield return new WaitForSeconds((1 / attackSpeed) * 0.4f);
        swingingSword = false;
    }


    public void MeleeAttackHeavy()
    {
        if (!swingingSword)
        {
            StartCoroutine(HeavyAttackCoroutine());
        }
    }

    IEnumerator HeavyAttackCoroutine()
    {
        anim.SetTrigger("swordSlashHeavy");
        swingingSword = true;
        swingingSwordHeavy = true;
        yield return new WaitForSeconds((1 / attackSpeed) * 2.3f);
        Collider[] enemiesHit = Physics.OverlapSphere(attackPoint.position, attackRadiusHeavy, layerMask);
        ResolveSwordHit(enemiesHit, attackDamageHeavy);
        yield return new WaitForSeconds((1 / attackSpeed) * 1.5f);
        swingingSword = false;
        swingingSwordHeavy = false;
    }

    void ResolveSwordHit(Collider[] enemiesHit, int damage)
    {
        foreach (Collider enemy in enemiesHit)
        {
            enemy.GetComponent<Hitable>().TakeDamage(damage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}
