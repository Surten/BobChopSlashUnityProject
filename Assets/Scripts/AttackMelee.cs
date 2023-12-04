using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackMelee : MonoBehaviour
{

    public Animator anim;
    public Transform attackPoint;
    public float attackRadius = 1f;
    public int attackDamage = 5;
    public float attackSpeed = 1f;

    public LayerMask layerMask;

    public bool swingingSwordCR = false;

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


    public void SwingSword()
    {
        if (!swingingSwordCR)
        {
            StartCoroutine(Attacking());
        }


    }

    IEnumerator Attacking()
    {
        anim.SetTrigger("swordSlash");
        swingingSwordCR = true;
        yield return new WaitForSeconds((1 / attackSpeed) * 0.6f);
        ResolveSwordHit();
        yield return new WaitForSeconds((1 / attackSpeed) * 0.4f);
        swingingSwordCR = false;
    }

    void ResolveSwordHit()
    {
        Collider[] enemiesHit = Physics.OverlapSphere(attackPoint.position, attackRadius, layerMask);
        foreach (Collider enemy in enemiesHit)
        {
            enemy.GetComponent<Hitable>().TakeDamage(attackDamage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}
